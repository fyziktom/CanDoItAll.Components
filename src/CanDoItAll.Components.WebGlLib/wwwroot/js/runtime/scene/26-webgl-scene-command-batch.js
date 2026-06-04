import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatchDetailed } from "./13-webgl-scene-patching.js";
import { enqueueMotionDetailed } from "./14-webgl-scene-motion.js";
import {
    commandLifecycleStates,
    compactBatchResultForInterop,
    completeCommandResult,
    createCommandResult,
    failCommand,
    setCommandLifecycle,
    warnCommand
} from "./20-webgl-scene-command-results.js";
import { normalizeCommandBatch, normalizeCommandBatchForAudit } from "./28-webgl-scene-command-batch-normalizer.js";
import { advanceCommandStageRunner, enqueueCommandStages, hasPendingCommandStageRunnerWork, syncStageDiagnostics } from "./30-webgl-scene-stage-runner.js";
import { collectRuntimeIdleBlockers, waitForRuntimeIdle } from "./40-webgl-scene-runtime-idle.js";

export { normalizeCommandBatchForAudit };

export function applyCommandBatch(state, batch) {
    const startedAt = performance.now();
    const normalized = normalizeCommandBatch(batch);
    const result = createCommandResult(state, "command-batch", normalized.batchId);
    result.commandResults = [];
    result.metrics = normalized.metrics;

    for (const warning of normalized.warnings) {
        warnCommand(result, warning);
    }

    for (const patch of normalized.patches) {
        const patchResult = applyPatchDetailed(state, patch);
        appendChildResult(result, patchResult);
    }

    for (const motion of normalized.motions) {
        const motionResult = enqueueMotionDetailed(state, motion);
        appendChildResult(result, motionResult);
    }

    applyOrScheduleStages(state, normalized, result);

    normalized.metrics.batchDurationMs = Math.round(performance.now() - startedAt);
    syncBatchDiagnostics(state, normalized.metrics);
    result.metrics = normalized.metrics;
    result.proofSnapshot = getProofSnapshot(state);
    result.metadata = {
        ...result.metadata,
        batchCommandCount: String(normalized.metrics.batchCommandCount),
        stageCount: String(normalized.metrics.stageCount),
        batchDurationMs: String(normalized.metrics.batchDurationMs),
        coalescedPatchCount: String(normalized.metrics.coalescedPatchCount),
        droppedDuplicateMotionCount: String(normalized.metrics.droppedDuplicateMotionCount),
        commandCountBeforeNormalization: String(normalized.metrics.commandCountBeforeNormalization),
        commandCountAfterNormalization: String(normalized.metrics.commandCountAfterNormalization),
        preservedOrderedDuplicateMotionCount: String(normalized.metrics.preservedOrderedDuplicateMotionCount),
        interopCallsAvoided: String(normalized.metrics.interopCallsAvoided)
    };
    syncCommandBatchLifecycle(state, result);
    compactBatchResultForInterop(state, result);
    return completeCommandResult(state, result);
}

export async function applyCommandBatchAndWait(state, batch, options = {}) {
    const result = applyCommandBatch(state, batch);
    if (!result) {
        return null;
    }

    const timeoutMs = Number(options?.timeoutMs) > 0 ? Number(options.timeoutMs) : 2000;
    const pollIntervalMs = Number(options?.pollIntervalMs) > 0 ? Number(options.pollIntervalMs) : 16;
    const reason = String(options?.reason || `command-batch:${result.commandId || "settled"}`).trim();
    const requireRuntimeIdle = options?.requireRuntimeIdle !== false && options?.hardFailOnIdleTimeout !== false;
    const idleResult = await waitForRuntimeIdle(state, { timeoutMs, pollIntervalMs, reason });
    annotateCommandBatchIdleResult(result, idleResult);
    result.metadata.runtimeIdleRequired = String(requireRuntimeIdle);
    result.diagnostics.runtimeIdleRequired = String(requireRuntimeIdle);
    if (result.success && idleResult?.idle === true) {
        setCommandLifecycle(result, commandLifecycleStates.settled, true);
    } else if (result.success && requireRuntimeIdle) {
        failCommand(
            state,
            result,
            `Runtime idle proof failed for command batch '${result.commandId}'. Blockers: ${(idleResult?.blockers || []).join(", ")}.`,
            "WebGL scene command batch did not settle.");
    } else if (result.success) {
        setCommandLifecycle(result, commandLifecycleStates.scheduled, false);
        warnCommand(result, `Command batch did not settle before timeout. Blockers: ${(idleResult?.blockers || []).join(", ")}.`);
    } else {
        setCommandLifecycle(result, commandLifecycleStates.failed, false);
    }

    return result;
}

