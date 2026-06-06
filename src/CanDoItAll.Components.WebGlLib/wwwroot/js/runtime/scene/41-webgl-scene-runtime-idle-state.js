import { hasAutomaticStageBarrierWork } from "./32-webgl-scene-stage-barriers.js";

const scheduledRenderBlocker = "render-loop:scheduled";
const semanticOnlyPolicyMode = "semanticOnly";
const visualStrictPolicyMode = "visualStrict";
const allowFinalRenderDrainPolicyMode = "allowFinalRenderDrain";

export function buildRuntimeIdleState(state, options = {}) {
    const policyMode = normalizePolicyMode(options.policyMode);
    const finalRenderDrainAllowed = policyMode === allowFinalRenderDrainPolicyMode;
    const visualIdleRequired = policyMode !== semanticOnlyPolicyMode;
    if (!state) {
        return {
            idle: false,
            semanticIdle: false,
            visualIdle: false,
            finalRenderDrained: false,
            policyMode,
            finalRenderDrainAllowed,
            visualIdleRequired,
            blockers: ["runtime:not-created"],
            semanticBlockers: ["runtime:not-created"],
            visualBlockers: []
        };
    }

    const semanticBlockers = collectSemanticRuntimeIdleBlockers(state);
    const rawVisualBlockers = collectVisualRuntimeIdleBlockers(state);
    const semanticIdle = semanticBlockers.length === 0;
    const explicitFinalRenderDrained = options.finalRenderDrained === true ||
        state.diagnostics?.finalRenderDrained === true;
    const finalScheduledRenderDrained = semanticIdle &&
        finalRenderDrainAllowed &&
        explicitFinalRenderDrained &&
        hasOnlyFinalScheduledRenderBlockers(rawVisualBlockers);
    const visualIdle = rawVisualBlockers.length === 0 || finalScheduledRenderDrained;
    const finalRenderDrained = semanticIdle && (rawVisualBlockers.length === 0 || finalScheduledRenderDrained);
    const visualBlockers = visualIdle ? [] : rawVisualBlockers;
    const idle = semanticIdle && (!visualIdleRequired || visualIdle);

    return {
        idle,
        semanticIdle,
        visualIdle,
        finalRenderDrained,
        policyMode,
        finalRenderDrainAllowed,
        visualIdleRequired,
        blockers: visualIdleRequired ? [...semanticBlockers, ...visualBlockers] : semanticBlockers,
        semanticBlockers,
        visualBlockers,
        rawVisualBlockers
    };
}

export function collectSemanticRuntimeIdleBlockers(state) {
    const blockers = [];
    if (!state) {
        blockers.push("runtime:not-created");
        return blockers;
    }

    const queuedMotionCount = countQueuedMotions(state);
    const runner = state.commandStageRunner;
    const queuedStageCount = runner?.queue?.length || 0;
    const hasActiveBarrier = !!runner?.activeBarrier;
    const hasCurrentStage = !!runner?.currentStageId;
    const hasAutomaticStageWork = hasAutomaticStageBarrierWork(state, runner);

    addCountBlocker(blockers, "motion:active", state.motions?.size || 0);
    addCountBlocker(blockers, "motion:queued", queuedMotionCount);
    addCountBlocker(blockers, "command-stage:queued", queuedStageCount);
    if (hasActiveBarrier) {
        blockers.push("command-stage:barrier");
    }

    if (runner && !runner.cancelled && hasCurrentStage) {
        blockers.push("command-stage:active");
    }

    if (hasAutomaticStageWork && queuedStageCount === 0 && !hasActiveBarrier && !hasCurrentStage) {
        blockers.push("command-stage:automatic-work");
    }

    addCountBlocker(blockers, "asset-cache:pending-disposal", state.diagnostics?.assetCachePendingDisposalCount || 0);
    return blockers;
}

export function collectVisualRuntimeIdleBlockers(state) {
    const blockers = [];
    if (!state) {
        return blockers;
    }

    if (state.isRenderingFrame) {
        blockers.push("render-loop:frame-active");
    }

    if (state.animationHandle || state.diagnostics?.isRenderLoopActive) {
        blockers.push(scheduledRenderBlocker);
    }

    if (state.options?.renderMode === "continuous") {
        blockers.push("render-loop:continuous-mode");
    }

    return blockers;
}

export function hasOnlyFinalScheduledRenderBlockers(blockers) {
    return Array.isArray(blockers) &&
        blockers.length > 0 &&
        blockers.every(blocker => blocker === scheduledRenderBlocker);
}

export function shouldTreatFinalScheduledRenderAsDrained(idleState, consecutiveSemanticIdleProbes, policyMode = allowFinalRenderDrainPolicyMode) {
    return idleState?.semanticIdle === true &&
        normalizePolicyMode(policyMode) === allowFinalRenderDrainPolicyMode &&
        idleState.visualIdle !== true &&
        consecutiveSemanticIdleProbes >= 2 &&
        hasOnlyFinalScheduledRenderBlockers(idleState.rawVisualBlockers || idleState.visualBlockers);
}

export function syncRuntimeIdleDiagnostics(state, idleState = null) {
    if (!state?.diagnostics) {
        return;
    }

    const resolved = idleState || buildRuntimeIdleState(state);
    state.diagnostics.semanticIdle = resolved.semanticIdle === true;
    state.diagnostics.visualIdle = resolved.visualIdle === true;
    state.diagnostics.finalRenderDrained = resolved.finalRenderDrained === true;
    state.diagnostics.runtimeIdlePolicyMode = resolved.policyMode || allowFinalRenderDrainPolicyMode;
    state.diagnostics.runtimeIdleFinalRenderDrainAllowed = resolved.finalRenderDrainAllowed === true;
    state.diagnostics.runtimeIdleVisualRequired = resolved.visualIdleRequired !== false;
}

function normalizePolicyMode(value) {
    const text = String(value || "").trim();
    if (text === semanticOnlyPolicyMode || text === visualStrictPolicyMode || text === allowFinalRenderDrainPolicyMode) {
        return text;
    }

    return allowFinalRenderDrainPolicyMode;
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
