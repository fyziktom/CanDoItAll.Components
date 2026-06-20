export const stageBarrierPolicies = Object.freeze({
    none: "none",
    waitSeconds: "wait-seconds",
    waitForActiveMotions: "wait-for-active-motions",
    waitForObjectMotions: "wait-for-object-motions",
    waitForRenderIdle: "wait-for-render-idle",
    waitForEvent: "wait-for-event"
});

export function createStageBarrier(state, item) {
    const stage = item?.stage || {};
    const waitSeconds = Math.max(0, Number(stage.waitSeconds) || 0);
    const policy = resolveStageBarrierPolicy(stage, waitSeconds);
    if (policy === stageBarrierPolicies.none) {
        return null;
    }

    if (policy === stageBarrierPolicies.waitSeconds && waitSeconds <= 0) {
        return null;
    }

    const eventId = resolveBarrierEventId(stage, policy);
    return {
        policy,
        batchId: item?.batchId || "",
        stageId: stage.stageId || "",
        remainingSeconds: policy === stageBarrierPolicies.waitSeconds ? waitSeconds : 0,
        elapsedSeconds: 0,
        timeoutSeconds: resolveBarrierTimeoutSeconds(state, stage),
        timedOut: false,
        objectIds: resolveBarrierObjectIds(stage),
        eventId,
        idleFrameCount: 0,
        signaled: false
    };
}

export function resolveStageBarrierPolicy(stage, waitSeconds = Number(stage?.waitSeconds) || 0) {
    const raw = String(
        stage?.barrierPolicy ||
        stage?.BarrierPolicy ||
        stage?.metadata?.barrierPolicy ||
        stage?.metadata?.stageBarrierPolicy ||
        ""
    ).trim().toLowerCase();
    switch (raw.replaceAll("_", "-")) {
        case "":
            return waitSeconds > 0 ? stageBarrierPolicies.waitSeconds : stageBarrierPolicies.none;
        case "none":
        case "no-barrier":
            return stageBarrierPolicies.none;
        case "wait-seconds":
        case "waitseconds":
        case "time-delay":
        case "timedelay":
            return stageBarrierPolicies.waitSeconds;
        case "wait-for-active-motions":
        case "waitforactivemotions":
            return stageBarrierPolicies.waitForActiveMotions;
        case "wait-for-object-motions":
        case "waitforobjectmotions":
            return stageBarrierPolicies.waitForObjectMotions;
        case "wait-for-render-idle":
        case "waitforrenderidle":
            return stageBarrierPolicies.waitForRenderIdle;
        case "wait-for-event":
        case "waitforevent":
        case "manual-step":
        case "manualstep":
            return stageBarrierPolicies.waitForEvent;
        default:
            return raw;
    }
}

export function updateStageBarrier(state, barrier, deltaSeconds) {
    if (!barrier) {
        return;
    }

    barrier.elapsedSeconds = roundWait((barrier.elapsedSeconds || 0) + Math.max(0, Number(deltaSeconds) || 0));
    if (barrier.policy === stageBarrierPolicies.waitSeconds) {
        barrier.remainingSeconds = Math.max(0, barrier.remainingSeconds - Math.max(0, Number(deltaSeconds) || 0));
    } else if (barrier.policy === stageBarrierPolicies.waitForRenderIdle && !hasAnyMotion(state)) {
        barrier.idleFrameCount += 1;
    }
}

export function isStageBarrierReady(state, barrier) {
    return describeStageBarrier(state, barrier).isReady;
}

export function describeStageBarrier(state, barrier) {
    if (!barrier) {
        return { isReady: true, target: "", blockers: [] };
    }

    const description = describeStageBarrierCore(state, barrier);
    if (!description.isReady &&
        barrier.timeoutSeconds > 0 &&
        (barrier.elapsedSeconds || 0) >= barrier.timeoutSeconds) {
        barrier.timedOut = true;
        return {
            isReady: true,
            target: description.target,
            blockers: [`timeout:${roundWait(barrier.elapsedSeconds)}/${roundWait(barrier.timeoutSeconds)}`, ...description.blockers],
            warning: `timeout:${barrier.stageId}`
        };
    }

    return description;
}

