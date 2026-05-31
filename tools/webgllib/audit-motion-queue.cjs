const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSceneDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const reportDir = path.join(repoRoot, "artifacts", "webgl-runtime-motion-queue-hardening-v14", "motion-queue");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtime = await import(`${pathToFileURL(writeAuditModule()).href}?v=${Date.now()}`);
  const completedAssertions = [];

  let state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.to.target", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.to.home", 0, "append"));
  assertEqual(state.motions.size, 1, "one active motion after append enqueue");
  assertEqual(state.motionQueuesByObjectId.get("actor").length, 1, "one queued motion after append enqueue");
  assertEqual(state.diagnostics.queuedMotionCount, 1, "diagnostics queued motion count after append enqueue");
  assertEqual(state.diagnostics.maxMotionQueueLength, 1, "diagnostics max queue length after append enqueue");
  completedAssertions.push("append queues after active motion");

  runtime.advanceMotions(state, 0.5);
  assertEqual(state.objectLookup.get("actor").position.x, 2, "halfway through first motion");
  assertEqual([...state.motions.keys()][0], "actor.to.target", "first motion remains active before completion");

  runtime.advanceMotions(state, 0.5);
  assertEqual(state.objectLookup.get("actor").position.x, 4, "first motion completes at target");
  assertEqual([...state.motions.keys()][0], "actor.to.home", "second motion activates only after first completion");
  assertEqual(state.motions.get("actor.to.home").startPosition.x, 4, "queued motion starts from completed active transform");
  assertEqual(state.motionQueuesByObjectId.has("actor"), false, "queue is empty after promotion");
  completedAssertions.push("active completion activates next motion with updated start transform");

  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 0, "second motion completes at home");
  assertEqual(state.motions.size, 0, "no active motion after queued sequence completes");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  const queuedCancel = runtime.cancelMotionDetailed(state, "actor.queued");
  assertEqual(queuedCancel.success, true, "queued motion cancel succeeds");
  assertEqual(state.motions.size, 1, "active motion remains after queued cancel");
  assertEqual(state.diagnostics.queuedMotionCount, 0, "queued cancel removes queue item");
  completedAssertions.push("queued motion cancel removes only queued item");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  runtime.advanceMotions(state, 0.5);
  assertEqual(state.objectLookup.get("actor").position.x, 2, "active motion reaches mid-point before cancel");
  const activeCancel = runtime.cancelMotionDetailed(state, "actor.active");
  assertEqual(activeCancel.success, true, "active motion cancel succeeds");
  assertEqual([...state.motions.keys()][0], "actor.queued", "queued motion promotes after active cancel");
  assertEqual(state.motions.get("actor.queued").startPosition.x, 2, "promoted motion starts from current transform after cancel");
  assertEqual(state.diagnostics.queuedMotionCount, 0, "active cancel drains one queued item into active state");
  runtime.advanceMotions(state, 1);
  assertEqual(state.objectLookup.get("actor").position.x, 8, "promoted motion reaches target after active cancel");
  completedAssertions.push("active motion cancel promotes queued motion from current transform");

  state = createState();
  runtime.enqueueMotionDetailed(state, motion("actor.active", 4, "append"));
  runtime.enqueueMotionDetailed(state, motion("actor.queued", 8, "append"));
  runtime.clearObjectMotionState(state, "actor");
  assertEqual(state.motions.size, 0, "object removal clear removes active motion");
  assertEqual(state.motionQueuesByObjectId.has("actor"), false, "object removal clear removes queued motions");
  completedAssertions.push("object removal clear removes active and queued motion state");

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB03.motion-queue.edge-cases",
    activeMotionCount: state.motions.size,
    queuedMotionCount: state.diagnostics.queuedMotionCount,
    maxMotionQueueLength: state.diagnostics.maxMotionQueueLength,
    cancelledMotionCount: state.diagnostics.cancelledMotionCount,
    acceptedMotionCount: state.diagnostics.motionAcceptedCount,
    completedMotionCount: state.diagnostics.motionCompletedCount,
    finalPosition: state.objectLookup.get("actor").position,
    assertions: completedAssertions
  };
  fs.writeFileSync(path.join(reportDir, "motion-queue-proof.json"), `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log("Motion queue audit passed for append, promotion, cancellation, and object-removal clearing.");
}

function writeAuditModule() {
  const queueSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "29-webgl-scene-motion-queues.js"), "utf8"));
  const cancellationSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "31-webgl-scene-motion-cancellation.js"), "utf8"));
  const motionSource = stripModuleSyntax(fs.readFileSync(path.join(runtimeSceneDir, "14-webgl-scene-motion.js"), "utf8"));
  const modulePath = path.join(reportDir, "motion-queue-runtime-audit.mjs");
  fs.writeFileSync(
    modulePath,
    `${runtimeStubs()}\n${queueSource}\n${cancellationSource}\n${motionSource}\nexport { enqueueMotionDetailed, cancelMotionDetailed, clearObjectMotionState, advanceMotions };\n`,
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
      maxMotionQueueLength: 0,
      cancelledMotionCount: 0
    },
    options: { deterministicMode: true },
    scheduleRender() {},
    dotNetRef: null
  };
}

function motion(motionId, x, queueMode) {
  return {
    motionId,
    objectId: "actor",
    durationSeconds: 1,
    queueMode,
    targetPosition: { x, y: 0, z: 0 }
  };
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
