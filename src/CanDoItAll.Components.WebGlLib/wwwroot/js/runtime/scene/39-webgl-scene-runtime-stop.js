import { clearMotionsDetailed } from "./14-webgl-scene-motion.js";
import { completeCommandResult, createCommandResult } from "./20-webgl-scene-command-results.js";
import { syncMotionQueueDiagnostics } from "./29-webgl-scene-motion-queues.js";
import { cancelCommandStageRunner, syncStageDiagnostics } from "./30-webgl-scene-stage-runner.js";

export function cancelCommandStages(state, reason = "cancelled") {
    const normalizedReason = normalizeReason(reason);
    const result = createCommandResult(state, "command-stage-cancel", normalizedReason);
    const stageWorkBefore = countCommandStageWork(state);

    if (stageWorkBefore > 0) {
        cancelCommandStageRunner(state, normalizedReason);
    } else {
        syncStageDiagnostics(state);
    }

    state.diagnostics.lastRuntimeStopCancelledCommandStageCount = stageWorkBefore;
    result.metadata = {
        ...result.metadata,
        reason: normalizedReason,
        cancelledCommandStageCount: String(stageWorkBefore),
        queuedCommandStageCountAfter: String(state.diagnostics.queuedCommandStageCount || 0),
        commandStageCancelledCount: String(state.diagnostics.commandStageCancelledCount || 0)
    };
    state.scheduleRender("command-stage-cancel");
    return completeCommandResult(state, result);
}

export function stopRuntimeActivity(state, reason = "runtime-stop") {
    const normalizedReason = normalizeReason(reason);
    const result = createCommandResult(state, "runtime-stop", normalizedReason);
    const activeMotionCountBefore = state.motions?.size || 0;
    const queuedMotionCountBefore = countQueuedMotions(state);
    const clearedMotionCount = activeMotionCountBefore + queuedMotionCountBefore;
    const stageWorkBefore = countCommandStageWork(state);
    let clearMotionResult = null;

    if (stageWorkBefore > 0) {
        cancelCommandStageRunner(state, normalizedReason);
    } else {
        syncStageDiagnostics(state);
    }

    if (clearedMotionCount > 0) {
        clearMotionResult = clearMotionsDetailed(state);
        result.affectedObjectIds.push(...(clearMotionResult?.affectedObjectIds || []));
        result.warnings.push(...(clearMotionResult?.warnings || []));
        result.errors.push(...(clearMotionResult?.errors || []));
    } else {
        syncMotionQueueDiagnostics(state);
    }

    state.diagnostics.runtimeStopCount = (state.diagnostics.runtimeStopCount || 0) + 1;
    state.diagnostics.lastRuntimeStopReason = normalizedReason;
    state.diagnostics.clearedMotionCount = (state.diagnostics.clearedMotionCount || 0) + clearedMotionCount;
    state.diagnostics.lastRuntimeStopClearedMotionCount = clearedMotionCount;
    state.diagnostics.lastRuntimeStopCancelledCommandStageCount = stageWorkBefore;
    syncMotionQueueDiagnostics(state);
    syncStageDiagnostics(state);

    result.metadata = {
        ...result.metadata,
        reason: normalizedReason,
        activeMotionCountBefore: String(activeMotionCountBefore),
        queuedMotionCountBefore: String(queuedMotionCountBefore),
        clearedMotionCount: String(clearedMotionCount),
        cancelledCommandStageCount: String(stageWorkBefore),
        activeMotionCountAfter: String(state.motions?.size || 0),
        queuedMotionCountAfter: String(state.diagnostics.queuedMotionCount || 0),
        queuedCommandStageCountAfter: String(state.diagnostics.queuedCommandStageCount || 0),
        runtimeStopCount: String(state.diagnostics.runtimeStopCount || 0),
        commandStageCancelledCount: String(state.diagnostics.commandStageCancelledCount || 0)
    };
    state.scheduleRender("runtime-stop");
    return completeCommandResult(state, result);
}

function countQueuedMotions(state) {
    let queued = 0;
    for (const queue of state.motionQueuesByObjectId?.values?.() || []) {
        queued += queue?.length || 0;
    }

    return queued;
}

function countCommandStageWork(state) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return 0;
    }

    return (runner.queue?.length || 0) + (runner.activeBarrier ? 1 : 0);
}

function normalizeReason(reason) {
    const value = String(reason || "").trim();
    return value || "runtime-stop";
}
