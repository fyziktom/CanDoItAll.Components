const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const webGlLibRoot = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib");
const webGlWwwRoot = path.join(webGlLibRoot, "wwwroot");
const webGlJsRoot = path.join(webGlLibRoot, "wwwroot", "js");
const sceneRuntimeDir = path.join(webGlJsRoot, "runtime", "scene");
const vendorDirs = [
  path.join(webGlWwwRoot, "vendor"),
  path.join(webGlJsRoot, "vendor")
];
const reportDir = path.join(repoRoot, "artifacts", "webgl-engine-economy-hardening-v1", "sb02-import-audit");

if (process.argv.includes("--self-test")) {
  runSelfTest();
} else {
  main();
}

function main() {
  const modules = loadSceneModules(sceneRuntimeDir);
  const failures = [];
  const warnings = [];

  auditImportTargets(modules, failures);
  auditSceneExportReferences(modules, failures);
  auditGlobalRegistration(modules, failures);
  auditForbiddenDependencies(modules, failures);

  const report = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB02.imports",
    sceneRuntimeDir: relative(sceneRuntimeDir),
    moduleCount: modules.size,
    failures,
    warnings,
    modules: Array.from(modules.values()).map(module => ({
      path: relative(module.filePath),
      imports: module.imports.map(item => ({
        imported: item.imported,
        local: item.local,
        specifier: item.specifier
      })),
      exports: Array.from(module.exports).sort()
    }))
  };

  fs.mkdirSync(reportDir, { recursive: true });
  fs.writeFileSync(path.join(reportDir, "scene-runtime-import-audit.json"), `${JSON.stringify(report, null, 2)}\n`, "utf8");

  if (failures.length > 0) {
    for (const failure of failures) {
      console.error(`[FAIL] ${failure}`);
    }

    console.error(`Scene runtime import/export audit failed with ${failures.length} failure(s).`);
    process.exit(1);
  }

  console.log(`Scene runtime import/export audit passed for ${modules.size} module(s).`);
}

function runSelfTest() {
  const fixtureRoot = path.join(repoRoot, "__fixture__");
  const fixtureModules = new Map([
    [path.join(fixtureRoot, "a.js"), analyzeModule(path.join(fixtureRoot, "a.js"), "export function present() {}\n")],
    [path.join(fixtureRoot, "b.js"), analyzeModule(path.join(fixtureRoot, "b.js"), "import { missing } from \"./a.js\";\npresent();\n")]
  ]);
  const failures = [];
  auditImportTargets(fixtureModules, failures);
  auditSceneExportReferences(fixtureModules, failures);

  const caughtMissingImport = failures.some(item => item.includes("imports 'missing'"));
  const caughtUnimportedReference = failures.some(item => item.includes("uses scene symbol 'present'"));
  if (!caughtMissingImport || !caughtUnimportedReference) {
    console.error("Self-test failed. The audit did not catch both a missing exported import and an unimported scene symbol.");
    for (const failure of failures) {
      console.error(`[SELF-TEST] ${failure}`);
    }

    process.exit(1);
  }

  console.log("Self-test passed: missing exported imports and unimported scene symbols are rejected.");
}

function loadSceneModules(root) {
  return new Map(fs.readdirSync(root)
    .filter(file => file.endsWith(".js"))
    .sort((left, right) => left.localeCompare(right))
    .map(file => {
      const filePath = path.join(root, file);
      return [filePath, analyzeModule(filePath, fs.readFileSync(filePath, "utf8"))];
    }));
}

function analyzeModule(filePath, source) {
  const imports = parseImports(filePath, source);
  const exports = parseExports(source);
  const strippedSource = stripImportStatements(stripCommentsAndStrings(source));
  const localNames = collectLocalNames(imports, exports, strippedSource);
  return {
    filePath,
    source,
    strippedSource,
    imports,
    exports,
    localNames
  };
}

