const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const defaultConfigPath = path.join(__dirname, "domain-boundary-audit.config.json");

function runBoundaryAudit(options = {}) {
    const args = parseArgs(process.argv.slice(2));
    const profileName = args.profile || options.defaultProfile || "webglrunlib";
    const configPath = path.resolve(repoRoot, args.config || defaultConfigPath);
    const config = loadJson(configPath);
    assertSchema(config, configPath);

    const terms = loadForbiddenDomainTerms(config, configPath);
    const patterns = buildForbiddenDomainTermPatterns(terms);
    const profile = config.profiles?.[profileName];
    if (!profile) {
        throw new Error(`Unknown domain-boundary audit profile '${profileName}'.`);
    }

    const globalAllowlist = normalizeAllowlist(config.globalAllowlist);
    const findings = [];
    const allowedFindings = [];
    const scannedFiles = new Set();
    const scannedScopes = [];

    for (const rule of profile.projectReferenceRules || []) {
        auditProjectReferenceRule(rule, findings);
    }

    for (const scope of profile.scopes || []) {
        const scopeAllowlist = mergeAllowlists(globalAllowlist, normalizeAllowlist(scope.allowlist));
        const scopeStats = scanScope(scope, config, patterns, scopeAllowlist, findings, allowedFindings, scannedFiles);
        scannedScopes.push(scopeStats);
    }

    if (process.env.WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE) {
        scanText({
            label: "WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE",
            repoPath: "WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE",
            text: process.env.WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE,
            patterns,
            allowlist: emptyAllowlist(),
            findings,
            allowedFindings
        });
    }

    if (process.env.WEBGLLIB_BOUNDARY_AUDIT_PROBE) {
        scanText({
            label: "WEBGLLIB_BOUNDARY_AUDIT_PROBE",
            repoPath: "WEBGLLIB_BOUNDARY_AUDIT_PROBE",
            text: process.env.WEBGLLIB_BOUNDARY_AUDIT_PROBE,
            patterns,
            allowlist: emptyAllowlist(),
            findings,
            allowedFindings
        });
    }

    if (findings.length > 0) {
        console.error(`Domain boundary audit failed for profile '${profileName}'.`);
        for (const finding of findings) {
            console.error(`- ${finding}`);
        }

        process.exit(1);
    }

    console.log(`Domain boundary audit passed for profile '${profileName}'.`);
    console.log(`- Term registry: ${terms.length} forbidden term(s) from ${config.termFiles.length} config file(s).`);
    console.log(`- Scanned files: ${scannedFiles.size}.`);
    for (const scope of scannedScopes) {
        console.log(`- Scope '${scope.name}': ${scope.fileCount} file(s), ${scope.allowedFindingCount} allowlisted match(es).`);
    }

    if (allowedFindings.length > 0) {
        const byReason = new Map();
        for (const finding of allowedFindings) {
            const key = finding.reason;
            byReason.set(key, (byReason.get(key) || 0) + 1);
        }

        console.log("- Allowlisted match reasons:");
        for (const [reason, count] of [...byReason.entries()].sort((left, right) => left[0].localeCompare(right[0]))) {
            console.log(`  - ${count}x ${reason}`);
        }
    }
}

function loadForbiddenDomainTerms(config = loadJson(defaultConfigPath), configPath = defaultConfigPath) {
    const terms = [];
    for (const termFile of config.termFiles || []) {
        const resolvedPath = path.resolve(path.dirname(configPath), "..", "..", termFile);
        const json = loadJson(resolvedPath);
        const values = Array.isArray(json)
            ? json
            : json.forbiddenDomainTerms || json.terms || [];
        for (const value of values) {
            if (typeof value === "string" && value.trim()) {
                terms.push(value.trim());
            }
        }
    }

    return [...new Set(terms.map(term => term.toLowerCase()))].sort((left, right) => left.localeCompare(right));
}

function buildForbiddenDomainTermPatterns(terms = loadForbiddenDomainTerms()) {
    return terms.map(term => ({
        term,
        pattern: createTermPattern(term)
    }));
}

function containsForbiddenDomainTerm(value, patterns = buildForbiddenDomainTermPatterns()) {
    if (!value) {
        return false;
    }

    return patterns.some(item => item.pattern.test(value));
}

