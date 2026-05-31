const barrierPolicies = Object.freeze({
    timeDelay: "time-delay",
    waitForActiveMotions: "wait-for-active-motions",
    waitForObjectMotions: "wait-for-object-motions",
    waitForRenderIdle: "wait-for-render-idle",
    manualStep: "manual-step"
});

export function enqueueCommandStages(state, batchId, stages, applyStage) {
    if (!Array.isArray(stages) || stages.length === 0) {
        syncStageDiagnostics(state);
        return;
    }

    state.commandStageRunner ??= createCommandStageRunner();
    const runner = state.commandStageRunner;
    runner.cancelled = false;
    for (const stage of stages) {
        runner.queue.push({
            batchId: batchId || "command-batch",
            stage
        });
    }

    syncStageDiagnostics(state);
    drainReadyStages(state, applyStage);
}

export function advanceCommandStageRunner(state, deltaSeconds, applyStage) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return;
    }

    updateActiveBarrier(state, runner, deltaSeconds);
    drainReadyStages(state, applyStage);

    if (hasPendingCommandStageRunnerWork(state)) {
        state.scheduleRender("command-stage");
    }
}

export function requestCommandStageManualStep(state) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return false;
    }

    runner.manualStepRequested = true;
    syncStageDiagnostics(state);
    state.scheduleRender("command-stage");
    return true;
}

export function cancelCommandStageRunner(state, reason = "cancelled") {
    const runner = state.commandStageRunner;
    if (!runner) {
        return;
    }

    runner.cancelled = true;
    runner.queue.length = 0;
    runner.activeBarrier = null;
    runner.manualStepRequested = false;
    runner.currentBatchId = "";
    runner.currentStageId = "";
    state.diagnostics.commandStageCancelledCount = (state.diagnostics.commandStageCancelledCount || 0) + 1;
    state.diagnostics.lastStageCancelReason = reason;
    syncStageDiagnostics(state);
}

export function hasPendingCommandStageRunnerWork(state) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return false;
    }

    if (runner.activeBarrier?.policy === barrierPolicies.manualStep && !runner.manualStepRequested) {
        return false;
    }

    return runner.queue.length > 0 || !!runner.activeBarrier;
}

export function syncStageDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    const runner = state.commandStageRunner || createCommandStageRunner();
    state.diagnostics.currentCommandBatchId = runner.currentBatchId || "";
    state.diagnostics.currentCommandStageId = runner.currentStageId || "";
    state.diagnostics.completedCommandStageCount = runner.completedStageIds?.length || 0;
    state.diagnostics.failedCommandStageCount = runner.failedStageIds?.length || 0;
    state.diagnostics.skippedCommandStageCount = runner.skippedStageIds?.length || 0;
    state.diagnostics.queuedCommandStageCount = runner.queue?.length || 0;
    state.diagnostics.commandStageWaitSeconds = roundWait(runner.activeBarrier?.remainingSeconds || 0);
    state.diagnostics.commandStageBarrierPolicy = runner.activeBarrier?.policy || "";
    state.diagnostics.commandStageBarrierObjectIds = [...(runner.activeBarrier?.objectIds || [])];
    state.diagnostics.completedCommandStageIds = [...(runner.completedStageIds || [])];
    state.diagnostics.failedCommandStageIds = [...(runner.failedStageIds || [])];
    state.diagnostics.skippedCommandStageIds = [...(runner.skippedStageIds || [])];
    state.diagnostics.lastStageError = runner.lastStageError || "";
    state.diagnostics.commandStageResultLog = [...(runner.resultLog || [])];
    state.diagnostics.commandStageQueueSnapshot = (runner.queue || []).map(item => ({
        batchId: item.batchId || "",
        stageId: item.stage?.stageId || "",
        barrierPolicy: resolveBarrierPolicy(item.stage),
        waitSeconds: roundWait(Number(item.stage?.waitSeconds) || 0)
    }));
}

function drainReadyStages(state, applyStage) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return;
    }

    while (isActiveBarrierReady(state, runner) && runner.queue.length > 0 && !runner.cancelled) {
        clearActiveBarrier(runner);
        const item = runner.queue.shift();
        runner.currentBatchId = item.batchId;
        runner.currentStageId = item.stage?.stageId || "";
        if (!item.stage) {
            runner.skippedStageIds.push("");
            appendStageResult(runner, item, "skipped", "Stage payload was missing.");
            syncStageDiagnostics(state);
            continue;
        }

        try {
            applyStage(item.stage);
            runner.completedStageIds.push(runner.currentStageId);
            appendStageResult(runner, item, "completed", "");
        } catch (error) {
            const message = error?.message || String(error);
            runner.failedStageIds.push(runner.currentStageId);
            runner.lastStageError = message;
            state.diagnostics.lastError = message;
            appendStageResult(runner, item, "failed", message);
        }

        runner.activeBarrier = createBarrier(item.stage);
        syncStageDiagnostics(state);
    }

    if (runner.queue.length === 0 && isActiveBarrierReady(state, runner)) {
        clearActiveBarrier(runner);
        runner.currentBatchId = "";
        runner.currentStageId = "";
        syncStageDiagnostics(state);
    }
}

