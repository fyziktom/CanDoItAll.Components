# audit-scene-runtime.cjs skeleton

```js
const fs = require("fs");
const path = require("path");
const vm = require("vm");

const repoRoot = process.cwd();
const runtimeDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const warningLineCount = 220;
const hardLineCount = 320;
const allowInnerHtml = new Set([
  // Prefer empty. If a static template is temporarily allowed, document it here with a reason.
]);

let hasError = false;

for (const file of fs.readdirSync(runtimeDir).filter(name => name.endsWith(".js")).sort()) {
  const fullPath = path.join(runtimeDir, file);
  const text = fs.readFileSync(fullPath, "utf8");
  const lines = text.split(/\r?\n/).length;

  if (lines > hardLineCount) {
    console.error(`[FAIL] ${file} has ${lines} lines; hard limit is ${hardLineCount}.`);
    hasError = true;
  } else if (lines > warningLineCount) {
    console.warn(`[WARN] ${file} has ${lines} lines; warning threshold is ${warningLineCount}.`);
  } else {
    console.log(`[OK] ${file} has ${lines} lines.`);
  }

  if (/\.innerHTML\s*=/.test(text) && !allowInnerHtml.has(file)) {
    console.error(`[FAIL] ${file} uses innerHTML without allowlist.`);
    hasError = true;
  }

  for (const forbidden of ["eval(", "new Function("]) {
    if (text.includes(forbidden)) {
      console.error(`[FAIL] ${file} contains forbidden pattern: ${forbidden}`);
      hasError = true;
    }
  }

  try {
    new vm.SourceTextModule(text, { identifier: file });
  } catch (error) {
    console.error(`[FAIL] ${file} failed module parse: ${error.message}`);
    hasError = true;
  }
}

process.exit(hasError ? 1 : 0);
```
