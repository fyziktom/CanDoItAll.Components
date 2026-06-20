const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeStopSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "39-webgl-scene-runtime-stop.js");
const motionSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "14-webgl-scene-motion.js");
const motionQueueSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "29-webgl-scene-motion-queues.js");
const stageRunnerSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "30-webgl-scene-stage-runner.js");
const stageBarrierSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "32-webgl-scene-stage-barriers.js");
const journalSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "33-webgl-scene-command-journal.js");
const reportDir = path.join(repoRoot, "artifacts", "webgl-runtime-stop-hardening-v5");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtime = await import(`${pathToFileURL(writeAuditModule()).href}?v=${Date.now()}`);
  const assertions = [];

  assertStopClearsRuntimeActivity(runtime, assertions);
  assertSecondStopIsIdleAndSuccessful(runtime, assertions);
  assertCancelCommandStagesOnlyAffectsStages(runtime, assertions);

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB02.runtime-stop",
    assertions
  };
  fs.writeFileSync(path.join(reportDir, "runtime-stop-proof.json"), `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log("Runtime stop audit passed for stopRuntimeActivity, cancelCommandStages, idempotent idle stop, motion clearing, and stage cancellation.");
}

function assertStopClearsRuntimeActivity(runtime, assertions) {
  const state = createState();
  const first = runtime.stopRuntimeActivity(state, "pause");

  assertEqual(first.success, true, "runtime stop succeeds");
  assertEqual(first.commandKind, "runtime-stop", "runtime stop command kind");
  assertEqual(state.motions.size, 0, "runtime stop clears active motions");
  assertEqual(state.motionQueuesByObjectId.size, 0, "runtime stop clears queued motions");
  assertEqual(state.diagnostics.activeMotionCount, 0, "active motion diagnostics clear");
  assertEqual(state.diagnostics.queuedMotionCount, 0, "queued motion diagnostics clear");
  assertEqual(state.diagnostics.queuedCommandStageCount, 0, "queued stage diagnostics clear");
  assertEqual(state.diagnostics.commandStageBarrierPolicy, "", "active stage barrier clears");
  assertEqual(state.diagnostics.runtimeStopCount, 1, "runtime stop count increments");
  assertEqual(state.diagnostics.lastRuntimeStopReason, "pause", "runtime stop reason records");
  assertEqual(state.diagnostics.lastRuntimeStopClearedMotionCount, 3, "runtime stop reports active and queued motions");
  assertEqual(state.diagnostics.lastRuntimeStopCancelledCommandStageCount, 2, "runtime stop reports active barrier and queued stage");
  assertEqual(first.metadata.activeMotionCountBefore, "1", "metadata active motion count before stop");
  assertEqual(first.metadata.queuedMotionCountBefore, "2", "metadata queued motion count before stop");
  assertEqual(first.metadata.clearedMotionCount, "3", "metadata cleared motion count");
  assertEqual(first.metadata.cancelledCommandStageCount, "2", "metadata cancelled stage count");
  assertEqual(first.metadata.activeMotionCountAfter, "0", "metadata active motion count after stop");
  assertEqual(first.metadata.queuedCommandStageCountAfter, "0", "metadata stage count after stop");
  assertions.push("stopRuntimeActivity clears active motions, queued motions, active stage barrier, and queued command stages");
}

function assertSecondStopIsIdleAndSuccessful(runtime, assertions) {
  const state = createState();
  runtime.stopRuntimeActivity(state, "pause");
  const second = runtime.stopRuntimeActivity(state, "pause-again");

  assertEqual(second.success, true, "second runtime stop succeeds");
  assertEqual(state.motions.size, 0, "second stop leaves active motions empty");
  assertEqual(state.motionQueuesByObjectId.size, 0, "second stop leaves queued motions empty");
  assertEqual(state.diagnostics.queuedCommandStageCount, 0, "second stop leaves stage queue empty");
  assertEqual(state.diagnostics.runtimeStopCount, 2, "second stop increments stop count");
  assertEqual(state.diagnostics.lastRuntimeStopReason, "pause-again", "second stop updates reason");
  assertEqual(state.diagnostics.lastRuntimeStopClearedMotionCount, 0, "second stop reports no new motions cleared");
  assertEqual(state.diagnostics.lastRuntimeStopCancelledCommandStageCount, 0, "second stop reports no new stages cancelled");
  assertEqual(second.metadata.clearedMotionCount, "0", "second metadata has zero cleared motions");
  assertEqual(second.metadata.cancelledCommandStageCount, "0", "second metadata has zero cancelled stages");
  assertions.push("calling stopRuntimeActivity twice is idempotent for runtime state");
}

function assertCancelCommandStagesOnlyAffectsStages(runtime, assertions) {
  const state = createState();
  const result = runtime.cancelCommandStages(state, "stage-only");

  assertEqual(result.success, true, "cancelCommandStages succeeds");
  assertEqual(result.commandKind, "command-stage-cancel", "stage cancel command kind");
  assertEqual(state.motions.size, 1, "stage-only cancel leaves active motion alone");
  assertEqual(state.motionQueuesByObjectId.size, 1, "stage-only cancel leaves queued motions alone");
  assertEqual(state.diagnostics.queuedCommandStageCount, 0, "stage-only cancel clears stage queue");
  assertEqual(state.diagnostics.lastStageCancelReason, "stage-only", "stage-only cancel reason records");
  assertEqual(result.metadata.cancelledCommandStageCount, "2", "stage-only metadata reports stage work");
  assertions.push("cancelCommandStages clears command stages without clearing motion activity");
}

function writeAuditModule() {
  const source = [
    runtimeStubs(),
    stripModuleSyntax(motionQueueSourcePath),
    stripModuleSyntax(motionSourcePath),
    stripModuleSyntax(stageBarrierSourcePath),
    stripModuleSyntax(journalSourcePath),
    stripModuleSyntax(stageRunnerSourcePath),
    stripModuleSyntax(runtimeStopSourcePath),
    "export { stopRuntimeActivity, cancelCommandStages };"
  ].join("\n");
  const modulePath = path.join(reportDir, "runtime-stop-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function stripModuleSyntax(filePath) {
  let source = fs.readFileSync(filePath, "utf8")
    .replace(/import\s*\{[\s\S]*?\}\s*from\s*".*?";\s*/g, "")
    .replace(/^import .*?;\s*$/gm, "")
    .replace(/\bexport\s+function\s+/g, "function ");
  if (path.basename(filePath) === "32-webgl-scene-stage-barriers.js") {
    source = source
      .replace(/\broundWait\(/g, "roundWaitBarrier(")
      .replace(/\bfunction roundWaitBarrier\(/g, "function roundWaitBarrier(");
  }

  return source;
}

function runtimeStubs() {
  return `
function round(value, decimals = 2) {
  const multiplier = Math.pow(10, decimals);
  return Math.round((Number(value) || 0) * multiplier) / multiplier;
}
function resolveObjectPosition(sceneObject) {
  return sceneObject.position || { x: 0, y: 0, z: 0 };
}
function resolveObjectRotation(sceneObject) {
  return sceneObject.rotation || { x: 0, y: 0, z: 0 };
}
function resolveObjectScale(sceneObject) {
  return sceneObject.scale || { x: 1, y: 1, z: 1 };
}
function updateObjectRuntimeTransform() {
  return true;
}
function notifyStateChanged() {
}
function resolveSceneRevision(scene) {
  return Number(scene?.revision) || 0;
}
function createCommandResult(state, commandKind, commandId = "") {
  return {
    commandId: commandId || commandKind,
    success: true,
    succeeded: true,
    sceneId: state?.sceneModel?.sceneId || "",
    commandKind,
    revision: 0,
    errors: [],
    warnings: [],
    affectedObjectIds: [],
    affectedLinkIds: [],
    diagnostics: {},
    metadata: {}
  };
}
function completeCommandResult(state, result) {
  result.success = result.errors.length === 0;
  result.succeeded = result.success;
  result.diagnostics = {
    renderCount: String(state?.diagnostics?.renderCount || 0),
    activeMotionCount: String(state?.motions?.size || 0)
  };
  return result;
}
function failCommand(_state, result, message) {
  result.success = false;
  result.succeeded = false;
  result.errors.push(message);
  return result;
}
`;
}

function createState() {
  const state = {
    sceneModel: { sceneId: "runtime-stop-audit", revision: 1, objects: [], links: [] },
    objectLookup: new Map([
      ["actor", {
        position: { x: 0, y: 0, z: 0 },
        rotation: { x: 0, y: 0, z: 0 },
        scale: { x: 1, y: 1, z: 1 }
      }]
    ]),
    motions: new Map([
      ["motion.active", { motionId: "motion.active", objectId: "actor" }]
    ]),
    motionQueuesByObjectId: new Map([
      ["actor", [
        { motionId: "motion.queued.1", objectId: "actor" },
        { motionId: "motion.queued.2", objectId: "actor" }
      ]]
    ]),
    commandStageRunner: {
      queue: [{ batchId: "batch.stop", stage: { stageId: "stage.queued" } }],
      activeBarrier: { batchId: "batch.stop", stageId: "stage.active", policy: "wait-seconds", remainingSeconds: 1 },
      currentBatchId: "batch.stop",
      currentStageId: "stage.active",
      completedStageIds: ["stage.active"],
      failedStageIds: [],
      skippedStageIds: [],
      resultLog: [{ resultId: "stage.active:applied" }],
      resultSequence: 1,
      lastStageError: "",
      cancelled: false
    },
    commandStageJournal: null,
    diagnostics: {
      renderCount: 0,
      queuedMotionCount: 2,
      activeMotionCount: 1,
      activeMotionIds: ["motion.active"],
      queuedMotionIds: ["motion.queued.1", "motion.queued.2"],
      motionQueueSnapshot: [],
      maxMotionQueueLength: 2,
      cancelledMotionCount: 0,
      commandStageCancelledCount: 0,
      queuedCommandStageCount: 1,
      commandStageBarrierPolicy: "wait-seconds",
      commandStageJournalCounters: {
        started: 0,
        applied: 0,
        completed: 0,
        warnings: 0,
        failures: 0
      },
      commandStageRecentJournalEntries: [],
      commandStageRecentResultIds: [],
      lastStageCancelReason: ""
    },
    options: { deterministicMode: true, maxCommandStageJournalEntries: 20 },
    scheduledReasons: [],
    scheduleRender(reason) {
      this.scheduledReasons.push(reason);
    },
    dotNetRef: null
  };
  return state;
}

function assertEqual(actual, expected, label) {
  if (actual !== expected) {
    throw new Error(`${label}: expected ${expected}, actual ${actual}`);
  }
}

main().catch(error => {
  console.error(error?.stack || error);
  process.exit(1);
});
