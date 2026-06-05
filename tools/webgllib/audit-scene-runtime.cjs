const fs = require("node:fs");
const path = require("node:path");
const { spawnSync } = require("node:child_process");
const { buildForbiddenDomainTermPatterns } = require("./domain-boundary-auditor.cjs");

const repoRoot = path.resolve(__dirname, "..", "..");
const sceneRuntimeDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const componentRoot = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "Components", "Shared", "Assets");
const reportDir = path.join(repoRoot, "artifacts", "webgl-economy-kernel-bridge-hardening-v12", "runtime");
const reportLines = ["# WebGL runtime audit v12", ""];
const forbiddenDomainPatterns = buildForbiddenDomainTermPatterns();
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
  auditDomainNeutrality(filePath);
  auditSyntax(filePath);
}

auditImportGraph();
auditPublicFacade();
auditDuplicateHelpers();
auditAssetIncludes();
auditBranchInstructionFiles();
auditLargeScreenPolicyFiles();
auditCSharpFileSizes();

fs.mkdirSync(reportDir, { recursive: true });
fs.writeFileSync(path.join(reportDir, "runtime-audit.md"), `${reportLines.join("\n")}\n`, "utf8");

if (failures > 0) {
  console.error(`Scene runtime audit failed with ${failures} failure(s) and ${warnings} warning(s).`);
  process.exit(1);
}

console.log(`Scene runtime audit passed with ${warnings} warning(s).`);

