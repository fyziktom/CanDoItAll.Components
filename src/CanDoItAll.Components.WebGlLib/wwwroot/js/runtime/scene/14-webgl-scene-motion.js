import {
    round,
    resolveObjectPosition,
    resolveObjectRotation,
    resolveObjectScale
} from "./02-webgl-scene-core.js";
import { updateObjectRuntimeTransform } from "./11-webgl-scene-graph.js";
import {
    completeCommandResult,
    createCommandResult,
    failCommand
} from "./20-webgl-scene-command-results.js";
import { notifyStateChanged } from "./24-webgl-scene-notifications.js";

export function enqueueMotion(state, command) {
    return enqueueMotionDetailed(state, command).success;
}

export function enqueueMotionDetailed(state, command) {
    const result = createCommandResult(state, "motion-enqueue", command?.motionId || command?.commandId || "");
    const normalized = normalizeCommand(state, command, result);
    if (!normalized) {
        return completeCommandResult(state, result);
    }

    if (normalized.queueMode !== "append" && normalized.replaceExistingForObject !== false) {
        for (const [motionId, motion] of state.motions.entries()) {
            if (motion.objectId === normalized.objectId) {
                state.motions.delete(motionId);
            }
        }
    }

    state.motions.set(normalized.motionId, normalized);
    state.diagnostics.motionAcceptedCount += 1;
    result.commandId = normalized.motionId;
    result.affectedObjectIds.push(normalized.objectId);
    state.scheduleRender("motion-enqueued");
    return completeCommandResult(state, result);
}

export function clearMotions(state, objectId) {
    return clearMotionsDetailed(state, objectId).success;
}

export function clearMotionsDetailed(state, objectId) {
    const result = createCommandResult(state, "motion-clear", "");
    if (!objectId) {
        result.affectedObjectIds.push(...Array.from(state.motions.values()).map(motion => motion.objectId));
        state.motions.clear();
        state.scheduleRender("motion-clear");
        return completeCommandResult(state, result);
    }

    for (const [motionId, motion] of state.motions.entries()) {
        if (motion.objectId === objectId) {
            result.affectedObjectIds.push(motion.objectId);
            state.motions.delete(motionId);
        }
    }

    state.scheduleRender("motion-clear");
    return completeCommandResult(state, result);
}

export function cancelMotion(state, motionId) {
    return cancelMotionDetailed(state, motionId).success;
}

export function cancelMotionDetailed(state, motionId) {
    const result = createCommandResult(state, "motion-cancel", motionId || "");
    if (!motionId || !state.motions.has(motionId)) {
        failCommand(state, result, `Motion '${motionId || ""}' was not found.`, "WebGL scene motion cancel failed.");
        return completeCommandResult(state, result);
    }

    const motion = state.motions.get(motionId);
    state.motions.delete(motionId);
    result.affectedObjectIds.push(motion?.objectId || "");
    state.scheduleRender("motion-cancel");
    return completeCommandResult(state, result);
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
        sceneObject.position = lerpVector(motion.startPosition, motion.targetPosition, eased, 3);
        sceneObject.rotation = lerpVector(motion.startRotation, motion.targetRotation, eased, 4);
        sceneObject.scale = lerpVector(motion.startScale, motion.targetScale, eased, 4);
        updateObjectRuntimeTransform(state, motion.objectId, false);

        if (t >= 1) {
            if (motion.snapAtEnd !== false) {
                sceneObject.position = motion.targetPosition;
                sceneObject.rotation = motion.targetRotation;
                sceneObject.scale = motion.targetScale;
                updateObjectRuntimeTransform(state, motion.objectId, false);
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

function normalizeCommand(state, command, result) {
    const objectId = command?.objectId || "";
    const sceneObject = objectId ? state.objectLookup.get(objectId) : null;
    if (!sceneObject) {
        failMotion(state, result, `Motion target '${objectId}' was not found.`);
        return null;
    }

    const startPosition = vectorPayload(resolveObjectPosition(sceneObject));
    const startRotation = resolveObjectRotation(sceneObject);
    const startScale = resolveObjectScale(sceneObject);
    const targetPosition = normalizePosition(command.targetPosition, startPosition);
    const targetRotation = command.targetRotation ? normalizePosition(command.targetRotation, startRotation) : startRotation;
    const targetScale = command.targetScale ? normalizeScale(command.targetScale, startScale) : startScale;
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
        motionId: command.motionId || nextMotionId(state, objectId),
        objectId,
        startPosition,
        targetPosition,
        startRotation,
        targetRotation,
        startScale,
        targetScale,
        durationSeconds: duration,
        elapsedSeconds: 0,
        easing: command.easing || "linear",
        snapAtEnd: command.snapAtEnd !== false,
        replaceExistingForObject: command.replaceExistingForObject !== false,
        queueMode: String(command.queueMode || command.metadata?.queueMode || "").toLowerCase()
    };
}

function normalizePosition(value, fallback) {
    return {
        x: round(Number(value?.x ?? value?.X ?? fallback.x), 3),
        y: round(Number(value?.y ?? value?.Y ?? fallback.y), 3),
        z: round(Number(value?.z ?? value?.Z ?? fallback.z), 3)
    };
}

function normalizeScale(value, fallback) {
    const scale = normalizePosition(value, fallback);
    return {
        x: Math.max(0.01, scale.x),
        y: Math.max(0.01, scale.y),
        z: Math.max(0.01, scale.z)
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

function lerpVector(start, end, t, decimals) {
    return {
        x: round(lerp(start.x, end.x, t), decimals),
        y: round(lerp(start.y, end.y, t), decimals),
        z: round(lerp(start.z, end.z, t), decimals)
    };
}

function vectorPayload(vector) {
    return { x: vector.x, y: vector.y, z: vector.z };
}

function failMotion(state, result, message) {
    state.diagnostics.motionFailedCount += 1;
    return failCommand(state, result, message, "WebGL scene motion failed.");
}

function notifyMotionCompleted(state, motion) {
    const result = createCommandResult(state, "motion-completed", motion.motionId);
    result.affectedObjectIds.push(motion.objectId);
    state.dotNetRef?.invokeMethodAsync("OnMotionCompleted", JSON.stringify(completeCommandResult(state, result)))
        .catch(error => console.warn("WebGL scene motion completion callback failed.", error));
}

function nextMotionId(state, objectId) {
    if (state.options?.deterministicMode !== false) {
        state.nextMotionSequence = (state.nextMotionSequence || 0) + 1;
        return `${objectId}:motion:${state.nextMotionSequence}`;
    }

    return `${objectId}:motion:${Date.now()}`;
}