export function advanceCommandBatchStages(state, deltaSeconds) {
    advanceCommandStageRunner(state, deltaSeconds, stage => applyStage(state, stage, null));
}

function applyOrScheduleStages(state, normalized, result) {
    enqueueCommandStages(state, normalized.batchId, normalized.stages, stage => applyStage(state, stage, result));
    if (hasPendingCommandStageRunnerWork(state)) {
        state.scheduleRender("command-stage");
    }
    syncStageDiagnostics(state);
}

function applyStage(state, stage, result) {
    for (const patch of stage.patches) {
        const patchResult = applyPatchDetailed(state, patch);
        appendChildResult(result, patchResult);
    }

    for (const motion of stage.motions) {
        const motionResult = enqueueMotionDetailed(state, motion);
        appendChildResult(result, motionResult);
    }
}

function appendChildResult(batchResult, childResult) {
    if (!batchResult) {
        return;
    }

    if (!childResult) {
        batchResult.errors.push("Batch command returned no result.");
        return;
    }

    batchResult.commandResults.push(childResult);
    batchResult.affectedObjectIds.push(...(childResult.affectedObjectIds || []));
    batchResult.affectedLinkIds.push(...(childResult.affectedLinkIds || []));
    batchResult.errors.push(...(childResult.errors || []));
    batchResult.warnings.push(...(childResult.warnings || []));
}

function syncBatchDiagnostics(state, metrics) {
    if (!state?.diagnostics) {
        return;
    }

    state.diagnostics.batchCommandCount = metrics.batchCommandCount;
    state.diagnostics.batchStageCount = metrics.stageCount;
    state.diagnostics.batchDurationMs = metrics.batchDurationMs;
    state.diagnostics.coalescedPatchCount = metrics.coalescedPatchCount;
    state.diagnostics.droppedDuplicateMotionCount = metrics.droppedDuplicateMotionCount;
    state.diagnostics.commandCountBeforeNormalization = metrics.commandCountBeforeNormalization;
    state.diagnostics.commandCountAfterNormalization = metrics.commandCountAfterNormalization;
    state.diagnostics.preservedOrderedDuplicateMotionCount = metrics.preservedOrderedDuplicateMotionCount;
    state.diagnostics.interopCallsAvoided = metrics.interopCallsAvoided;
    syncStageDiagnostics(state);
}

function syncCommandBatchLifecycle(state, result) {
    if (result.errors.length > 0) {
        setCommandLifecycle(result, commandLifecycleStates.failed, false);
        return;
    }

    const blockers = collectRuntimeIdleBlockers(state);
    result.metadata.runtimeIdleBlockers = blockers.join(",");
    result.metadata.runtimeIdle = String(blockers.length === 0);
    result.diagnostics.runtimeIdleBlockers = blockers.join(",");
    result.diagnostics.runtimeIdle = String(blockers.length === 0);
    setCommandLifecycle(
        result,
        blockers.length === 0 ? commandLifecycleStates.settled : commandLifecycleStates.scheduled,
        blockers.length === 0);
}

function annotateCommandBatchIdleResult(result, idleResult) {
    result.metadata.runtimeIdle = String(idleResult?.idle === true);
    result.metadata.runtimeIdleTimedOut = String(idleResult?.timedOut === true);
    result.metadata.runtimeIdleElapsedMs = String(idleResult?.elapsedMs || 0);
    result.metadata.runtimeIdleBlockers = (idleResult?.blockers || []).join(",");
    result.diagnostics.runtimeIdle = result.metadata.runtimeIdle;
    result.diagnostics.runtimeIdleTimedOut = result.metadata.runtimeIdleTimedOut;
    result.diagnostics.runtimeIdleElapsedMs = result.metadata.runtimeIdleElapsedMs;
    result.diagnostics.runtimeIdleBlockers = result.metadata.runtimeIdleBlockers;
    if (idleResult?.diagnostics) {
        result.diagnostics.activeMotionCount = String(idleResult.diagnostics.activeMotionCount || 0);
        result.diagnostics.queuedMotionCount = String(idleResult.diagnostics.queuedMotionCount || 0);
        result.diagnostics.queuedCommandStageCount = String(idleResult.diagnostics.queuedCommandStageCount || 0);
        result.diagnostics.currentCommandBatchId = idleResult.diagnostics.currentCommandBatchId || "";
        result.diagnostics.currentCommandStageId = idleResult.diagnostics.currentCommandStageId || "";
        result.diagnostics.commandStageBarrierPolicy = idleResult.diagnostics.commandStageBarrierPolicy || "";
        result.diagnostics.commandStageBarrierBlockers = (idleResult.diagnostics.commandStageBarrierBlockers || []).join(",");
    }
}