function parseImports(filePath, source) {
  const imports = [];
  const importPattern = /import\s+(?:(?<namespace>\*\s+as\s+[A-Za-z_$][\w$]*)|(?<defaultName>[A-Za-z_$][\w$]*)\s*,?\s*)?(?:\{(?<named>[\s\S]*?)\})?\s*from\s*["'](?<specifier>[^"']+)["'];?/g;
  for (const match of source.matchAll(importPattern)) {
    const specifier = match.groups.specifier;
    const resolvedPath = resolveSpecifier(filePath, specifier);
    const namespace = match.groups.namespace;
    if (namespace) {
      const local = namespace.replace("*", "").replace("as", "").trim();
      imports.push({ imported: "*", local, specifier, resolvedPath, namespace: true });
    }

    if (match.groups.defaultName) {
      imports.push({ imported: "default", local: match.groups.defaultName.trim(), specifier, resolvedPath, defaultImport: true });
    }

    const named = match.groups.named || "";
    for (const part of named.split(",")) {
      const item = part.trim();
      if (!item) {
        continue;
      }

      const [importedName, localName] = item.split(/\s+as\s+/);
      imports.push({
        imported: importedName.trim(),
        local: (localName || importedName).trim(),
        specifier,
        resolvedPath,
        namespace: false
      });
    }
  }

  return imports;
}

function parseExports(source) {
  const exports = new Set();
  for (const match of source.matchAll(/\bexport\s+(?:async\s+)?function\s+([A-Za-z_$][\w$]*)/g)) {
    exports.add(match[1]);
  }

  for (const match of source.matchAll(/\bexport\s+class\s+([A-Za-z_$][\w$]*)/g)) {
    exports.add(match[1]);
  }

  for (const match of source.matchAll(/\bexport\s+(?:const|let|var)\s+([A-Za-z_$][\w$]*)/g)) {
    exports.add(match[1]);
  }

  for (const match of source.matchAll(/\bexport\s*\{([\s\S]*?)\}\s*;?/g)) {
    for (const part of match[1].split(",")) {
      const item = part.trim();
      if (!item) {
        continue;
      }

      const [name, alias] = item.split(/\s+as\s+/);
      exports.add((alias || name).trim());
    }
  }

  return exports;
}

function collectLocalNames(imports, exports, strippedSource) {
  const names = new Set(exports);
  for (const item of imports) {
    names.add(item.local);
  }

  for (const match of strippedSource.matchAll(/\b(?:function|class)\s+([A-Za-z_$][\w$]*)/g)) {
    names.add(match[1]);
  }

  for (const match of strippedSource.matchAll(/\b(?:const|let|var)\s+([^;\n]+)/g)) {
    for (const part of match[1].split(",")) {
      const name = part.split("=")[0].trim();
      if (/^[A-Za-z_$][\w$]*$/.test(name)) {
        names.add(name);
      }
    }
  }

  for (const match of strippedSource.matchAll(/\bcatch\s*\(\s*([A-Za-z_$][\w$]*)\s*\)/g)) {
    names.add(match[1]);
  }

  return names;
}

function auditImportTargets(modules, failures) {
  for (const module of modules.values()) {
    const importsByTarget = groupBy(module.imports, item => item.resolvedPath || item.specifier);
    for (const [target, imports] of importsByTarget) {
      if (!target || !path.isAbsolute(target)) {
        failures.push(`${relative(module.filePath)} imports non-relative module '${imports[0].specifier}'. Scene runtime modules must use explicit local/vendor imports.`);
        continue;
      }

      if (!fs.existsSync(target) && !modules.has(target)) {
        failures.push(`${relative(module.filePath)} imports missing module '${imports[0].specifier}'.`);
        continue;
      }

      if (!modules.has(target)) {
        continue;
      }

      const targetExports = modules.get(target).exports;
      for (const item of imports) {
        if (item.namespace || item.defaultImport) {
          continue;
        }

        if (!targetExports.has(item.imported)) {
          failures.push(`${relative(module.filePath)} imports '${item.imported}' from ${relative(target)}, but that module does not export it.`);
        }
      }
    }
  }
}

function auditSceneExportReferences(modules, failures) {
  const exportOwners = new Map();
  for (const module of modules.values()) {
    for (const name of module.exports) {
      if (!exportOwners.has(name)) {
        exportOwners.set(name, []);
      }

      exportOwners.get(name).push(module.filePath);
    }
  }

  for (const module of modules.values()) {
    const importedNames = new Set(module.imports.map(item => item.local));
    for (const [name, owners] of exportOwners) {
      if (owners.includes(module.filePath) || importedNames.has(name) || module.localNames.has(name)) {
        continue;
      }

      const index = findIdentifierReference(module.strippedSource, name);
      if (index >= 0) {
        failures.push(`${relative(module.filePath)} uses scene symbol '${name}' at line ${lineAt(module.strippedSource, index)} without importing it from ${owners.map(relative).join(", ")}.`);
      }
    }
  }
}

