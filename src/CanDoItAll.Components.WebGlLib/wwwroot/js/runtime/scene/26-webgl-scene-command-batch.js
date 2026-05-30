import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatchDetailed } from "./13-webgl-scene-patching.js";
import { enqueueMotionDetailed } from "./14-webgl-scene-motion.js";
import { compactBatchResultForInterop, completeCommandResult, createCommandResult, warnCommand } from "./20-webgl-scene-command-results.js";

export function applyCommandBatch(state, batch) {
    const startedAt = performance.now();
    const normalized = normalizeBatch(batch);
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

    for (const stage of normalized.stages) {
        for (const patch of stage.patches) {
            const patchResult = applyPatchDetailed(state, patch);
            appendChildResult(result, patchResult);
        }

        for (const motion of stage.motions) {
            const motionResult = enqueueMotionDetailed(state, motion);
            appendChildResult(result, motionResult);
        }
    }

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
        droppedDuplicateMotionCount: String(normalized.metrics.droppedDuplicateMotionCount)
    };
    compactBatchResultForInterop(state, result);
    return completeCommandResult(state, result);
}

function appendChildResult(batchResult, childResult) {
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

function normalizeBatch(batch) {
    const patches = Array.isArray(batch?.patches) ? batch.patches : [];
    const motions = Array.isArray(batch?.motions) ? batch.motions : [];
    const stages = Array.isArray(batch?.stages) ? batch.stages : [];
    const orderingMode = normalizeOrderingMode(batch?.orderingMode);
    const metrics = {
        batchCommandCount: countCommands(patches, motions, stages),
        stageCount: stages.length,
        batchDurationMs: 0,
        coalescedPatchCount: 0,
        droppedDuplicateMotionCount: 0,
        estimatedHostInteropCallCount: patches.length + motions.length > 0 ? 1 : 0
    };
    const warnings = [];

    return {
        batchId: batch?.batchId || "command-batch",
        patches: coalescePatches(patches, motions.length > 0, orderingMode, metrics, warnings, batch?.batchId || "command-batch"),
        motions: deduplicateMotions(motions, batch?.allowDuplicateMotionsPerObject === true, orderingMode, metrics, warnings, batch?.batchId || "command-batch"),
        stages: stages.map(stage => normalizeStage(stage, batch, metrics, warnings)),
        metrics,
        warnings
    };
}

export function normalizeCommandBatchForAudit(batch) {
    return normalizeBatch(batch);
}

function normalizeStage(stage, batch, metrics, warnings) {
    const patches = Array.isArray(stage?.patches) ? stage.patches : [];
    const motions = Array.isArray(stage?.motions) ? stage.motions : [];
    const orderingMode = normalizeOrderingMode(stage?.orderingMode ?? batch?.orderingMode);
    const stageMetrics = {
        coalescedPatchCount: 0,
        droppedDuplicateMotionCount: 0
    };
    const batchId = `${batch?.batchId || "command-batch"}:${stage?.stageId || "stage"}`;
    const normalized = {
        stageId: stage?.stageId || "",
        orderingMode,
        waitSeconds: Number(stage?.waitSeconds) || 0,
        metadata: {
            ...(stage?.metadata || {}),
            orderingMode,
            stageCommandCount: String(patches.length + motions.length)
        },
        patches: coalescePatches(patches, motions.length > 0, orderingMode, stageMetrics, warnings, batchId),
        motions: deduplicateMotions(
            motions,
            stage?.allowDuplicateMotionsPerObject === true || batch?.allowDuplicateMotionsPerObject === true,
            orderingMode,
            stageMetrics,
            warnings,
            batchId)
    };
    metrics.coalescedPatchCount += stageMetrics.coalescedPatchCount;
    metrics.droppedDuplicateMotionCount += stageMetrics.droppedDuplicateMotionCount;
    return normalized;
}

function normalizeOrderingMode(value) {
    const mode = String(value ?? "coalesceIndependent").toLowerCase();
    if (mode === "1" || mode === "preserveorder" || mode === "preserve-order") {
        return "preserve-order";
    }

    if (mode === "2" || mode === "sequential") {
        return "sequential";
    }

    return "coalesce-independent";
}

function preservesOrder(orderingMode) {
    return orderingMode === "preserve-order" || orderingMode === "sequential";
}

function coalescePatches(patches, containsMotions, orderingMode, metrics, warnings, batchId) {
    if (patches.length <= 1 || preservesOrder(orderingMode)) {
        return patches;
    }

    if (!canCoalescePatches(patches, containsMotions)) {
        warnings.push(`Patch coalescing was skipped for batch '${batchId}' because the patch set has ordered semantics.`);
        return patches;
    }

    const merged = {
        sceneId: "",
        baseRevision: 0,
        nextRevision: 0,
        objectPatches: [],
        addObjects: [],
        removeObjectIds: [],
        addLinks: [],
        removeLinkIds: [],
        metadata: {}
    };
    const objectPatchesById = new Map();
    let originalObjectPatchCount = 0;

    for (const patch of patches) {
        merged.sceneId ||= patch?.sceneId || "";
        merged.baseRevision ||= patch?.baseRevision || 0;
        merged.nextRevision = patch?.nextRevision || merged.nextRevision;
        merged.addObjects.push(...(patch?.addObjects || []));
        merged.removeObjectIds.push(...(patch?.removeObjectIds || []));
        merged.addLinks.push(...(patch?.addLinks || []));
        merged.removeLinkIds.push(...(patch?.removeLinkIds || []));
        merged.metadata = { ...merged.metadata, ...(patch?.metadata || {}) };

        for (const objectPatch of patch?.objectPatches || []) {
            originalObjectPatchCount += 1;
            const objectId = objectPatch?.objectId || "";
            if (!objectId) {
                merged.objectPatches.push(objectPatch);
                continue;
            }

            objectPatchesById.set(objectId, mergeObjectPatch(objectPatchesById.get(objectId), objectPatch));
        }
    }

    merged.objectPatches.push(...objectPatchesById.values());
    metrics.coalescedPatchCount = Math.max(0, originalObjectPatchCount - objectPatchesById.size);
    return [merged];
}

function canCoalescePatches(patches, containsMotions) {
    let baseRevision = 0;
    let nextRevision = 0;
    for (const patch of patches) {
        if ((patch?.addObjects || []).length ||
            (patch?.removeObjectIds || []).length ||
            (patch?.addLinks || []).length ||
            (patch?.removeLinkIds || []).length) {
            return false;
        }

        if (isTruthy(patch?.metadata?.preserveOrder) ||
            isTruthy(patch?.metadata?.requiresOrderedSemantics) ||
            isTruthy(patch?.metadata?.dependsOnIntermediateState)) {
            return false;
        }

        if (Number(patch?.baseRevision) > 0) {
            baseRevision ||= Number(patch.baseRevision);
            if (baseRevision !== Number(patch.baseRevision)) {
                return false;
            }
        }

        if (Number(patch?.nextRevision) > 0) {
            nextRevision ||= Number(patch.nextRevision);
            if (nextRevision !== Number(patch.nextRevision)) {
                return false;
            }
        }

        if (containsMotions && (patch?.objectPatches || []).some(isStatefulObjectPatch)) {
            return false;
        }
    }

    return true;
}

function isStatefulObjectPatch(objectPatch) {
    return objectPatch?.assetId != null ||
        objectPatch?.symbols != null ||
        objectPatch?.metadata?.poseKey != null;
}

function mergeObjectPatch(existing, next) {
    if (!existing) {
        return { ...next, metadata: { ...(next?.metadata || {}) } };
    }

    return {
        ...existing,
        ...Object.fromEntries(Object.entries(next || {}).filter(([, value]) => value !== undefined && value !== null)),
        metadata: {
            ...(existing.metadata || {}),
            ...(next?.metadata || {})
        }
    };
}

function deduplicateMotions(motions, allowDuplicates, orderingMode, metrics, warnings, batchId) {
    if (allowDuplicates || preservesOrder(orderingMode)) {
        return motions;
    }

    const seenObjectIds = new Set();
    const deduplicated = [];
    for (const motion of motions) {
        const objectId = motion?.objectId || "";
        if (!objectId || !seenObjectIds.has(objectId)) {
            deduplicated.push(motion);
            if (objectId) {
                seenObjectIds.add(objectId);
            }
            continue;
        }

        metrics.droppedDuplicateMotionCount += 1;
        warnings.push(`Duplicate motion for object '${objectId}' was dropped from batch '${batchId}'.`);
    }

    return deduplicated;
}

function countCommands(patches, motions, stages) {
    return patches.length + motions.length + stages.reduce((count, stage) => {
        return count +
            (Array.isArray(stage?.patches) ? stage.patches.length : 0) +
            (Array.isArray(stage?.motions) ? stage.motions.length : 0);
    }, 0);
}

function isTruthy(value) {
    const normalized = String(value || "").toLowerCase();
    return normalized === "true" || normalized === "1" || normalized === "yes";
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
}
