const fs = require("node:fs");
const path = require("node:path");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "30-webgl-scene-stage-runner.js");
const barrierSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "32-webgl-scene-stage-barriers.js");
const journalSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "33-webgl-scene-command-journal.js");
const schedulerSourcePath = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "22-webgl-scene-scheduler.js");
const reportDir = path.join(repoRoot, "artifacts", "webgl-runtime-stage-runner-hardening-v15", "stage-runner");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const runtimeModulePath = writeAuditRuntimeModule();
  const schedulerModulePath = writeAuditSchedulerModule();
  const runtime = await import(`${pathToFileURL(runtimeModulePath).href}?v=${Date.now()}`);
  const scheduler = await import(`${pathToFileURL(schedulerModulePath).href}?v=${Date.now()}`);

  const assertions = [];
  assertTimeDelayBarrier(runtime, assertions);
  assertNoneBarrier(runtime, assertions);
  assertWaitForActiveMotionsBarrier(runtime, assertions);
  assertWaitForObjectMotionsBarrier(runtime, assertions);
  assertWaitForRenderIdleBarrier(runtime, assertions);
  assertWaitForEventBarrier(runtime, scheduler, assertions);
  assertCommandJournal(runtime, assertions);
  assertSchedulerIntegration(scheduler, assertions);

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    invariantId: "SB03.stage-runner.barrier-diagnostics",
    assertions
  };
  fs.writeFileSync(path.join(reportDir, "stage-runner-proof.json"), `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log("Stage runner audit passed for none, wait-seconds, motion, object, render-idle, event barriers, scheduler integration, and journal diagnostics.");
}

function assertTimeDelayBarrier(runtime, assertions) {
  const state = createState();
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.time", [
    stage("move.target", { waitSeconds: 0.5 }),
    stage("change.pose", { waitSeconds: 0.25 }),
    stage("move.home")
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["move.target"], "first time-delay stage applies immediately");
  assertEqual(state.diagnostics.commandStageBarrierPolicy, "wait-seconds", "wait-seconds barrier policy");
  assertEqual(state.diagnostics.completedCommandStageCount, 1, "completed count after first time-delay stage");
  assertEqual(state.diagnostics.queuedCommandStageCount, 2, "queued count after first time-delay stage");

  runtime.advanceCommandStageRunner(state, 0.49, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target"], "time-delay barrier holds before elapsed time");
  runtime.advanceCommandStageRunner(state, 0.02, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target", "change.pose"], "second stage applies after elapsed time");
  runtime.advanceCommandStageRunner(state, 0.25, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.target", "change.pose", "move.home"], "all time-delay stages apply");
  assertEqual(state.diagnostics.commandStageResultLog.length, 3, "stage result log records wait-seconds stages");
  assertions.push("wait-seconds barrier preserves ordered wait stages and result log");
}

function assertNoneBarrier(runtime, assertions) {
  const state = createState();
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.none", [
    stage("first.none", { barrierPolicy: "none", waitSeconds: 10 }),
    stage("second.none")
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["first.none", "second.none"], "none barrier ignores waitSeconds and drains immediately");
  assertEqual(state.diagnostics.completedCommandStageCount, 2, "none barrier completes both stages");
  assertions.push("none barrier explicitly disables waits");
}

function assertWaitForActiveMotionsBarrier(runtime, assertions) {
  const state = createState();
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.active", [
    stage("move.actor", { barrierPolicy: "wait-for-active-motions" }),
    stage("show.symbol")
  ], item => {
    applied.push(item.stageId);
    if (item.stageId === "move.actor") {
      state.motions.set("motion.actor", { objectId: "actor" });
    }
  });

  assertDeepEqual(applied, ["move.actor"], "active-motion barrier holds queued stage");
  assertEqual(state.diagnostics.commandStageBarrierPolicy, "wait-for-active-motions", "active-motion barrier policy");
  assertEqual(state.diagnostics.queuedCommandStageCount, 1, "active-motion barrier leaves one queued stage");
  state.motions.clear();
  runtime.advanceCommandStageRunner(state, 0.016, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["move.actor", "show.symbol"], "active-motion barrier releases after active motions finish");
  assertions.push("wait-for-active-motions waits for all active or queued motions before next stage");
}

function assertWaitForObjectMotionsBarrier(runtime, assertions) {
  const state = createState();
  state.motions.set("motion.other", { objectId: "other" });
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.object", [
    stage("wait.actor", { barrierPolicy: "wait-for-object-motions", barrierObjectIds: ["actor"] }),
    stage("after.actor")
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["wait.actor", "after.actor"], "object-motion barrier ignores unrelated active motions");
  assertEqual(state.diagnostics.completedCommandStageCount, 2, "object-motion barrier completes both stages");
  assertions.push("wait-for-object-motions only blocks selected object ids");
}

function assertWaitForRenderIdleBarrier(runtime, assertions) {
  const state = createState();
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.idle", [
    stage("wait.idle", { barrierPolicy: "wait-for-render-idle" }),
    stage("after.idle")
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["wait.idle"], "render-idle barrier waits for another idle frame");
  runtime.advanceCommandStageRunner(state, 0.016, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["wait.idle", "after.idle"], "render-idle barrier releases after idle frame");
  assertions.push("wait-for-render-idle waits for an idle render turn before next stage");
}

function assertWaitForEventBarrier(runtime, scheduler, assertions) {
  const state = createState();
  const applied = [];
  runtime.enqueueCommandStages(state, "batch.event", [
    stage("wait.event", { barrierPolicy: "wait-for-event", barrierEventId: "asset.loaded" }),
    stage("after.event")
  ], item => applied.push(item.stageId));

  assertDeepEqual(applied, ["wait.event"], "event barrier applies first stage only");
  runtime.advanceCommandStageRunner(state, 1, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["wait.event"], "event barrier does not auto-advance");
  assertEqual(state.diagnostics.commandStageBarrierEventId, "asset.loaded", "event barrier id is visible");
  assertDeepEqual(state.diagnostics.commandStageBarrierBlockers, ["event:asset.loaded"], "event barrier blocker is visible");
  assertEqual(scheduler.resolveRenderReason(state), "", "scheduler stays idle while event is pending");
  assertEqual(runtime.signalCommandStageEvent(state, "asset.loaded"), true, "event signal accepted");
  assertEqual(scheduler.resolveRenderReason(state), "command-stage", "scheduler wakes after event signal");
  runtime.advanceCommandStageRunner(state, 0, item => applied.push(item.stageId));
  assertDeepEqual(applied, ["wait.event", "after.event"], "event signal advances next stage");
  assertions.push("wait-for-event barrier requires explicit runtime event signal");
}

function assertCommandJournal(runtime, assertions) {
  const state = createState({ maxCommandStageJournalEntries: 20 });
  const applied = [];
  runtime.enqueueCommandStages(
    state,
    "batch.journal",
    Array.from({ length: 25 }, (_, index) => stage(`journal.${index + 1}`)),
    item => applied.push(item.stageId));

  assertEqual(applied.length, 25, "journal stages apply");
  assertEqual(state.diagnostics.commandStageJournalCount, 20, "journal is bounded to configured size");
  assertEqual(state.diagnostics.commandStageJournalDroppedCount > 0, true, "journal tracks dropped entries");
  assertEqual(state.diagnostics.commandStageJournalCounters.started, 25, "journal counts started stages");
  assertEqual(state.diagnostics.commandStageJournalCounters.applied, 25, "journal counts applied stages");
  assertEqual(state.diagnostics.commandStageJournalCounters.completed, 25, "journal counts completed stages");
  assertEqual(state.diagnostics.commandStageRecentResultIds.length > 0, true, "journal exposes recent result ids");
  assertions.push("bounded command journal records delayed stage start, apply, completion, counters, and recent result ids");
}

function assertSchedulerIntegration(scheduler, assertions) {
  assertEqual(
    scheduler.resolveRenderReason(createSchedulerState({ queue: [stage("queued.stage")], activeBarrier: null })),
    "command-stage",
    "scheduler sees queued command stage runner state");
  assertEqual(
    scheduler.resolveRenderReason(createSchedulerState({ queue: [], activeBarrier: { policy: "wait-seconds", remainingSeconds: 0.25 } })),
    "command-stage",
    "scheduler sees command stage runner time-delay state");
  assertEqual(
    scheduler.resolveRenderReason(createSchedulerState({ queue: [stage("event.stage")], activeBarrier: { policy: "wait-for-event", eventId: "external.ready" } })),
    "",
    "scheduler ignores event barrier without signal");
  assertEqual(
    scheduler.resolveRenderReason(createSchedulerState({ queue: [], activeBarrier: null, motionQueuesByObjectId: new Map([["actor", [motion("queued", 1, "append")]]]) })),
    "motion",
    "scheduler sees queued motions");
  assertEqual(
    scheduler.resolveRenderReason(createSchedulerState({ queue: [stage("cancelled.stage")], cancelled: true })),
    "",
    "scheduler ignores cancelled command stage runner state");
  assertions.push("scheduler resolves command-stage only when runner has automatic pending work");
}

function writeAuditRuntimeModule() {
  const barrierSource = fs.readFileSync(barrierSourcePath, "utf8");
  const journalSource = fs.readFileSync(journalSourcePath, "utf8");
  fs.writeFileSync(path.join(reportDir, "stage-barriers-audit.mjs"), barrierSource, "utf8");
  fs.writeFileSync(path.join(reportDir, "command-journal-audit.mjs"), journalSource, "utf8");
  const source = fs.readFileSync(runtimeSourcePath, "utf8")
    .replace('from "./32-webgl-scene-stage-barriers.js"', 'from "./stage-barriers-audit.mjs"')
    .replace('from "./33-webgl-scene-command-journal.js"', 'from "./command-journal-audit.mjs"');
  const modulePath = path.join(reportDir, "stage-runner-runtime-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function writeAuditSchedulerModule() {
  const source = fs.readFileSync(schedulerSourcePath, "utf8")
    .replace('from "./30-webgl-scene-stage-runner.js"', 'from "./stage-runner-runtime-audit.mjs"');
  const modulePath = path.join(reportDir, "stage-runner-scheduler-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function createState(options = {}) {
  return {
    commandStageRunner: null,
    diagnostics: { animatedSymbolCount: 0 },
    motions: new Map(),
    motionQueuesByObjectId: new Map(),
    options: { renderMode: "auto", ...options },
    cameraDampingFrames: 0,
    renderRequested: false,
    renderReason: "",
    scheduleCount: 0,
    scheduleRender() {
      this.scheduleCount += 1;
    }
  };
}

function createSchedulerState(runner) {
  return {
    options: { renderMode: "auto" },
    diagnostics: { animatedSymbolCount: 0 },
    motions: new Map(),
    motionQueuesByObjectId: runner.motionQueuesByObjectId || new Map(),
    cameraDampingFrames: 0,
    renderRequested: false,
    renderReason: "",
    commandStageRunner: {
      queue: runner.queue || [],
      activeBarrier: runner.activeBarrier || null,
      manualStepRequested: runner.manualStepRequested === true,
      cancelled: runner.cancelled === true
    }
  };
}

function stage(stageId, options = {}) {
  return {
    stageId,
    waitSeconds: options.waitSeconds || 0,
    barrierPolicy: options.barrierPolicy || "",
    barrierObjectIds: options.barrierObjectIds || [],
    barrierEventId: options.barrierEventId || "",
    patches: [],
    motions: options.motions || []
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
