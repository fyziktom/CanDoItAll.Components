import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatchDetailed } from "./13-webgl-scene-patching.js";
import { enqueueMotionDetailed } from "./14-webgl-scene-motion.js";
import { compactBatchResultForInterop, completeCommandResult, createCommandResult, warnCommand } from "./20-webgl-scene-command-results.js";
import { normalizeCommandBatch, normalizeCommandBatchForAudit } from "./28-webgl-scene-command-batch-normalizer.js";
import { advanceCommandStageRunner, enqueueCommandStages, syncStageDiagnostics } from "./30-webgl-scene-stage-runner.js";

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
    advanceCommandStageRunner(state, deltaSeconds, stage => applyStage(state, stage, null));
}

function applyOrScheduleStages(state, normalized, result) {
    enqueueCommandStages(state, normalized.batchId, normalized.stages, stage => applyStage(state, stage, result));
    if ((state.commandStageRunner?.queue?.length || 0) > 0 || (state.commandStageRunner?.waitSeconds || 0) > 0) {
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
