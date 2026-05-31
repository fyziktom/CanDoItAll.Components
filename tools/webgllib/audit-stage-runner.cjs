const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "30-webgl-scene-stage-runner.js");
const reportDir = path.join(repoRoot, "artifacts", "webgl-economy-kernel-hardening-v12", "stage-runner");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtimeModulePath = writeAuditRuntimeModule();
  const runtime = await import(`${pathToFileURL(runtimeModulePath).href}?v=${Date.now()}`);
  const state = createState();
  const applied = [];

  runtime.enqueueCommandStages(state, "batch.sequence", [
    stage("move.target", 0.5),
    stage("change.pose", 0.25),
    stage("move.home", 0)
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["move.target"], "first stage applies immediately");
  assertEqual(state.diagnostics.currentCommandBatchId, "batch.sequence", "current batch after first stage");
  assertEqual(state.diagnostics.currentCommandStageId, "move.target", "current stage after first stage");
  assertEqual(state.diagnostics.completedCommandStageCount, 1, "completed count after first stage");
  assertEqual(state.diagnostics.queuedCommandStageCount, 2, "queued count after first stage");

  runtime.advanceCommandStageRunner(state, 0.49, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target"], "wait barrier holds before elapsed time");

  runtime.advanceCommandStageRunner(state, 0.02, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target", "change.pose"], "second stage applies after wait");
  assertEqual(state.diagnostics.queuedCommandStageCount, 1, "queued count after second stage");

  runtime.advanceCommandStageRunner(state, 0.25, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target", "change.pose", "move.home"], "third stage applies after second wait");
  assertEqual(state.diagnostics.completedCommandStageCount, 3, "all stages completed");
  assertEqual(state.diagnostics.queuedCommandStageCount, 0, "queue empty after completion");

  runtime.enqueueCommandStages(state, "batch.cancel", [stage("cancelled.stage", 1)], item => applied.push(item.stageId));
  runtime.cancelCommandStageRunner(state, "audit-cancel");
  assertEqual(state.diagnostics.commandStageCancelledCount, 1, "cancel count");
  assertEqual(state.diagnostics.lastStageCancelReason, "audit-cancel", "cancel reason");

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB02.stage-runner.wait-barrier",
    applied,
    diagnostics: {
      completedCommandStageCount: state.diagnostics.completedCommandStageCount,
      failedCommandStageCount: state.diagnostics.failedCommandStageCount,
      queuedCommandStageCount: state.diagnostics.queuedCommandStageCount,
      commandStageCancelledCount: state.diagnostics.commandStageCancelledCount,
      lastStageCancelReason: state.diagnostics.lastStageCancelReason
    }
  };
  fs.writeFileSync(path.join(reportDir, "stage-runner-proof.json"), `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log("Stage runner audit passed for ordered wait barriers and cancellation diagnostics.");
}

function writeAuditRuntimeModule() {
  const source = fs.readFileSync(runtimeSourcePath, "utf8");
  const modulePath = path.join(reportDir, "stage-runner-runtime-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function createState() {
  return {
    commandStageRunner: null,
    diagnostics: {},
    scheduleCount: 0,
    scheduleRender() {
      this.scheduleCount += 1;
    }
  };
}

function stage(stageId, waitSeconds) {
  return { stageId, waitSeconds, patches: [], motions: [] };
}

function assertEqual(actual, expected, label) {
  if (actual !== expected) {
    throw new Error(`${label}: expected ${expected}, actual ${actual}`);
  }
}

function assertDeepEqual(actual, expected, label) {
  const actualJson = JSON.stringify(actual);
  const expectedJson = JSON.stringify(expected);
  if (actualJson !== expectedJson) {
    throw new Error(`${label}: expected ${expectedJson}, actual ${actualJson}`);
  }
}

main().catch(error => {
  console.error(error?.stack || error);
  process.exit(1);
});

