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

    if (runner.waitSeconds > 0) {
        runner.waitSeconds = Math.max(0, runner.waitSeconds - Math.max(0, Number(deltaSeconds) || 0));
    }

    if (runner.waitSeconds <= 0) {
        drainReadyStages(state, applyStage);
    }

    if (runner.queue.length || runner.waitSeconds > 0) {
        state.scheduleRender("command-stage");
    }
}

export function cancelCommandStageRunner(state, reason = "cancelled") {
    const runner = state.commandStageRunner;
    if (!runner) {
        return;
    }

    runner.cancelled = true;
    runner.queue.length = 0;
    runner.waitSeconds = 0;
    runner.currentBatchId = "";
    runner.currentStageId = "";
    state.diagnostics.commandStageCancelledCount = (state.diagnostics.commandStageCancelledCount || 0) + 1;
    state.diagnostics.lastStageCancelReason = reason;
    syncStageDiagnostics(state);
}

export function syncStageDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    const runner = state.commandStageRunner || createCommandStageRunner();
    state.diagnostics.currentCommandBatchId = runner.currentBatchId || "";
    state.diagnostics.currentCommandStageId = runner.currentStageId || "";
    state.diagnostics.completedCommandStageCount = runner.completedCount || 0;
    state.diagnostics.failedCommandStageCount = runner.failedCount || 0;
    state.diagnostics.queuedCommandStageCount = runner.queue?.length || 0;
    state.diagnostics.commandStageWaitSeconds = roundWait(runner.waitSeconds || 0);
}

function drainReadyStages(state, applyStage) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return;
    }

    while (runner.waitSeconds <= 0 && runner.queue.length > 0 && !runner.cancelled) {
        const item = runner.queue.shift();
        runner.currentBatchId = item.batchId;
        runner.currentStageId = item.stage?.stageId || "";
        try {
            applyStage(item.stage);
            runner.completedCount += 1;
        } catch (error) {
            runner.failedCount += 1;
            state.diagnostics.lastError = error?.message || String(error);
        }

        runner.waitSeconds = Math.max(0, Number(item.stage?.waitSeconds) || 0);
        syncStageDiagnostics(state);
        if (runner.waitSeconds > 0) {
            break;
        }
    }

    if (runner.queue.length === 0 && runner.waitSeconds <= 0) {
        runner.currentBatchId = "";
        runner.currentStageId = "";
        syncStageDiagnostics(state);
    }
}

function createCommandStageRunner() {
    return {
        queue: [],
        waitSeconds: 0,
        currentBatchId: "",
        currentStageId: "",
        completedCount: 0,
        failedCount: 0,
        cancelled: false
    };
}

function roundWait(value) {
    return Math.round((Number(value) || 0) * 1000) / 1000;
}