function scanScope(scope, config, patterns, allowlist, findings, allowedFindings, scannedFiles) {
    const extensions = new Set(scope.extensions || config.defaultExtensions || []);
    const excludeDirs = new Set([...(config.defaultExcludeDirs || []), ...(scope.excludeDirs || [])]);
    let fileCount = 0;
    let allowedFindingCount = 0;

    for (const root of scope.roots || []) {
        const rootPath = path.resolve(repoRoot, root);
        walk(rootPath, excludeDirs, filePath => {
            if (!extensions.has(path.extname(filePath))) {
                return;
            }

            const repoPath = toRepoPath(filePath);
            fileCount += 1;
            scannedFiles.add(repoPath);
            const before = allowedFindings.length;
            scanText({
                label: repoPath,
                repoPath,
                text: read(filePath),
                patterns,
                allowlist,
                findings,
                allowedFindings
            });
            allowedFindingCount += allowedFindings.length - before;
        });
    }

    return {
        name: scope.name || "(unnamed)",
        fileCount,
        allowedFindingCount
    };
}

function scanText({ label, repoPath, text, patterns, allowlist, findings, allowedFindings }) {
    const lines = text.split(/\r?\n/);
    for (let index = 0; index < lines.length; index++) {
        const line = lines[index];
        for (const item of patterns) {
            item.pattern.lastIndex = 0;
            if (!item.pattern.test(line)) {
                continue;
            }

            const allowed = resolveAllowlist(repoPath, index + 1, line, item.term, allowlist);
            if (allowed) {
                allowedFindings.push({
                    path: repoPath,
                    line: index + 1,
                    term: item.term,
                    reason: allowed.reason
                });
                continue;
            }

            findings.push(`${label}:${index + 1} contains forbidden domain term '${item.term}': ${line.trim()}`);
        }
    }
}

function auditProjectReferenceRule(rule, findings) {
    const projectPath = path.resolve(repoRoot, rule.project);
    if (!fs.existsSync(projectPath)) {
        findings.push(`${rule.project}: expected project file is missing.`);
        return;
    }

    const text = read(projectPath);
    const references = [...text.matchAll(/<ProjectReference\s+Include="([^"]+)"/gi)].map(match => match[1]);
    const allowedProjectFileNames = new Set(rule.allowedProjectFileNames || []);
    for (const reference of references) {
        const fileName = path.basename(reference);
        if (allowedProjectFileNames.size > 0 && !allowedProjectFileNames.has(fileName)) {
            findings.push(`${rule.project}: unexpected project reference '${reference}'.`);
        }

        for (const fragment of rule.forbiddenReferenceFragments || []) {
            if (reference.includes(fragment)) {
                findings.push(`${rule.project}: forbidden project reference '${reference}'.`);
            }
        }
    }
}

function normalizeAllowlist(allowlist = {}) {
    return {
        paths: (allowlist.paths || []).map(item => ({
            ...item,
            globPattern: normalizeRepoPath(item.glob || "")
        })),
        lines: (allowlist.lines || []).map(item => ({
            ...item,
            regex: new RegExp(item.pattern, item.flags || "i"),
            terms: item.terms ? new Set(item.terms.map(term => term.toLowerCase())) : null
        }))
    };
}

function emptyAllowlist() {
    return { paths: [], lines: [] };
}

function mergeAllowlists(...allowlists) {
    return {
        paths: allowlists.flatMap(item => item.paths || []),
        lines: allowlists.flatMap(item => item.lines || [])
    };
}

function resolveAllowlist(repoPath, lineNumber, line, term, allowlist) {
    for (const item of allowlist.paths) {
        if (matchesGlob(repoPath, item.globPattern)) {
            return { reason: item.reason || `path allowlist ${item.glob}` };
        }
    }

    for (const item of allowlist.lines) {
        if (item.terms && !item.terms.has(term.toLowerCase())) {
            continue;
        }

        item.regex.lastIndex = 0;
        if (item.regex.test(line)) {
            return { reason: item.reason || `line allowlist at ${repoPath}:${lineNumber}` };
        }
    }

    return null;
}

