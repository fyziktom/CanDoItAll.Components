const fs = require("fs");
const path = require("path");

const repoRoot = path.resolve(__dirname, "..", "..");
const webGlRunLibRoot = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlRunLib");
const webGlRunLibProject = path.join(webGlRunLibRoot, "CanDoItAll.Components.WebGlRunLib.csproj");

const forbiddenPatterns = [
    { pattern: /CanDoItAll\.Economy/i, reason: "WebGlRunLib must not reference Economy assemblies." },
    { pattern: /\bEconomy\b/i, reason: "Economy terms belong in consuming domain packages." },
    { pattern: /\bLedger\b/i, reason: "Ledger semantics belong in Economy or another domain package." },
    { pattern: /\bMarket\b/i, reason: "Market semantics belong in Economy or another domain package." },
    { pattern: /\bVernon\b/i, reason: "Experiment-specific semantics do not belong in WebGlRunLib." },
    { pattern: /\bProductionLine\b/i, reason: "Production-line semantics do not belong in WebGlRunLib." },
    { pattern: /\bWorkOrder\b/i, reason: "Production/work-order semantics do not belong in WebGlRunLib." },
    { pattern: /\bMachine\b/i, reason: "Production-machine semantics do not belong in WebGlRunLib." },
    { pattern: /\bBuyer\b/i, reason: "Buyer semantics belong in a domain package." },
    { pattern: /\bSeller\b/i, reason: "Seller semantics belong in a domain package." },
    { pattern: /\bPrice\b/i, reason: "Price semantics belong in a domain package." },
    { pattern: /\bAccount\b/i, reason: "Account semantics belong in a domain package." }
];

const sourceExtensions = new Set([".cs", ".csproj", ".props", ".targets"]);
const findings = [];

function toRepoPath(filePath) {
    return path.relative(repoRoot, filePath).replaceAll(path.sep, "/");
}

function read(filePath) {
    return fs.readFileSync(filePath, "utf8");
}

function walk(root, visitor) {
    if (!fs.existsSync(root)) {
        return;
    }

    for (const entry of fs.readdirSync(root, { withFileTypes: true })) {
        const fullPath = path.join(root, entry.name);
        if (entry.isDirectory()) {
            if (["bin", "obj", "node_modules"].includes(entry.name)) {
                continue;
            }

            walk(fullPath, visitor);
            continue;
        }

        visitor(fullPath);
    }
}

function scanText(label, text) {
    for (const { pattern, reason } of forbiddenPatterns) {
        if (pattern.test(text)) {
            findings.push(`${label}: ${reason}`);
        }
    }
}

function scanFile(filePath) {
    if (!sourceExtensions.has(path.extname(filePath))) {
        return;
    }

    if (filePath.endsWith(path.join("Documents", "WebGlRunDocumentValidator.cs"))) {
        return;
    }

    scanText(toRepoPath(filePath), read(filePath));
}

function assertProjectReferences(filePath) {
    if (!fs.existsSync(filePath)) {
        findings.push(`${toRepoPath(filePath)}: expected project file is missing.`);
        return;
    }

    const text = read(filePath);
    const references = [...text.matchAll(/<ProjectReference\s+Include="([^"]+)"/gi)].map(match => match[1]);
    for (const reference of references) {
        if (!/CanDoItAll\.Components\.WebGlLib\.csproj/i.test(reference)) {
            findings.push(`${toRepoPath(filePath)}: unexpected project reference '${reference}'.`);
        }

        if (/Economy/i.test(reference)) {
            findings.push(`${toRepoPath(filePath)}: forbidden domain project reference '${reference}'.`);
        }
    }
}

assertProjectReferences(webGlRunLibProject);
walk(webGlRunLibRoot, scanFile);

if (process.env.WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE) {
    scanText("boundary-audit-probe", process.env.WEBGLRUNLIB_BOUNDARY_AUDIT_PROBE);
}

if (findings.length > 0) {
    console.error("WebGlRunLib boundary audit failed:");
    for (const finding of findings) {
        console.error(`- ${finding}`);
    }

    process.exit(1);
}

console.log("WebGlRunLib boundary audit passed.");
console.log("- WebGlRunLib references WebGlLib only.");
console.log("- WebGlRunLib first-party source has no forbidden domain terms.");
