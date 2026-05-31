import {
    resolveObjectPosition,
    resolveObjectRotation,
    resolveObjectScale
} from "./02-webgl-scene-core.js";

export function hasObjectMotion(state, objectId) {
    return Array.from(state.motions.values()).some(motion => motion.objectId === objectId) ||
        getObjectQueue(state, objectId).length > 0;
}

export function enqueueObjectMotion(state, motion) {
    getObjectQueue(state, motion.objectId).push(motion);
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
}

export function clearObjectMotionState(state, objectId, result = null) {
    for (const [motionId, motion] of state.motions.entries()) {
        if (motion.objectId === objectId) {
            result?.affectedObjectIds.push(motion.objectId);
            state.motions.delete(motionId);
        }
    }

    if (state.motionQueuesByObjectId?.has(objectId)) {
        result?.affectedObjectIds.push(objectId);
        state.motionQueuesByObjectId.delete(objectId);
    }
}

export function removeQueuedMotion(state, motionId, result) {
    for (const [objectId, queue] of state.motionQueuesByObjectId || []) {
        const index = queue.findIndex(motion => motion.motionId === motionId);
        if (index < 0) {
            continue;
        }

        queue.splice(index, 1);
        result.affectedObjectIds.push(objectId);
        if (queue.length === 0) {
            state.motionQueuesByObjectId.delete(objectId);
        }

        return true;
    }

    return false;
}
