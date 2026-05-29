import { round, resolveObjectPosition } from "./02-webgl-scene-core.js";
import { notifyStateChanged } from "./05-webgl-scene-interaction.js";
import { updateObjectRuntimeTransform } from "./11-webgl-scene-graph.js";

export function enqueueMotion(state, command) {
    return enqueueMotionDetailed(state, command).success;
}

export function enqueueMotionDetailed(state, command) {
    const normalized = normalizeCommand(state, command);
    if (!normalized) {
        return commandResult(state, "motion-enqueue", command?.motionId || "", false, [`Motion target '${command?.objectId || ""}' was not found.`]);
    }

    if (normalized.replaceExistingForObject !== false) {
        for (const [motionId, motion] of state.motions.entries()) {
            if (motion.objectId === normalized.objectId) {
                state.motions.delete(motionId);
            }
        }
    }

    state.motions.set(normalized.motionId, normalized);
    state.diagnostics.motionAcceptedCount += 1;
    state.scheduleRender("motion-enqueued");
    return commandResult(state, "motion-enqueue", normalized.motionId, true, [], [normalized.objectId]);
}

export function clearMotions(state, objectId) {
    return clearMotionsDetailed(state, objectId).success;
}

export function clearMotionsDetailed(state, objectId) {
    const affected = [];
    if (!objectId) {
        affected.push(...Array.from(state.motions.values()).map(motion => motion.objectId));
        state.motions.clear();
        state.scheduleRender("motion-clear");
        return commandResult(state, "motion-clear", "", true, [], affected);
    }

    for (const [motionId, motion] of state.motions.entries()) {
        if (motion.objectId === objectId) {
            affected.push(motion.objectId);
            state.motions.delete(motionId);
        }
    }

    state.scheduleRender("motion-clear");
    return commandResult(state, "motion-clear", "", true, [], affected);
}

export function advanceMotions(state, deltaSeconds) {
    if (!state.motions.size) {
        return;
    }

    const completed = [];
    for (const motion of state.motions.values()) {
        const sceneObject = state.objectLookup.get(motion.objectId);
        if (!sceneObject) {
            completed.push(motion.motionId);
            continue;
        }

        motion.elapsedSeconds += deltaSeconds;
        const t = Math.min(1, motion.elapsedSeconds / Math.max(motion.durationSeconds, 0.001));
        const eased = applyEasing(t, motion.easing);
        sceneObject.position = {
            x: round(lerp(motion.startPosition.x, motion.targetPosition.x, eased), 3),
            y: round(lerp(motion.startPosition.y, motion.targetPosition.y, eased), 3),
            z: round(lerp(motion.startPosition.z, motion.targetPosition.z, eased), 3)
        };
        updateObjectRuntimeTransform(state, motion.objectId, true);

        if (t >= 1) {
            if (motion.snapAtEnd !== false) {
                sceneObject.position = motion.targetPosition;
                updateObjectRuntimeTransform(state, motion.objectId, true);
            }

            completed.push(motion.motionId);
        }
    }

    for (const motionId of completed) {
        const motion = state.motions.get(motionId);
        state.motions.delete(motionId);
        if (motion) {
            state.diagnostics.motionCompletedCount += 1;
            notifyMotionCompleted(state, motion);
        }
    }

    if (completed.length) {
        notifyStateChanged(state);
    }

    state.scheduleRender(state.motions.size ? "motion" : "motion-complete");
}

function normalizeCommand(state, command) {
    const objectId = command?.objectId || "";
    const sceneObject = objectId ? state.objectLookup.get(objectId) : null;
    if (!sceneObject) {
        return failMotion(state, `Motion target '${objectId}' was not found.`);
    }

    const startPosition = resolveObjectPosition(sceneObject);
    const targetPosition = normalizePosition(command.targetPosition, startPosition);
    const distance = Math.hypot(
        targetPosition.x - startPosition.x,
        targetPosition.y - startPosition.y,
        targetPosition.z - startPosition.z);
    const speed = Math.max(0, Number(command.speedUnitsPerSecond) || 0);
    const duration = Math.max(
        0.001,
        Number(command.durationSeconds) > 0
            ? Number(command.durationSeconds)
            : distance / Math.max(speed, 0.001));

    return {
        motionId: command.motionId || `${objectId}:${Date.now()}`,
        objectId,
        startPosition: { x: startPosition.x, y: startPosition.y, z: startPosition.z },
        targetPosition,
        durationSeconds: duration,
        elapsedSeconds: 0,
        easing: command.easing || "linear",
        snapAtEnd: command.snapAtEnd !== false,
        replaceExistingForObject: command.replaceExistingForObject !== false
    };
}

function normalizePosition(value, fallback) {
    return {
        x: round(Number(value?.x ?? value?.X ?? fallback.x), 3),
        y: round(Number(value?.y ?? value?.Y ?? fallback.y), 3),
        z: round(Number(value?.z ?? value?.Z ?? fallback.z), 3)
    };
}

function applyEasing(t, easing) {
    switch (easing) {
        case "ease-in":
            return t * t;
        case "ease-out":
            return 1 - Math.pow(1 - t, 2);
        case "ease-in-out":
            return t < 0.5 ? 2 * t * t : 1 - Math.pow(-2 * t + 2, 2) / 2;
        default:
            return t;
    }
}

function lerp(start, end, t) {
    return start + (end - start) * t;
}

function failMotion(state, message) {
    state.diagnostics.failedPatchCommands.add(message);
    state.diagnostics.motionFailedCount += 1;
    state.diagnostics.failedCommandDetails.push(commandResult(state, "motion", "", false, [message]));
    state.diagnostics.lastError = message;
    state.notifyRuntimeError?.("WebGL scene motion failed.", new Error(message));
    return null;
}

function commandResult(state, commandKind, commandId, success, errors = [], affectedObjectIds = []) {
    const result = {
        commandId: commandId || `${commandKind}:${Date.now()}`,
        success,
        succeeded: success,
        sceneId: state.sceneModel.sceneId || "",
        commandKind,
        revision: state.sceneModel.uiState?.revision || 0,
        errors,
        warnings: [],
        affectedObjectIds: Array.from(new Set(affectedObjectIds.filter(Boolean))),
        affectedLinkIds: [],
        diagnostics: {
            activeMotionCount: String(state.motions?.size || 0),
            renderCount: String(state.diagnostics.renderCount || 0)
        }
    };
    state.commandResults.push(result);
    return result;
}

function notifyMotionCompleted(state, motion) {
    const result = commandResult(state, "motion-completed", motion.motionId, true, [], [motion.objectId]);
    state.dotNetRef?.invokeMethodAsync("OnMotionCompleted", JSON.stringify(result))
        .catch(error => console.warn("WebGL scene motion completion callback failed.", error));
}
