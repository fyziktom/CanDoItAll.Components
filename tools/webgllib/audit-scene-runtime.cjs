const fs = require("node:fs");
const path = require("node:path");
const { spawnSync } = require("node:child_process");

const repoRoot = path.resolve(__dirname, "..", "..");
const sceneRuntimeDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const componentRoot = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "Components", "Shared", "Assets");
const warningLineThreshold = 220;
const failureLineThreshold = 320;
const thinFacadeLineThreshold = 180;

let failures = 0;
let warnings = 0;

const runtimeFiles = fs.readdirSync(sceneRuntimeDir)
  .filter(file => file.endsWith(".js"))
  .sort((left, right) => left.localeCompare(right))
  .map(file => path.join(sceneRuntimeDir, file));

for (const filePath of runtimeFiles) {
  auditLineCount(filePath);
  auditUnsafePatterns(filePath);
  auditSyntax(filePath);
}

auditPublicFacade();
auditAssetIncludes();
auditBranchInstructionFiles();

if (failures > 0) {
  console.error(`Scene runtime audit failed with ${failures} failure(s) and ${warnings} warning(s).`);
  process.exit(1);
}

console.log(`Scene runtime audit passed with ${warnings} warning(s).`);

function auditLineCount(filePath) {
  const relativePath = relative(filePath);
  const lines = read(filePath).split(/\r?\n/).length;
  if (path.basename(filePath) === "01-webgl-scene.js" && lines > thinFacadeLineThreshold) {
    fail(`${relativePath} has ${lines} lines; public facade should stay under ${thinFacadeLineThreshold}.`);
    return;
  }

  if (lines > failureLineThreshold) {
    fail(`${relativePath} has ${lines} lines; hard threshold is ${failureLineThreshold}.`);
    return;
  }

  if (lines > warningLineThreshold) {
    warn(`${relativePath} has ${lines} lines; warning threshold is ${warningLineThreshold}.`);
  }
}

function auditUnsafePatterns(filePath) {
  const relativePath = relative(filePath);
  const lines = read(filePath).split(/\r?\n/);
  lines.forEach((line, index) => {
    const lineNumber = index + 1;
    if (/\.\s*innerHTML\s*=/.test(line) && !hasStaticInnerHtmlAllowlist(lines, index)) {
      fail(`${relativePath}:${lineNumber} uses innerHTML without a static-safe allowlist comment.`);
    }

    const forbidden = [
      { pattern: /\beval\s*\(/, label: "eval" },
      { pattern: /\bnew\s+Function\s*\(/, label: "new Function" },
      { pattern: /document\.write\s*\(/, label: "document.write" },
      { pattern: /insertAdjacentHTML\s*\(/, label: "insertAdjacentHTML" },
      { pattern: /createElement\s*\(\s*["']script["']\s*\)/, label: "dynamic script element" }
    ];
    for (const item of forbidden) {
      if (item.pattern.test(line)) {
        fail(`${relativePath}:${lineNumber} uses forbidden ${item.label}.`);
      }
    }
  });
}

function auditSyntax(filePath) {
  const result = spawnSync(process.execPath, ["--check", "--input-type=module"], {
    input: read(filePath),
    encoding: "utf8"
  });
  if (result.status !== 0) {
    fail(`${relative(filePath)} failed module syntax validation: ${result.stderr.trim() || result.stdout.trim()}`);
  }
}

function auditPublicFacade() {
  const facade = read(path.join(sceneRuntimeDir, "01-webgl-scene.js"));
  if (!facade.includes("root.webglScene")) {
    fail("01-webgl-scene.js no longer assigns window.CanDoItAll.webglScene.");
  }
}

function auditAssetIncludes() {
  const bodyAssets = read(path.join(componentRoot, "WebGlLibBodyAssets.razor"));
  if (!bodyAssets.includes("runtime/workbench/01-webgl-workbench.js")) {
    fail("WebGlLibBodyAssets.razor no longer includes the WebGL workbench runtime entry.");
  }

  if (!bodyAssets.includes("runtime/scene/01-webgl-scene.js")) {
    fail("WebGlLibBodyAssets.razor no longer includes the WebGL scene runtime entry.");
  }
}

function auditBranchInstructionFiles() {
  const instructionRoots = [
    path.join(repoRoot, "codex"),
    path.join(repoRoot, ".github")
  ];
  const branchCommandPattern = /\b(git\s+(checkout|switch)\s+-(b|c)|git\s+branch\s+(?!-)[\w/]+)/i;
  for (const root of instructionRoots) {
    for (const filePath of walk(root, [".md", ".yml", ".yaml"])) {
      const lines = read(filePath).split(/\r?\n/);
      lines.forEach((line, index) => {
        if (!branchCommandPattern.test(line)) {
          return;
        }

        if (/\b(do not|don't|never|forbidden|must not|not run)\b/i.test(line)) {
          return;
        }

        fail(`${relative(filePath)}:${index + 1} contains an unguarded branch-creation instruction.`);
      });
    }
  }
}

function hasStaticInnerHtmlAllowlist(lines, index) {
  const nearby = [
    lines[index - 1] || "",
    lines[index] || "",
    lines[index + 1] || ""
  ].join(" ");
  return /webgl-audit:\s*allow-innerHTML-static/i.test(nearby);
}

function walk(root, extensions) {
  if (!fs.existsSync(root)) {
    return [];
  }

  const files = [];
  for (const entry of fs.readdirSync(root, { withFileTypes: true })) {
    const fullPath = path.join(root, entry.name);
    if (entry.isDirectory()) {
      files.push(...walk(fullPath, extensions));
      continue;
    }

    if (extensions.includes(path.extname(entry.name).toLowerCase())) {
      files.push(fullPath);
    }
  }

  return files;
}

function read(filePath) {
  return fs.readFileSync(filePath, "utf8");
}

function relative(filePath) {
  return path.relative(repoRoot, filePath).replaceAll("\\", "/");
}

function fail(message) {
  failures += 1;
  console.error(`[FAIL] ${message}`);
}

function warn(message) {
  warnings += 1;
  console.log(`[WARN] ${message}`);
}
