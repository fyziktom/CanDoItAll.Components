import {
    completeCommandResult,
    createCommandResult,
    failCommand
} from "./20-webgl-scene-command-results.js";
import {
    activateNextMotion,
    removeQueuedMotion,
    syncMotionQueueDiagnostics
} from "./29-webgl-scene-motion-queues.js";

export function cancelMotion(state, motionId) {
    return cancelMotionDetailed(state, motionId).success;
}

export function cancelMotionDetailed(state, motionId) {
    const result = createCommandResult(state, "motion-cancel", motionId || "");
    if (!motionId || (!state.motions.has(motionId) && !removeQueuedMotion(state, motionId, result))) {
        failCommand(state, result, `Motion '${motionId || ""}' was not found.`, "WebGL scene motion cancel failed.");
        return completeCommandResult(state, result);
    }

    if (!state.motions.has(motionId)) {
        state.scheduleRender("motion-cancel");
        return completeCommandResult(state, result);
    }

    const motion = state.motions.get(motionId);
    state.motions.delete(motionId);
    state.diagnostics.cancelledMotionCount = (state.diagnostics.cancelledMotionCount || 0) + 1;
    activateNextMotion(state, motion.objectId, motionVectorPayload);
    syncMotionQueueDiagnostics(state);
    result.affectedObjectIds.push(motion?.objectId || "");
    state.scheduleRender("motion-cancel");
    return completeCommandResult(state, result);
}

function motionVectorPayload(vector) {
    return { x: vector.x, y: vector.y, z: vector.z };
}
