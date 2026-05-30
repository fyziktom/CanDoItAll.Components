import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatchDetailed } from "./13-webgl-scene-patching.js";
import { enqueueMotionDetailed } from "./14-webgl-scene-motion.js";
import { completeCommandResult, createCommandResult, warnCommand } from "./20-webgl-scene-command-results.js";

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

    normalized.metrics.batchDurationMs = Math.round(performance.now() - startedAt);
    syncBatchDiagnostics(state, normalized.metrics);
    result.metrics = normalized.metrics;
    result.proofSnapshot = getProofSnapshot(state);
    result.metadata = {
        ...result.metadata,
        batchCommandCount: String(normalized.metrics.batchCommandCount),
        batchDurationMs: String(normalized.metrics.batchDurationMs),
        coalescedPatchCount: String(normalized.metrics.coalescedPatchCount),
        droppedDuplicateMotionCount: String(normalized.metrics.droppedDuplicateMotionCount)
    };
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
    const metrics = {
        batchCommandCount: patches.length + motions.length,
        batchDurationMs: 0,
        coalescedPatchCount: 0,
        droppedDuplicateMotionCount: 0,
        estimatedHostInteropCallCount: patches.length + motions.length > 0 ? 1 : 0
    };
    const warnings = [];

    return {
        batchId: batch?.batchId || "command-batch",
        patches: coalescePatches(patches, metrics),
        motions: deduplicateMotions(motions, batch?.allowDuplicateMotionsPerObject === true, metrics, warnings, batch?.batchId || "command-batch"),
        metrics,
        warnings
    };
}

function coalescePatches(patches, metrics) {
    if (patches.length <= 1) {
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

function deduplicateMotions(motions, allowDuplicates, metrics, warnings, batchId) {
    if (allowDuplicates) {
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

function syncBatchDiagnostics(state, metrics) {
    if (!state?.diagnostics) {
        return;
    }

    state.diagnostics.batchCommandCount = metrics.batchCommandCount;
    state.diagnostics.batchDurationMs = metrics.batchDurationMs;
    state.diagnostics.coalescedPatchCount = metrics.coalescedPatchCount;
    state.diagnostics.droppedDuplicateMotionCount = metrics.droppedDuplicateMotionCount;
}
