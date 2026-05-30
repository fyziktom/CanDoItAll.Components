const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "26-webgl-scene-command-batch.js");
const fixtureDir = path.join(__dirname, "command-batch-fixtures");
const reportDir = path.join(repoRoot, "artifacts", "webgl-economy-sharedwell-hardening-v8", "command-batch-parity");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtimeModulePath = writeAuditRuntimeModule();
  const runtime = await import(`${pathToFileURL(runtimeModulePath).href}?v=${Date.now()}`);
  const fixtures = fs.readdirSync(fixtureDir)
    .filter(file => file.endsWith(".json"))
    .sort((left, right) => left.localeCompare(right));

  const rows = [];
  for (const fixtureFile of fixtures) {
    const fixture = JSON.parse(fs.readFileSync(path.join(fixtureDir, fixtureFile), "utf8"));
    const normalized = runtime.normalizeCommandBatchForAudit(fixture.input);
    const actual = summarize(normalized);
    const expected = fixture.expected;
    assertEqual(actual, expected, fixture.name);
    rows.push({ name: fixture.name, actual });
  }

  fs.writeFileSync(path.join(reportDir, "js-command-batch-parity-summary.json"), JSON.stringify(rows, null, 2), "utf8");
  console.log(`Command batch parity audit passed for ${rows.length} fixture(s).`);
}

function writeAuditRuntimeModule() {
  const source = fs.readFileSync(runtimeSourcePath, "utf8")
    .split(/\r?\n/)
    .filter(line => !line.startsWith("import "))
    .join("\n");
  const modulePath = path.join(reportDir, "command-batch-runtime-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function summarize(normalized) {
  return {
    batchCommandCount: normalized.metrics.batchCommandCount,
    stageCount: normalized.metrics.stageCount,
    patchCount: normalized.patches.length,
    motionCount: normalized.motions.length,
    coalescedPatchCount: normalized.metrics.coalescedPatchCount,
    droppedDuplicateMotionCount: normalized.metrics.droppedDuplicateMotionCount,
    stageSummaries: normalized.stages.map(stage => ({
      stageId: stage.stageId,
      patchCount: stage.patches.length,
      motionCount: stage.motions.length
    }))
  };
}

function assertEqual(actual, expected, name) {
  const actualJson = JSON.stringify(actual);
  const expectedJson = JSON.stringify(expected);
  if (actualJson !== expectedJson) {
    throw new Error(`Fixture '${name}' mismatch.\nExpected: ${expectedJson}\nActual:   ${actualJson}`);
  }
}

main().catch(error => {
  console.error(error?.stack || error);
  process.exit(1);
});

