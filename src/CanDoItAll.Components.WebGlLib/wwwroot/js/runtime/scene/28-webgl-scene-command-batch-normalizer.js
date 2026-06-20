export function normalizeCommandBatch(batch) {
    const patches = Array.isArray(batch?.patches) ? batch.patches : [];
    const motions = Array.isArray(batch?.motions) ? batch.motions : [];
    const stages = Array.isArray(batch?.stages) ? batch.stages : [];
    const batchingPolicy = normalizeBatchingPolicy(batch?.batchingPolicy, batch?.orderingMode);
    const orderingMode = normalizeOrderingMode(batch?.orderingMode, batchingPolicy);
    const commandCount = countCommands(patches, motions, stages);
    const metrics = {
        batchCommandCount: commandCount,
        commandCountBeforeNormalization: commandCount,
        commandCountAfterNormalization: 0,
        stageCount: stages.length,
        batchDurationMs: 0,
        coalescedPatchCount: 0,
        droppedDuplicateMotionCount: 0,
        preservedOrderedDuplicateMotionCount: 0,
        estimatedHostInteropCallCount: commandCount > 0 ? 1 : 0,
        interopCallsAvoided: 0
    };
    const warnings = [];
    const normalized = {
        batchId: batch?.batchId || "command-batch",
        batchingPolicy,
        patches: coalescePatches(patches, motions.length > 0, orderingMode, metrics, warnings, batch?.batchId || "command-batch"),
        motions: deduplicateMotions(motions, batch?.allowDuplicateMotionsPerObject === true, orderingMode, metrics, warnings, batch?.batchId || "command-batch"),
        stages: stages.map(stage => normalizeStage(stage, batch, metrics, warnings)),
        metrics,
        warnings
    };
    normalized.metrics.commandCountAfterNormalization = countCommands(normalized.patches, normalized.motions, normalized.stages);
    normalized.metrics.interopCallsAvoided = Math.max(0, normalized.metrics.commandCountBeforeNormalization - normalized.metrics.estimatedHostInteropCallCount);
    return normalized;
}

export function normalizeCommandBatchForAudit(batch) {
    return normalizeCommandBatch(batch);
}

function normalizeStage(stage, batch, metrics, warnings) {
    const patches = Array.isArray(stage?.patches) ? stage.patches : [];
    const motions = Array.isArray(stage?.motions) ? stage.motions : [];
    const batchingPolicy = normalizeBatchingPolicy(stage?.batchingPolicy ?? batch?.batchingPolicy, stage?.orderingMode ?? batch?.orderingMode);
    const orderingMode = normalizeOrderingMode(stage?.orderingMode ?? batch?.orderingMode, batchingPolicy);
    const stageMetrics = {
        coalescedPatchCount: 0,
        droppedDuplicateMotionCount: 0,
        preservedOrderedDuplicateMotionCount: 0
    };
    const batchId = `${batch?.batchId || "command-batch"}:${stage?.stageId || "stage"}`;
    const normalized = {
        stageId: stage?.stageId || "",
        orderingMode,
        batchingPolicy,
        waitSeconds: Number(stage?.waitSeconds) || 0,
        barrierPolicy: stage?.barrierPolicy || stage?.metadata?.barrierPolicy || "",
        barrierObjectIds: Array.isArray(stage?.barrierObjectIds)
            ? [...stage.barrierObjectIds]
            : stringList(stage?.metadata?.barrierObjectIds),
        barrierEventId: stage?.barrierEventId || stage?.metadata?.barrierEventId || "",
        metadata: {
            ...(stage?.metadata || {}),
            orderingMode,
            batchingPolicy,
            barrierPolicy: stage?.barrierPolicy || stage?.metadata?.barrierPolicy || "",
            barrierEventId: stage?.barrierEventId || stage?.metadata?.barrierEventId || "",
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
    metrics.preservedOrderedDuplicateMotionCount += stageMetrics.preservedOrderedDuplicateMotionCount;
    return normalized;
}

function stringList(value) {
    return String(value || "")
        .split(/[;,]/)
        .map(item => item.trim())
        .filter(Boolean);
}

function normalizeBatchingPolicy(value, orderingMode) {
    const policy = String(value || "").toLowerCase();
    if (policy === "preserve-order" || policy === "preserveorder" || policy === "sequential") {
        return "preserve-order";
    }

    if (policy === "parallel") {
        return "parallel";
    }

    if (policy === "coalesce-within-stage" || policy === "coalesceindependent") {
        return "coalesce-within-stage";
    }

    const ordering = String(orderingMode ?? "").toLowerCase();
    if (ordering === "1" || ordering === "2" || ordering === "preserveorder" || ordering === "preserve-order" || ordering === "sequential") {
        return "preserve-order";
    }

    return "coalesce-within-stage";
}

function normalizeOrderingMode(value, batchingPolicy = "") {
    if (batchingPolicy === "preserve-order") {
        const mode = String(value ?? "").toLowerCase();
        return mode === "2" || mode === "sequential" ? "sequential" : "preserve-order";
    }

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
        metrics.preservedOrderedDuplicateMotionCount = (metrics.preservedOrderedDuplicateMotionCount || 0) + countDuplicateMotions(motions);
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

function countDuplicateMotions(motions) {
    const seen = new Set();
    let count = 0;
    for (const motion of motions) {
        const objectId = motion?.objectId || "";
        if (!objectId) {
            continue;
        }

        if (seen.has(objectId)) {
            count += 1;
        } else {
            seen.add(objectId);
        }
    }

    return count;
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
