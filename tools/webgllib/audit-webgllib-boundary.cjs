const fs = require("fs");
const path = require("path");

const repoRoot = path.resolve(__dirname, "..", "..");
const webGlLibRoot = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib");
const webGlLibProject = path.join(webGlLibRoot, "CanDoItAll.Components.WebGlLib.csproj");
const webGlLibOnlySampleRoot = path.join(repoRoot, "samples", "CanDoItAll.Components.WebGlLibOnlyViewer");
const webGlLibOnlySampleProject = path.join(webGlLibOnlySampleRoot, "CanDoItAll.Components.WebGlLibOnlyViewer.csproj");

const forbiddenPatterns = [
    { pattern: /CanDoItAll\.Components\.WebGlRunLib/i, reason: "WebGlLib must not reference WebGlRunLib." },
    { pattern: /CanDoItAll\.Economy/i, reason: "Components WebGlLib must not reference Economy assemblies." },
    { pattern: /\bEconomy\b/i, reason: "Economy terms belong in consuming domain packages." },
    { pattern: /\bLedger\b/i, reason: "Ledger semantics belong in Economy or another domain package." },
    { pattern: /\bMarket\b/i, reason: "Market semantics belong in Economy or another domain package." },
    { pattern: /\bVernon\b/i, reason: "Experiment-specific semantics do not belong in WebGlLib." },
    { pattern: /\bProductionLine\b/i, reason: "Production-line semantics do not belong in WebGlLib." },
    { pattern: /\bWorkOrder\b/i, reason: "Production/work-order semantics do not belong in WebGlLib." },
    { pattern: /\bMachine\b/i, reason: "Production-machine semantics do not belong in WebGlLib." }
];

const sourceExtensions = new Set([".cs", ".razor", ".js", ".csproj", ".props", ".targets"]);
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
            if (["bin", "obj", "vendor", "node_modules"].includes(entry.name)) {
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
    const extension = path.extname(filePath);
    if (!sourceExtensions.has(extension)) {
        return;
    }

    scanText(toRepoPath(filePath), read(filePath));
}

function assertNoForbiddenProjectReference(filePath) {
    if (!fs.existsSync(filePath)) {
        findings.push(`${toRepoPath(filePath)}: expected project file is missing.`);
        return;
    }

    const text = read(filePath);
    const projectReferenceMatches = [...text.matchAll(/<ProjectReference\s+Include="([^"]+)"/gi)].map(match => match[1]);
    for (const reference of projectReferenceMatches) {
        if (/WebGlRunLib|Economy/i.test(reference)) {
            findings.push(`${toRepoPath(filePath)}: forbidden project reference '${reference}'.`);
        }
    }
}

assertNoForbiddenProjectReference(webGlLibProject);
assertNoForbiddenProjectReference(webGlLibOnlySampleProject);
walk(webGlLibRoot, scanFile);
walk(webGlLibOnlySampleRoot, scanFile);

if (process.env.WEBGLLIB_BOUNDARY_AUDIT_PROBE) {
    scanText("boundary-audit-probe", process.env.WEBGLLIB_BOUNDARY_AUDIT_PROBE);
}

if (findings.length > 0) {
    console.error("WebGlLib boundary audit failed:");
    for (const finding of findings) {
        console.error(`- ${finding}`);
    }

    process.exit(1);
}

console.log("WebGlLib boundary audit passed.");
console.log("- WebGlLib has no WebGlRunLib or Economy project reference.");
console.log("- WebGlLib first-party source has no forbidden domain terms.");
console.log("- WebGlLib-only sample has no WebGlRunLib or Economy dependency.");
