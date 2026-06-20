import { buildDiagnosticsSnapshot } from "./02-webgl-scene-core.js";
import {
    buildRuntimeIdleState,
    shouldTreatFinalScheduledRenderAsDrained,
    syncRuntimeIdleDiagnostics
} from "./41-webgl-scene-runtime-idle-state.js";

export function isRuntimeIdle(state) {
    const idleState = buildRuntimeIdleState(state);
    syncRuntimeIdleDiagnostics(state, idleState);
    return {
        ...idleState,
        diagnostics: buildDiagnosticsSnapshot(state)
    };
}

export async function waitForRuntimeIdle(state, options = {}) {
    const timeoutMs = clampPositive(options.timeoutMs, 2000, 60000);
    const pollIntervalMs = clampPositive(options.pollIntervalMs, 16, 1000);
    const reason = normalizeReason(options.reason, "runtime-idle");
    const policyMode = normalizePolicyMode(options.policyMode);
    const startedAt = performance.now();
    let consecutiveSemanticIdleProbes = 0;
    let lastState = null;

    while (performance.now() - startedAt < timeoutMs) {
        lastState = buildRuntimeIdleProbe(state, policyMode);
        consecutiveSemanticIdleProbes = lastState.semanticIdle
            ? consecutiveSemanticIdleProbes + 1
            : 0;

        if (lastState.idle) {
            break;
        }

        if (shouldTreatFinalScheduledRenderAsDrained(lastState, consecutiveSemanticIdleProbes, policyMode)) {
            markFinalRenderDrained(state);
            lastState = buildRuntimeIdleProbe(state, policyMode);
            break;
        }

        await delay(pollIntervalMs);
    }

    lastState ??= buildRuntimeIdleProbe(state, policyMode);
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
        policyMode,
        finalRenderDrainAllowed: lastState.finalRenderDrainAllowed,
        visualIdleRequired: lastState.visualIdleRequired,
        timeoutMs,
        pollIntervalMs,
        elapsedMs,
        blockers: lastState.blockers,
        semanticIdle: lastState.semanticIdle,
        visualIdle: lastState.visualIdle,
        finalRenderDrained: lastState.finalRenderDrained,
        semanticBlockers: lastState.semanticBlockers,
        visualBlockers: lastState.visualBlockers,
        diagnostics: lastState.diagnostics
    };
}

export function collectRuntimeIdleBlockers(state) {
    return buildRuntimeIdleState(state).blockers;
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

function normalizePolicyMode(value) {
    const text = String(value || "").trim();
    if (text === "semanticOnly" || text === "visualStrict" || text === "allowFinalRenderDrain") {
        return text;
    }

    return "allowFinalRenderDrain";
}

function buildRuntimeIdleProbe(state, policyMode) {
    const idleState = buildRuntimeIdleState(state, { policyMode });
    syncRuntimeIdleDiagnostics(state, idleState);
    return {
        ...idleState,
        diagnostics: buildDiagnosticsSnapshot(state)
    };
}

function delay(milliseconds) {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
}

function markFinalRenderDrained(state) {
    if (state?.diagnostics) {
        state.diagnostics.finalRenderDrained = true;
    }
}

function syncLastRuntimeStopIdleDiagnostics(state, idleState, elapsedMs) {
    if (!state?.diagnostics || (state.diagnostics.runtimeStopCount || 0) <= 0) {
        return;
    }

    state.diagnostics.lastRuntimeStopIdle = idleState.idle === true;
    state.diagnostics.lastRuntimeStopTimedOut = idleState.idle !== true;
    state.diagnostics.lastRuntimeStopIdleElapsedMs = elapsedMs;
    state.diagnostics.lastRuntimeStopBlockers = [...(idleState.blockers || [])];
    syncRuntimeIdleDiagnostics(state, idleState);
}
