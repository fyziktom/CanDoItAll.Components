const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSceneDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const reportDir = path.join(repoRoot, "artifacts", "webgl-runtime-motion-queue-hardening-v15", "motion-queue");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtime = await import(`${pathToFileURL(writeAuditModule()).href}?v=${Date.now()}`);
  const assertions = [];

  assertOrderedSequence(runtime, assertions);
  assertParallelObjects(runtime, assertions);
  assertQueuePolicies(runtime, assertions);
  assertCancellationAndClear(runtime, assertions);
  assertDeterministicMotionIds(runtime, assertions);
  assertEdgeCases(runtime, assertions);

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB04.motion-queue.policies",
    assertions
  };
  fs.writeFileSync(path.join(reportDir, "motion-queue-proof.json"), `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log("Motion queue audit passed for ordered sequence, parallel objects, queue policies, cancellation, deterministic ids, diagnostics, and edge cases.");
}

function assertOrderedSequence(runtime, assertions) {
  const state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.to.b", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.to.c", 8, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.home", 0, "append"));
  assertEqual(state.motions.size, 1, "one active motion after append sequence enqueue");
  assertEqual(state.motionQueuesByObjectId.get("actor").length, 2, "two queued motions after append sequence enqueue");
  assertDeepEqual(state.diagnostics.queuedMotionIds, ["actor.to.c", "actor.home"], "queued motion ids are exposed");

  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 4, "first motion completes at B");
  assertEqual([...state.motions.keys()][0], "actor.to.c", "second motion activates after first completion");
  assertEqual(state.motions.get("actor.to.c").startPosition.x, 4, "second motion starts from B");

  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 8, "second motion completes at C");
  assertEqual([...state.motions.keys()][0], "actor.home", "home motion activates after second completion");
  assertEqual(state.motions.get("actor.home").startPosition.x, 8, "home motion starts from C");

  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 0, "home motion completes at origin");
  assertEqual(state.motions.size, 0, "no active motion after A-B-C-home sequence");
  assertions.push("append queue moves A -> B -> C -> home with recalculated starts");
}

function assertQueuePolicies(runtime, assertions) {
  let state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.replace", 12, "replace"));
  assertEqual([...state.motions.keys()][0], "actor.active", "replace keeps active motion");
  assertDeepEqual(state.diagnostics.queuedMotionIds, ["actor.replace"], "replace swaps queued motions");
  runtime.advanceMotions(state, 1);
  assertEqual([...state.motions.keys()][0], "actor.replace", "replacement queued motion promotes after active completion");
  assertEqual(state.motions.get("actor.replace").startPosition.x, 4, "replacement starts from active end");
  assertions.push("replace policy swaps queued motions while preserving active motion");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.cancel.replace", 2, "cancel-and-replace"));
  assertDeepEqual([...state.motions.keys()], ["actor.cancel.replace"], "cancel-and-replace installs new active motion");
  assertEqual(state.motionQueuesByObjectId.has("actor"), false, "cancel-and-replace clears queued motions");
  assertEqual(state.diagnostics.cancelledMotionCount, 2, "cancel-and-replace counts cancelled active and queued motions");
  assertions.push("cancel-and-replace clears active and queued motions before activating replacement");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  const rejected = runtime.enqueueMotionDetailed(state, motion("actor.rejected", 8, "reject-if-active"));
  assertEqual(rejected.success, false, "reject-if-active fails while object has active motion");
  assertDeepEqual([...state.motions.keys()], ["actor.active"], "reject-if-active preserves active motion");
  assertions.push("reject-if-active refuses to change active or queued object motion state");
}

function assertParallelObjects(runtime, assertions) {
  const state = createState();
  state.objectLookup.set("other", {
    position: { x: 10, y: 0, z: 0 },
    rotation: { x: 0, y: 0, z: 0 },
    scale: { x: 1, y: 1, z: 1 }
  });

  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append", "actor"));
  runtime.enqueueMotionDetailed(state, motion("other.active", 14, "append", "other"));
  assertDeepEqual([...state.motions.keys()].sort(), ["actor.active", "other.active"], "different objects run active motions in parallel");

  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 4, "actor parallel motion completes");
  assertEqual(state.objectLookup.get("other").position.x, 14, "other parallel motion completes");
  assertions.push("different objects move in parallel without queueing each other");
}

function assertCancellationAndClear(runtime, assertions) {
  let state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  const cancelQueued = runtime.cancelMotionDetailed(state, "actor.queued");
  assertEqual(cancelQueued.success, true, "queued motion cancellation succeeds");
  assertDeepEqual([...state.motions.keys()], ["actor.active"], "queued motion cancellation preserves active motion");
  assertEqual(state.motionQueuesByObjectId.has("actor"), false, "queued motion cancellation removes queue");
  assertions.push("cancelling a queued motion does not cancel active motion");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  const clearResult = { affectedObjectIds: [] };
  runtime.clearObjectMotionState(state, "actor", clearResult);
  assertEqual(state.motions.size, 0, "clear object cancels active motion");
  assertEqual(state.motionQueuesByObjectId.has("actor"), false, "clear object cancels queued motions");
  assertEqual(state.diagnostics.cancelledMotionCount, 2, "clear object counts active and queued cancellations");
  assertions.push("clear object cancels active and queued motions");

  state = createState();
  state.objectLookup.set("other", {
    position: { x: 10, y: 0, z: 0 },
    rotation: { x: 0, y: 0, z: 0 },
    scale: { x: 1, y: 1, z: 1 }
  });
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append", "actor"));
  runtime.enqueueMotionDetailed(state, motion("other.active", 14, "append", "other"));
  runtime.enqueueMotionDetailed(state, motion("other.queued", 18, "append", "other"));
  const clearAll = runtime.clearMotionsDetailed(state);
  assertDeepEqual(clearAll.affectedObjectIds.sort(), ["actor", "other"], "clear-all reports active and queued object ids");
  assertEqual(state.diagnostics.cancelledMotionCount, 3, "clear-all counts active and queued motions");
  assertions.push("clear-all reports affected objects from active and queued motions");
}

function assertDeterministicMotionIds(runtime, assertions) {
  const first = createState();
  const second = createState();
  const firstResult = runtime.enqueueMotionDetailed(first, motion("", 4, "append"));
  const secondResult = runtime.enqueueMotionDetailed(second, motion("", 4, "append"));
  assertEqual(firstResult.commandId, "actor:motion:1", "first deterministic motion id");
  assertEqual(secondResult.commandId, "actor:motion:1", "deterministic motion id repeats for equivalent state");
  assertions.push("deterministic mode generates stable motion ids for equivalent runs");
}

function assertEdgeCases(runtime, assertions) {
  let state = createState();
  runtime.enqueueMotionDetailed(state, {
    motionId: "actor.zero",
    objectId: "actor",
    durationSeconds: 0,
    queuePolicy: "cancel-and-replace",
    targetPosition: { x: 0, y: 0, z: 0 }
  });
  runtime.advanceMotions(state, 1);
  assertEqual(state.motions.size, 0, "zero-duration motion completes without hanging");
  assertions.push("zero-duration motion normalizes to finite duration and completes");

  state = createState();
  const missing = runtime.enqueueMotionDetailed(state, motion("missing.object", 1, "append", "missing"));
  assertEqual(missing.success, false, "missing object motion fails");
  assertEqual(state.diagnostics.motionFailedCount, 1, "missing object increments failed motion diagnostics");
  assertions.push("missing-object motion fails with diagnostics instead of queueing");
}

function writeAuditModule() {
  const queueSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "29-webgl-scene-motion-queues.js"), "utf8"));
  const cancellationSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "31-webgl-scene-motion-cancellation.js"), "utf8"));
  const motionSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "14-webgl-scene-motion.js"), "utf8"));
  const modulePath = path.join(reportDir, "motion-queue-runtime-audit.mjs");
  fs.writeFileSync(
    modulePath,
    `${runtimeStubs()}\n${queueSource}\n${cancellationSource}\n${motionSource}\nexport { enqueueMotionDetailed, cancelMotionDetailed, clearMotionsDetailed, clearObjectMotionState, advanceMotions };\n`,
    "utf8");
  return modulePath;
}

function stripModuleSyntax(source) {
  return source
    .replace(/import\s*\{[\s\S]*?\}\s*from\s*".*?";\s*/g, "")
    .replace(/^import .*?;\s*$/gm, "")
    .replace(/\bexport\s+function\s+/g, "function ");
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
function createCommandResult(state, commandKind, commandId) {
  return { success: true, commandKind, commandId, affectedObjectIds: [], affectedLinkIds: [], errors: [], warnings: [], metadata: {} };
}
function completeCommandResult(_state, result) {
  result.success = result.errors.length === 0;
  return result;
}
function failCommand(_state, result, message) {
  result.success = false;
  result.errors.push(message);
  return result;
}
function notifyStateChanged() {
}
`;
}

function createState() {
  return {
    objectLookup: new Map([[
      "actor",
      {
        position: { x: 0, y: 0, z: 0 },
        rotation: { x: 0, y: 0, z: 0 },
        scale: { x: 1, y: 1, z: 1 }
      }
    ]]),
    motions: new Map(),
    motionQueuesByObjectId: new Map(),
    diagnostics: {
      motionAcceptedCount: 0,
      motionCompletedCount: 0,
      motionFailedCount: 0,
      queuedMotionCount: 0,
      queuedMotionIds: [],
      motionQueueSnapshot: [],
      maxMotionQueueLength: 0,
      cancelledMotionCount: 0
    },
    options: { deterministicMode: true },
    scheduleRender() {},
    dotNetRef: null
  };
}

function motion(motionId, x, queuePolicy, objectId = "actor") {
  return {
    motionId,
    objectId,
    durationSeconds: 1,
    queuePolicy,
    targetPosition: { x, y: 0, z: 0 }
  };
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
