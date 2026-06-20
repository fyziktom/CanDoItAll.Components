import {
    resolveObjectPosition,
    resolveObjectRotation,
    resolveObjectScale
} from "./02-webgl-scene-core.js";

export function hasObjectMotion(state, objectId) {
    return hasActiveObjectMotion(state, objectId) ||
        (state.motionQueuesByObjectId?.get(objectId)?.length || 0) > 0;
}

export function hasActiveObjectMotion(state, objectId) {
    return Array.from(state.motions.values()).some(motion => motion.objectId === objectId);
}

export function enqueueObjectMotion(state, motion) {
    const queue = getObjectQueue(state, motion.objectId);
    queue.push(motion);
    state.diagnostics.maxMotionQueueLength = Math.max(state.diagnostics.maxMotionQueueLength || 0, queue.length);
    syncMotionQueueDiagnostics(state);
}

export function getObjectQueue(state, objectId) {
    state.motionQueuesByObjectId ??= new Map();
    if (!state.motionQueuesByObjectId.has(objectId)) {
        state.motionQueuesByObjectId.set(objectId, []);
    }

    return state.motionQueuesByObjectId.get(objectId);
}

export function activateNextMotion(state, objectId, vectorPayload) {
    const queue = getObjectQueue(state, objectId);
    const next = queue.shift();
    if (!next) {
        state.motionQueuesByObjectId.delete(objectId);
        return;
    }

    if (queue.length === 0) {
        state.motionQueuesByObjectId.delete(objectId);
    }

    const sceneObject = state.objectLookup.get(objectId);
    if (!sceneObject) {
        activateNextMotion(state, objectId, vectorPayload);
        return;
    }

    next.startPosition = vectorPayload(resolveObjectPosition(sceneObject));
    next.startRotation = resolveObjectRotation(sceneObject);
    next.startScale = resolveObjectScale(sceneObject);
    next.elapsedSeconds = 0;
    state.motions.set(next.motionId, next);
    syncMotionQueueDiagnostics(state);
}

export function clearObjectMotionState(state, objectId, result = null) {
    let cancelledCount = 0;
    for (const [motionId, motion] of state.motions.entries()) {
        if (motion.objectId === objectId) {
            result?.affectedObjectIds.push(motion.objectId);
            state.motions.delete(motionId);
            cancelledCount += 1;
        }
    }

    if (state.motionQueuesByObjectId?.has(objectId)) {
        cancelledCount += state.motionQueuesByObjectId.get(objectId).length;
        result?.affectedObjectIds.push(objectId);
        state.motionQueuesByObjectId.delete(objectId);
    }

    state.diagnostics.cancelledMotionCount = (state.diagnostics.cancelledMotionCount || 0) + cancelledCount;
    syncMotionQueueDiagnostics(state);
}

export function clearQueuedObjectMotions(state, objectId, result = null) {
    const queue = state.motionQueuesByObjectId?.get(objectId);
    if (!queue?.length) {
        return 0;
    }

    const cancelledCount = queue.length;
    result?.affectedObjectIds.push(objectId);
    state.motionQueuesByObjectId.delete(objectId);
    state.diagnostics.cancelledMotionCount = (state.diagnostics.cancelledMotionCount || 0) + cancelledCount;
    syncMotionQueueDiagnostics(state);
    return cancelledCount;
}

export function removeQueuedMotion(state, motionId, result) {
    for (const [objectId, queue] of state.motionQueuesByObjectId || []) {
        const index = queue.findIndex(motion => motion.motionId === motionId);
        if (index < 0) {
            continue;
        }

        queue.splice(index, 1);
        result.affectedObjectIds.push(objectId);
        state.diagnostics.cancelledMotionCount = (state.diagnostics.cancelledMotionCount || 0) + 1;
        if (queue.length === 0) {
            state.motionQueuesByObjectId.delete(objectId);
        }

        syncMotionQueueDiagnostics(state);
        return true;
    }

    return false;
}

export function syncMotionQueueDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    let queued = 0;
    let maxLength = state.diagnostics.maxMotionQueueLength || 0;
    const queueSnapshot = [];
    const queuedMotionIds = [];
    for (const [objectId, queue] of state.motionQueuesByObjectId || []) {
        queued += queue.length;
        maxLength = Math.max(maxLength, queue.length);
        const ids = queue.map(motion => motion.motionId || "").filter(Boolean);
        queuedMotionIds.push(...ids);
        queueSnapshot.push({ objectId, queuedMotionIds: ids });
    }

    state.diagnostics.activeMotionCount = state.motions?.size || 0;
    state.diagnostics.queuedMotionCount = queued;
    state.diagnostics.activeMotionIds = Array.from(state.motions?.keys?.() || []);
    state.diagnostics.queuedMotionIds = queuedMotionIds;
    state.diagnostics.motionQueueSnapshot = queueSnapshot;
    state.diagnostics.maxMotionQueueLength = maxLength;
}
