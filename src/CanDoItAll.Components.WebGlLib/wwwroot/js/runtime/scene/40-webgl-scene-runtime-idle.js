import { buildDiagnosticsSnapshot } from "./02-webgl-scene-core.js";
import { hasAutomaticStageBarrierWork } from "./32-webgl-scene-stage-barriers.js";

export function isRuntimeIdle(state) {
    const blockers = collectRuntimeIdleBlockers(state);
    return {
        idle: blockers.length === 0,
        blockers,
        diagnostics: buildDiagnosticsSnapshot(state)
    };
}

export async function waitForRuntimeIdle(state, options = {}) {
    const timeoutMs = clampPositive(options.timeoutMs, 2000, 60000);
    const pollIntervalMs = clampPositive(options.pollIntervalMs, 16, 1000);
    const reason = normalizeReason(options.reason, "runtime-idle");
    const startedAt = performance.now();
    let lastState = isRuntimeIdle(state);

    while (!lastState.idle && performance.now() - startedAt < timeoutMs) {
        await delay(pollIntervalMs);
        lastState = isRuntimeIdle(state);
    }

    const elapsedMs = Math.round(performance.now() - startedAt);
    syncLastRuntimeStopIdleDiagnostics(state, lastState, elapsedMs);
    lastState = {
        ...lastState,
        diagnostics: buildDiagnosticsSnapshot(state)
    };
    return {
        success: lastState.idle,
        idle: lastState.idle,
        timedOut: !lastState.idle,
        reason,
        timeoutMs,
        pollIntervalMs,
        elapsedMs,
        blockers: lastState.blockers,
        diagnostics: lastState.diagnostics
    };
}

export function collectRuntimeIdleBlockers(state) {
    const blockers = [];
    if (!state) {
        blockers.push("runtime:not-created");
        return blockers;
    }

    const queuedMotionCount = countQueuedMotions(state);
    const queuedStageCount = state.commandStageRunner?.queue?.length || 0;
    const hasActiveBarrier = !!state.commandStageRunner?.activeBarrier;
    const hasCurrentStage = !!state.commandStageRunner?.currentStageId;
    const hasAutomaticStageWork = hasAutomaticStageBarrierWork(state, state.commandStageRunner);

    addCountBlocker(blockers, "motion:active", state.motions?.size || 0);
    addCountBlocker(blockers, "motion:queued", queuedMotionCount);
    addCountBlocker(blockers, "command-stage:queued", queuedStageCount);
    if (hasActiveBarrier) {
        blockers.push("command-stage:barrier");
    }

    if (state.commandStageRunner && !state.commandStageRunner.cancelled && hasCurrentStage) {
        blockers.push("command-stage:active");
    }

    if (hasAutomaticStageWork && queuedStageCount === 0 && !hasActiveBarrier && !hasCurrentStage) {
        blockers.push("command-stage:automatic-work");
    }

    addCountBlocker(blockers, "asset-cache:pending-disposal", state.diagnostics?.assetCachePendingDisposalCount || 0);
    if (state.isRenderingFrame) {
        blockers.push("render-loop:frame-active");
    }

    if (state.animationHandle || state.diagnostics?.isRenderLoopActive) {
        blockers.push("render-loop:scheduled");
    }

    if (state.options?.renderMode === "continuous") {
        blockers.push("render-loop:continuous-mode");
    }

    return blockers;
}

function addCountBlocker(blockers, name, count) {
    if (count > 0) {
        blockers.push(`${name}:${count}`);
    }
}

function countQueuedMotions(state) {
    let count = 0;
    for (const queue of state.motionQueuesByObjectId?.values?.() || []) {
        count += queue?.length || 0;
    }

    return count;
}

function clampPositive(value, fallback, max) {
    const number = Number(value);
    if (!Number.isFinite(number) || number <= 0) {
        return fallback;
    }

    return Math.min(Math.round(number), max);
}

function normalizeReason(value, fallback) {
    const text = String(value || "").trim();
    return text || fallback;
}

function delay(milliseconds) {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
}

function syncLastRuntimeStopIdleDiagnostics(state, idleState, elapsedMs) {
    if (!state?.diagnostics || (state.diagnostics.runtimeStopCount || 0) <= 0) {
        return;
    }

    state.diagnostics.lastRuntimeStopIdle = idleState.idle === true;
    state.diagnostics.lastRuntimeStopTimedOut = idleState.idle !== true;
    state.diagnostics.lastRuntimeStopIdleElapsedMs = elapsedMs;
    state.diagnostics.lastRuntimeStopBlockers = [...(idleState.blockers || [])];
}