function walk(root, excludeDirs, visitor) {
    if (!fs.existsSync(root)) {
        return;
    }

    const entry = fs.statSync(root);
    if (entry.isFile()) {
        visitor(root);
        return;
    }

    for (const dirent of fs.readdirSync(root, { withFileTypes: true })) {
        const fullPath = path.join(root, dirent.name);
        if (dirent.isDirectory()) {
            if (excludeDirs.has(dirent.name)) {
                continue;
            }

            walk(fullPath, excludeDirs, visitor);
            continue;
        }

        visitor(fullPath);
    }
}

function createTermPattern(term) {
    const escaped = escapeRegExp(term).replace(/\\ /g, "[\\s-]+");
    const hasOnlyWordCharacters = /^[A-Za-z0-9_]+$/.test(term);
    return new RegExp(hasOnlyWordCharacters ? `\\b${escaped}\\b` : escaped, "i");
}

function matchesGlob(repoPath, globPattern) {
    const normalizedPath = normalizeRepoPath(repoPath);
    if (globPattern.endsWith("/**")) {
        return normalizedPath.startsWith(globPattern.slice(0, -3));
    }

    if (!globPattern.includes("*")) {
        return normalizedPath === globPattern;
    }

    const regex = new RegExp(`^${escapeRegExp(globPattern).replace(/\\\*\\\*/g, ".*").replace(/\\\*/g, "[^/]*")}$`);
    return regex.test(normalizedPath);
}

function parseArgs(args) {
    const parsed = {};
    for (let index = 0; index < args.length; index++) {
        const arg = args[index];
        if (arg === "--config") {
            parsed.config = args[++index];
        } else if (arg === "--profile") {
            parsed.profile = args[++index];
        }
    }

    return parsed;
}

function assertSchema(config, configPath) {
    if (config.schemaVersion !== "domain-boundary-audit/v1") {
        throw new Error(`${configPath}: unsupported or missing schemaVersion.`);
    }

    if (!Array.isArray(config.termFiles) || config.termFiles.length === 0) {
        throw new Error(`${configPath}: termFiles must contain at least one registry.`);
    }

    validateAllowlistMetadata(config.globalAllowlist, "globalAllowlist");
    for (const [profileName, profile] of Object.entries(config.profiles || {})) {
        for (const scope of profile.scopes || []) {
            validateAllowlistMetadata(scope.allowlist, `profiles.${profileName}.scopes.${scope.name || "(unnamed)"}.allowlist`);
        }
    }
}

function validateAllowlistMetadata(allowlist, label) {
    if (!allowlist) {
        return;
    }

    for (const [index, item] of (allowlist.paths || []).entries()) {
        const entryLabel = `${label}.paths[${index}]`;
        requireAllowlistValue(item.glob, `${entryLabel}.glob`);
        requireAllowlistValue(item.reason, `${entryLabel}.reason`);
        requireAllowlistValue(item.owner, `${entryLabel}.owner`);
        requireAllowlistValue(item.expires, `${entryLabel}.expires`);
        assertNotExpired(item.expires, `${entryLabel}.expires`);
    }
}

function requireAllowlistValue(value, label) {
    if (typeof value !== "string" || value.trim().length === 0) {
        throw new Error(`${label} is required for domain-boundary allowlist entries.`);
    }
}

function assertNotExpired(value, label) {
    const parsed = Date.parse(`${value}T23:59:59Z`);
    if (Number.isNaN(parsed)) {
        throw new Error(`${label} must be an ISO yyyy-mm-dd date.`);
    }

    if (parsed < Date.now()) {
        throw new Error(`${label} expired on ${value}.`);
    }
}

function read(filePath) {
    return fs.readFileSync(filePath, "utf8");
}

function loadJson(filePath) {
    return JSON.parse(read(filePath));
}

function toRepoPath(filePath) {
    return normalizeRepoPath(path.relative(repoRoot, filePath));
}

function normalizeRepoPath(value) {
    return value.replaceAll("\\", "/");
}

function escapeRegExp(value) {
    return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

module.exports = {
    buildForbiddenDomainTermPatterns,
    containsForbiddenDomainTerm,
    loadForbiddenDomainTerms,
    runBoundaryAudit
};

if (require.main === module) {
    runBoundaryAudit();
}