function updateActiveBarrier(state, runner, deltaSeconds) {
    const barrier = runner.activeBarrier;
    if (!barrier) {
        return;
    }

    if (barrier.policy === barrierPolicies.timeDelay) {
        barrier.remainingSeconds = Math.max(0, barrier.remainingSeconds - Math.max(0, Number(deltaSeconds) || 0));
    } else if (barrier.policy === barrierPolicies.waitForRenderIdle && !hasAnyMotion(state)) {
        barrier.idleFrameCount += 1;
    }

    syncStageDiagnostics(state);
}

function isActiveBarrierReady(state, runner) {
    const barrier = runner.activeBarrier;
    if (!barrier) {
        return true;
    }

    switch (barrier.policy) {
        case barrierPolicies.timeDelay:
            return barrier.remainingSeconds <= 0;
        case barrierPolicies.waitForActiveMotions:
            return !hasAnyMotion(state);
        case barrierPolicies.waitForObjectMotions:
            return !hasObjectMotion(state, barrier.objectIds);
        case barrierPolicies.waitForRenderIdle:
            return !hasAnyMotion(state) && barrier.idleFrameCount > 0;
        case barrierPolicies.manualStep:
            return runner.manualStepRequested === true;
        default:
            runner.lastStageError = `Unknown stage barrier policy '${barrier.policy}'.`;
            return true;
    }
}

function clearActiveBarrier(runner) {
    if (runner.activeBarrier?.policy === barrierPolicies.manualStep) {
        runner.manualStepRequested = false;
    }

    runner.activeBarrier = null;
}

function createBarrier(stage) {
    const policy = resolveBarrierPolicy(stage);
    const waitSeconds = Math.max(0, Number(stage?.waitSeconds) || 0);
    if (!policy && waitSeconds <= 0) {
        return null;
    }

    const resolvedPolicy = policy || barrierPolicies.timeDelay;
    return {
        policy: resolvedPolicy,
        remainingSeconds: resolvedPolicy === barrierPolicies.timeDelay ? waitSeconds : 0,
        objectIds: resolveBarrierObjectIds(stage),
        idleFrameCount: 0
    };
}

function resolveBarrierPolicy(stage) {
    return String(
        stage?.barrierPolicy ||
        stage?.BarrierPolicy ||
        stage?.metadata?.barrierPolicy ||
        stage?.metadata?.stageBarrierPolicy ||
        ""
    ).trim().toLowerCase();
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

function hasAnyMotion(state) {
    if ((state.motions?.size || 0) > 0) {
        return true;
    }

    for (const queue of state.motionQueuesByObjectId?.values?.() || []) {
        if (queue.length > 0) {
            return true;
        }
    }

    return false;
}

function hasObjectMotion(state, objectIds) {
    if (!objectIds?.length) {
        return hasAnyMotion(state);
    }

    const objectSet = new Set(objectIds);
    for (const motion of state.motions?.values?.() || []) {
        if (objectSet.has(motion.objectId)) {
            return true;
        }
    }

    for (const objectId of objectSet) {
        if ((state.motionQueuesByObjectId?.get?.(objectId)?.length || 0) > 0) {
            return true;
        }
    }

    return false;
}

function appendStageResult(runner, item, status, error) {
    runner.resultLog.push({
        batchId: item.batchId || "",
        stageId: item.stage?.stageId || "",
        status,
        barrierPolicy: resolveBarrierPolicy(item.stage),
        error: error || ""
    });
    if (runner.resultLog.length > 100) {
        runner.resultLog.splice(0, runner.resultLog.length - 100);
    }
}

function createCommandStageRunner() {
    return {
        queue: [],
        activeBarrier: null,
        manualStepRequested: false,
        currentBatchId: "",
        currentStageId: "",
        completedStageIds: [],
        failedStageIds: [],
        skippedStageIds: [],
        resultLog: [],
        lastStageError: "",
        cancelled: false
    };
}

function roundWait(value) {
    return Math.round((Number(value) || 0) * 1000) / 1000;
}