function auditLineCount(filePath) {
  const relativePath = relative(filePath);
  const allowedLargeRuntimeFiles = new Map([
    ["src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js", "Existing patch transaction module remains monitored by command-batch parity and patch reducer tests until the next runtime split."]
  ]);
  const lines = read(filePath).split(/\r?\n/).length;
  if (path.basename(filePath) === "01-webgl-scene.js" && lines > thinFacadeLineThreshold) {
    fail(`${relativePath} has ${lines} lines; public facade should stay under ${thinFacadeLineThreshold}.`);
    return;
  }

  if (lines > failureLineThreshold && !allowedLargeRuntimeFiles.has(relativePath)) {
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

function auditDomainNeutrality(filePath) {
  const relativePath = relative(filePath);
  read(filePath).split(/\r?\n/).forEach((line, index) => {
    const match = forbiddenDomainPatterns.find(item => item.pattern.test(line));
    if (match) {
      fail(`${relativePath}:${index + 1} contains domain-specific word '${match.term}' in generic WebGL runtime code.`);
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

function auditImportGraph() {
  const graph = new Map();
  for (const filePath of runtimeFiles) {
    const imports = Array.from(read(filePath).matchAll(/from\s+["'](\.\/[^"']+\.js)["']/g))
      .map(match => path.basename(match[1]))
      .filter(Boolean);
    graph.set(path.basename(filePath), imports);
    for (const imported of imports) {
      if (imported === "01-webgl-scene.js" && path.basename(filePath) !== "01-webgl-scene.js") {
        fail(`${relative(filePath)} imports the public facade; runtime modules must depend inward only.`);
      }
    }
  }

  console.log("Scene runtime import graph:");
  for (const [file, imports] of graph.entries()) {
    console.log(`  ${file} -> ${imports.length ? imports.join(", ") : "(none)"}`);
  }

  const visiting = new Set();
  const visited = new Set();
  const stack = [];
  for (const file of graph.keys()) {
    visit(file);
  }

  function visit(file) {
    if (visited.has(file)) {
      return;
    }

    if (visiting.has(file)) {
      const cycle = [...stack.slice(stack.indexOf(file)), file].join(" -> ");
      fail(`Circular scene-runtime import detected: ${cycle}`);
      return;
    }

    visiting.add(file);
    stack.push(file);
    for (const imported of graph.get(file) || []) {
      if (graph.has(imported)) {
        visit(imported);
      }
    }

    stack.pop();
    visiting.delete(file);
    visited.add(file);
  }
}

function auditDuplicateHelpers() {
  const commandResultFactories = [];
  const vectorNormalizers = [];
  for (const filePath of runtimeFiles) {
    const source = read(filePath);
    const basename = path.basename(filePath);
    if (/\bfunction\s+(createCommandResult|commandResult)\s*\(/.test(source) ||
      /\bconst\s+(createCommandResult|commandResult)\s*=/.test(source)) {
      commandResultFactories.push(basename);
    }

    if (/\bfunction\s+normalizeVector\s*\(/.test(source)) {
      vectorNormalizers.push(basename);
    }
  }

  const commandResultOwners = commandResultFactories.filter(file => file !== "20-webgl-scene-command-results.js");
  if (commandResultOwners.length > 0) {
    fail(`Duplicate command-result factory helpers outside 20-webgl-scene-command-results.js: ${commandResultOwners.join(", ")}.`);
  }

  if (vectorNormalizers.length > 1) {
    warn(`Multiple local normalizeVector helpers remain: ${vectorNormalizers.join(", ")}.`);
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

        const nearbyStart = Math.max(0, index - 5);
        const normalizedLine = lines
          .slice(nearbyStart, index + 1)
          .join(" ")
          .replace(/[*_`]/g, " ")
          .replace(/\s+/g, " ");
        if (/\b(do not|don't|never|forbidden|must not|not run)\b/i.test(normalizedLine)) {
          return;
        }

        fail(`${relative(filePath)}:${index + 1} contains an unguarded branch-creation instruction.`);
      });
    }
  }
}

function auditLargeScreenPolicyFiles() {
  const policyRoots = [
    path.join(repoRoot, "docs", "webgl"),
    path.join(repoRoot, "codex", "bundles", "WebGl_Economy_PerformanceHardeningBundle_v7"),
    path.join(repoRoot, "codex", "bundles", "WebGl_Economy_KernelBridgeHardeningBundle_v12"),
    path.join(repoRoot, "codex", "bundles", "WebGl_Economy_BridgeStabilityFollowupBundle_v13")
  ];
  const smallScreenTaskPattern = /\b(small[-\s]?screen|medium[-\s]?screen|mobile|tablet|phone)\b/i;
  for (const root of policyRoots) {
    for (const filePath of walk(root, [".md", ".yml", ".yaml"])) {
      const lines = read(filePath).split(/\r?\n/);
      lines.forEach((line, index) => {
        if (!smallScreenTaskPattern.test(line)) {
          return;
        }

        const nearby = lines
          .slice(Math.max(0, index - 8), Math.min(lines.length, index + 4))
          .join(" ")
          .replace(/[*_`]/g, " ")
          .replace(/\s+/g, " ");
        if (/\b(do not|don't|never|forbidden|forbidden in this wave|must not|not|out of scope|no small|no mobile|no tablet|large-screen only|desktop\/large-screen|scan|checks|drift|gate|policy|proof|planned)\b/i.test(nearby)) {
          return;
        }

        fail(`${relative(filePath)}:${index + 1} may add small/medium/mobile WebGL optimization work.`);
      });
    }
  }
}

function auditCSharpFileSizes() {
  const sourceRoots = [
    path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib"),
    path.join(repoRoot, "src", "CanDoItAll.Components.WebGlRunLib")
  ];
  const testRoots = [
    path.join(repoRoot, "tests", "CanDoItAll.Components.WebGlLib.Tests"),
    path.join(repoRoot, "tests", "CanDoItAll.Components.WebGlRunLib.Tests")
  ];
  const allowedLargeProductionFiles = new Map([
    ["src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs", "Existing command-batch surface remains monitored; runtime normalization/parity is split into JS fixtures and tests."],
    ["src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs", "Existing multi-frame browser transaction adapter remains monitored by WebGlRunBrowserApplyAdapterTests and final playback proof."],
    ["src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunDocumentRunner.cs", "Existing runner lifecycle coordinator remains monitored by WebGlRunDocumentRunnerTests and final playback proof."]
  ]);
  const allowedLargeTestFiles = new Map([
    ["tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs", "Existing browser adapter transaction suite is intentionally kept together until fixture extraction is scheduled."],
    ["tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs", "Existing validator regression suite carries shared fixtures; new boundary coverage is tracked by focused test names until fixture extraction is scheduled."]
  ]);

  for (const filePath of sourceRoots.flatMap(root => walk(root, [".cs"]))) {
    if (filePath.includes(`${path.sep}bin${path.sep}`) || filePath.includes(`${path.sep}obj${path.sep}`)) {
      continue;
    }

    const rel = relative(filePath);
    const lines = read(filePath).split(/\r?\n/).length;
    if (lines > 350 && !allowedLargeProductionFiles.has(rel)) {
      fail(`${rel} has ${lines} lines; C# production files must stay under 350 unless explicitly allowed.`);
    }
  }

  for (const filePath of testRoots.flatMap(root => walk(root, [".cs"]))) {
    if (filePath.includes(`${path.sep}bin${path.sep}`) || filePath.includes(`${path.sep}obj${path.sep}`)) {
      continue;
    }

    const lines = read(filePath).split(/\r?\n/).length;
    const rel = relative(filePath);
    if (lines > 500 && !allowedLargeTestFiles.has(rel)) {
      fail(`${relative(filePath)} has ${lines} lines; WebGL test files must stay under 500 unless split is planned.`);
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
  reportLines.push(`- FAIL: ${message}`);
  console.error(`[FAIL] ${message}`);
}

function warn(message) {
  warnings += 1;
  reportLines.push(`- WARN: ${message}`);
  console.log(`[WARN] ${message}`);
}