function describeStageBarrierCore(state, barrier) {
    switch (barrier.policy) {
        case stageBarrierPolicies.waitSeconds:
            return barrier.remainingSeconds <= 0
                ? { isReady: true, target: "seconds", blockers: [] }
                : { isReady: false, target: "seconds", blockers: [`seconds:${roundWait(barrier.remainingSeconds)}`] };
        case stageBarrierPolicies.waitForActiveMotions:
            return describeMotionBarrier(state, "active-or-queued-motions");
        case stageBarrierPolicies.waitForObjectMotions:
            return describeObjectMotionBarrier(state, barrier.objectIds);
        case stageBarrierPolicies.waitForRenderIdle:
            if (hasAnyMotion(state)) {
                return { isReady: false, target: "render-idle", blockers: ["motion"] };
            }

            return barrier.idleFrameCount > 0
                ? { isReady: true, target: "render-idle", blockers: [] }
                : { isReady: false, target: "render-idle", blockers: ["idle-frame"] };
        case stageBarrierPolicies.waitForEvent:
            if (!barrier.eventId) {
                return { isReady: false, target: "event", blockers: ["missing-event-id"] };
            }

            return barrier.signaled
                ? { isReady: true, target: barrier.eventId, blockers: [] }
                : { isReady: false, target: barrier.eventId, blockers: [`event:${barrier.eventId}`] };
        default:
            return {
                isReady: true,
                target: barrier.policy || "unknown",
                blockers: [`unknown-policy:${barrier.policy || ""}`],
                warning: `unknown-policy:${barrier.policy || ""}`
            };
    }
}

export function hasAutomaticStageBarrierWork(state, runner) {
    if (!runner || runner.cancelled) {
        return false;
    }

    if (!runner.activeBarrier) {
        return runner.queue.length > 0;
    }

    if (runner.activeBarrier.policy === stageBarrierPolicies.waitForEvent &&
        !isStageBarrierReady(state, runner.activeBarrier)) {
        return false;
    }

    return true;
}

export function signalStageBarrierEvent(state, eventId) {
    const key = String(eventId || "").trim();
    if (!key) {
        return false;
    }

    const barrier = state.commandStageRunner?.activeBarrier;
    if (barrier?.policy === stageBarrierPolicies.waitForEvent && barrier.eventId === key) {
        barrier.signaled = true;
        return true;
    }

    return false;
}

function describeMotionBarrier(state, target) {
    const blockers = [];
    for (const motionId of state.motions?.keys?.() || []) {
        blockers.push(`active:${motionId}`);
    }

    for (const motionId of queuedMotionIds(state)) {
        blockers.push(`queued:${motionId}`);
    }

    return blockers.length === 0
        ? { isReady: true, target, blockers: [] }
        : { isReady: false, target, blockers };
}

function describeObjectMotionBarrier(state, objectIds) {
    if (!objectIds?.length) {
        return { isReady: true, target: "object-motions", blockers: ["missing-object-id"], warning: "missing-object-id" };
    }

    const objectSet = new Set(objectIds);
    const blockers = [];
    for (const motion of state.motions?.values?.() || []) {
        if (objectSet.has(motion.objectId)) {
            blockers.push(`active:${motion.objectId}`);
        }
    }

    for (const objectId of objectSet) {
        if ((state.motionQueuesByObjectId?.get?.(objectId)?.length || 0) > 0) {
            blockers.push(`queued:${objectId}`);
        }
    }

    return blockers.length === 0
        ? { isReady: true, target: objectIds.join(","), blockers: [] }
        : { isReady: false, target: objectIds.join(","), blockers };
}

function resolveBarrierObjectIds(stage) {
    const explicit = stage?.barrierObjectIds || stage?.BarrierObjectIds || stage?.metadata?.barrierObjectIds;
    const values = Array.isArray(explicit)
        ? explicit
        : String(explicit || "").split(/[;,]/);
    const objectIds = values.map(value => String(value || "").trim()).filter(Boolean);
    if (objectIds.length > 0) {
        return objectIds;
    }

    return Array.from(new Set((stage?.motions || []).map(motion => motion?.objectId).filter(Boolean)));
}

function resolveBarrierEventId(stage, policy) {
    const explicit = stage?.barrierEventId ||
        stage?.BarrierEventId ||
        stage?.eventId ||
        stage?.metadata?.barrierEventId ||
        stage?.metadata?.eventId;
    if (explicit) {
        return String(explicit).trim();
    }

    const rawPolicy = String(stage?.barrierPolicy || stage?.metadata?.barrierPolicy || "").toLowerCase();
    return rawPolicy.includes("manual") && policy === stageBarrierPolicies.waitForEvent ? "manual-step" : "";
}

function resolveBarrierTimeoutSeconds(state, stage) {
    const explicit = stage?.barrierTimeoutSeconds ||
        stage?.BarrierTimeoutSeconds ||
        stage?.metadata?.barrierTimeoutSeconds ||
        stage?.metadata?.stageBarrierTimeoutSeconds;
    const configured = explicit ?? state?.options?.commandStageBarrierTimeoutSeconds ?? 0;
    const timeout = Number(configured);
    return Number.isFinite(timeout) && timeout > 0 ? timeout : 0;
}

function hasAnyMotion(state) {
    return (state.motions?.size || 0) > 0 || queuedMotionIds(state).length > 0;
}

function queuedMotionIds(state) {
    const ids = [];
    for (const queue of state.motionQueuesByObjectId?.values?.() || []) {
        ids.push(...queue.map(motion => motion.motionId || "").filter(Boolean));
    }

    return ids;
}

function roundWait(value) {
    return Math.round((Number(value) || 0) * 1000) / 1000;
}
