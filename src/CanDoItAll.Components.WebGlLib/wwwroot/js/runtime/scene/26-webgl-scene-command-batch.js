import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatchDetailed } from "./13-webgl-scene-patching.js";
import { enqueueMotionDetailed } from "./14-webgl-scene-motion.js";
import { compactBatchResultForInterop, completeCommandResult, createCommandResult, warnCommand } from "./20-webgl-scene-command-results.js";
import { normalizeCommandBatch, normalizeCommandBatchForAudit } from "./28-webgl-scene-command-batch-normalizer.js";

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
    compactBatchResultForInterop(state, result);
    return completeCommandResult(state, result);
}

export function advanceCommandBatchStages(state, deltaSeconds) {
    const pending = state.pendingCommandStages || [];
    if (!pending.length) {
        return;
    }

    const ready = [];
    for (const item of pending) {
        item.remainingSeconds = Math.max(0, item.remainingSeconds - deltaSeconds);
        if (item.remainingSeconds <= 0) {
            ready.push(item);
        }
    }

    state.pendingCommandStages = pending.filter(item => item.remainingSeconds > 0);
    for (const item of ready) {
        applyStage(state, item.stage, null);
    }

    if (state.pendingCommandStages.length) {
        state.scheduleRender("command-stage");
    }
}

function applyOrScheduleStages(state, normalized, result) {
    let delaySeconds = 0;
    for (const stage of normalized.stages) {
        if (delaySeconds <= 0) {
            applyStage(state, stage, result);
        } else {
            state.pendingCommandStages ??= [];
            state.pendingCommandStages.push({
                batchId: normalized.batchId,
                stage,
                remainingSeconds: delaySeconds
            });
        }

        delaySeconds += Math.max(0, Number(stage.waitSeconds) || 0);
    }

    if ((state.pendingCommandStages || []).length) {
        state.scheduleRender("command-stage");
    }
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
}