function auditGlobalRegistration(modules, failures) {
  const registrations = [];
  for (const module of modules.values()) {
    const matches = Array.from(module.source.matchAll(/\broot\.webglScene\s*=/g));
    for (const match of matches) {
      registrations.push(`${relative(module.filePath)}:${lineAt(module.source, match.index)}`);
    }
  }

  if (registrations.length !== 1 || !registrations[0].startsWith("src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js:")) {
    failures.push(`Expected exactly one root.webglScene registration in 01-webgl-scene.js, found ${registrations.length}: ${registrations.join(", ") || "(none)"}.`);
  }
}

function auditForbiddenDependencies(modules, failures) {
  for (const module of modules.values()) {
    for (const item of module.imports) {
      if (/webglrunlib|economy|ledger|market|production-line|production line/i.test(item.specifier)) {
        failures.push(`${relative(module.filePath)} imports forbidden domain/run-layer specifier '${item.specifier}'.`);
        continue;
      }

      if (!item.resolvedPath || !path.isAbsolute(item.resolvedPath)) {
        continue;
      }

      const inSceneRuntime = isWithin(item.resolvedPath, sceneRuntimeDir);
      const inVendor = vendorDirs.some(vendorDir => isWithin(item.resolvedPath, vendorDir));
      if (!inSceneRuntime && !inVendor) {
        failures.push(`${relative(module.filePath)} imports ${relative(item.resolvedPath)}, outside scene runtime/vendor boundaries.`);
      }
    }
  }
}

function resolveSpecifier(filePath, specifier) {
  if (!specifier.startsWith(".")) {
    return specifier;
  }

  return path.resolve(path.dirname(filePath), specifier);
}

function stripCommentsAndStrings(source) {
  return source.replace(/\/\*[\s\S]*?\*\/|\/\/.*|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|`(?:\\[\s\S]|[^`\\])*`/g, match => " ".repeat(match.length));
}

function stripImportStatements(source) {
  return source.replace(/^\s*import\s+[\s\S]*?;\s*$/gm, match => " ".repeat(match.length));
}

function findIdentifierReference(source, name) {
  const pattern = new RegExp(`\\b${escapeRegExp(name)}\\b`, "g");
  for (const match of source.matchAll(pattern)) {
    const previous = previousNonWhitespace(source, match.index);
    if (previous === "." || previous === "#") {
      continue;
    }

    if (isObjectMethodKey(source, match.index, name.length)) {
      continue;
    }

    return match.index;
  }

  return -1;
}

function isObjectMethodKey(source, index, length) {
  const previous = previousNonWhitespace(source, index);
  if (previous !== "," && previous !== "{") {
    return false;
  }

  let cursor = nextNonWhitespaceIndex(source, index + length);
  if (source[cursor] !== "(") {
    return false;
  }

  let depth = 0;
  for (; cursor < source.length; cursor += 1) {
    if (source[cursor] === "(") {
      depth += 1;
      continue;
    }

    if (source[cursor] !== ")") {
      continue;
    }

    depth -= 1;
    if (depth === 0) {
      const next = nextNonWhitespaceIndex(source, cursor + 1);
      return source[next] === "{";
    }
  }

  return false;
}

function nextNonWhitespaceIndex(source, index) {
  for (let i = index; i < source.length; i += 1) {
    if (!/\s/.test(source[i])) {
      return i;
    }
  }

  return source.length;
}

function previousNonWhitespace(source, index) {
  for (let i = index - 1; i >= 0; i -= 1) {
    if (!/\s/.test(source[i])) {
      return source[i];
    }
  }

  return "";
}

function groupBy(items, keySelector) {
  const groups = new Map();
  for (const item of items) {
    const key = keySelector(item);
    if (!groups.has(key)) {
      groups.set(key, []);
    }

    groups.get(key).push(item);
  }

  return groups;
}

function isWithin(candidate, parent) {
  const relativePath = path.relative(parent, candidate);
  return relativePath === "" || (!relativePath.startsWith("..") && !path.isAbsolute(relativePath));
}

function lineAt(source, index) {
  return source.slice(0, index).split(/\r?\n/).length;
}

function relative(filePath) {
  return path.relative(repoRoot, filePath).replaceAll("\\", "/");
}

function escapeRegExp(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}
