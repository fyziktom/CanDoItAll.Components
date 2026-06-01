import {
    createStageBarrier,
    describeStageBarrier,
    hasAutomaticStageBarrierWork,
    isStageBarrierReady,
    resolveStageBarrierPolicy,
    signalStageBarrierEvent,
    updateStageBarrier
} from "./32-webgl-scene-stage-barriers.js";
import {
    appendCommandStageJournal,
    resetCommandStageJournal,
    syncCommandJournalDiagnostics
} from "./33-webgl-scene-command-journal.js";

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

    updateStageBarrier(state, runner.activeBarrier, deltaSeconds);
    drainReadyStages(state, applyStage);

    if (hasPendingCommandStageRunnerWork(state)) {
        state.scheduleRender("command-stage");
    }
}

export function requestCommandStageManualStep(state) {
    return signalCommandStageEvent(state, "manual-step");
}

export function signalCommandStageEvent(state, eventId) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return false;
    }

    const accepted = signalStageBarrierEvent(state, eventId);
    syncStageDiagnostics(state);
    if (accepted) {
        state.scheduleRender("command-stage");
    }

    return accepted;
}

export function cancelCommandStageRunner(state, reason = "cancelled") {
    const runner = state.commandStageRunner;
    if (!runner) {
        return;
    }

    runner.cancelled = true;
    runner.queue.length = 0;
    runner.activeBarrier = null;
    runner.currentBatchId = "";
    runner.currentStageId = "";
    runner.completedStageIds.length = 0;
    runner.failedStageIds.length = 0;
    runner.skippedStageIds.length = 0;
    runner.resultLog.length = 0;
    runner.resultSequence = 0;
    runner.lastStageError = "";
    resetCommandStageJournal(state);
    state.diagnostics.commandStageCancelledCount = (state.diagnostics.commandStageCancelledCount || 0) + 1;
    state.diagnostics.lastStageCancelReason = reason;
    state.diagnostics.commandStageBarrierTimedOut = false;
    state.diagnostics.lastStageBarrierWarning = "";
    appendCommandStageJournal(state, {
        eventKind: "stage-warning",
        status: "cancelled",
        message: reason
    });
    syncStageDiagnostics(state);
}

export function hasPendingCommandStageRunnerWork(state) {
    return hasAutomaticStageBarrierWork(state, state.commandStageRunner);
}

export function syncStageDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    const runner = state.commandStageRunner || createCommandStageRunner();
    const barrier = describeStageBarrier(state, runner.activeBarrier);
    state.diagnostics.currentCommandBatchId = runner.currentBatchId || "";
    state.diagnostics.currentCommandStageId = runner.currentStageId || "";
    state.diagnostics.completedCommandStageCount = runner.completedStageIds?.length || 0;
    state.diagnostics.failedCommandStageCount = runner.failedStageIds?.length || 0;
    state.diagnostics.skippedCommandStageCount = runner.skippedStageIds?.length || 0;
    state.diagnostics.queuedCommandStageCount = runner.queue?.length || 0;
    state.diagnostics.commandStageWaitSeconds = roundWait(runner.activeBarrier?.remainingSeconds || 0);
    state.diagnostics.commandStageBarrierPolicy = runner.activeBarrier?.policy || "";
    state.diagnostics.commandStageBarrierTarget = barrier.target || "";
    state.diagnostics.commandStageBarrierBlockers = barrier.blockers || [];
    state.diagnostics.commandStageBarrierElapsedSeconds = roundWait(runner.activeBarrier?.elapsedSeconds || 0);
    state.diagnostics.commandStageBarrierTimeoutSeconds = roundWait(runner.activeBarrier?.timeoutSeconds || 0);
    if (runner.activeBarrier) {
        state.diagnostics.commandStageBarrierTimedOut = runner.activeBarrier.timedOut === true;
    }

    if (barrier.warning) {
        state.diagnostics.lastStageBarrierWarning = barrier.warning;
    }

    state.diagnostics.commandStageBarrierEventId = runner.activeBarrier?.eventId || "";
    state.diagnostics.commandStageBarrierObjectIds = [...(runner.activeBarrier?.objectIds || [])];
    state.diagnostics.completedCommandStageIds = [...(runner.completedStageIds || [])];
    state.diagnostics.failedCommandStageIds = [...(runner.failedStageIds || [])];
    state.diagnostics.skippedCommandStageIds = [...(runner.skippedStageIds || [])];
    state.diagnostics.lastStageError = runner.lastStageError || "";
    state.diagnostics.commandStageResultLog = [...(runner.resultLog || [])];
    state.diagnostics.commandStageQueueSnapshot = (runner.queue || []).map(item => ({
        batchId: item.batchId || "",
        stageId: item.stage?.stageId || "",
        barrierPolicy: resolveStageBarrierPolicy(item.stage),
        waitSeconds: roundWait(Number(item.stage?.waitSeconds) || 0),
        barrierEventId: item.stage?.barrierEventId || item.stage?.metadata?.barrierEventId || ""
    }));
    syncCommandJournalDiagnostics(state);
}

function drainReadyStages(state, applyStage) {
    const runner = state.commandStageRunner;
    if (!runner || runner.cancelled) {
        return;
    }

    while (completeReadyBarrier(state, runner) && runner.queue.length > 0 && !runner.cancelled) {
        const item = runner.queue.shift();
        runner.currentBatchId = item.batchId;
        runner.currentStageId = item.stage?.stageId || "";
        if (!item.stage) {
            runner.skippedStageIds.push("");
            const result = appendStageResult(runner, item, "skipped", "Stage payload was missing.");
            appendJournal(state, item, "stage-warning", "skipped", result.resultId, result.error);
            syncStageDiagnostics(state);
            continue;
        }

        appendJournal(state, item, "stage-start", "started");
        try {
            applyStage(item.stage);
            runner.completedStageIds.push(runner.currentStageId);
            const result = appendStageResult(runner, item, "applied", "");
            appendJournal(state, item, "stage-apply", "applied", result.resultId);
        } catch (error) {
            const message = error?.message || String(error);
            runner.failedStageIds.push(runner.currentStageId);
            runner.lastStageError = message;
            state.diagnostics.lastError = message;
            const result = appendStageResult(runner, item, "failed", message);
            appendJournal(state, item, "stage-failure", "failed", result.resultId, message);
        }

        runner.activeBarrier = createStageBarrier(state, item);
        if (!runner.activeBarrier) {
            appendJournal(state, item, "stage-complete", "completed");
        }

        syncStageDiagnostics(state);
    }

    if (runner.queue.length === 0 && completeReadyBarrier(state, runner)) {
        runner.currentBatchId = "";
        runner.currentStageId = "";
        syncStageDiagnostics(state);
    }
}

function completeReadyBarrier(state, runner) {
    if (!runner.activeBarrier) {
        return true;
    }

    const barrierDescription = describeStageBarrier(state, runner.activeBarrier);
    if (!barrierDescription.isReady) {
        syncStageDiagnostics(state);
        return false;
    }

    const warning = runner.activeBarrier.timedOut
        ? `timeout:${runner.activeBarrier.stageId}`
        : barrierDescription.warning || "";
    if (warning) {
        state.diagnostics.lastStageBarrierWarning = warning;
        state.diagnostics.commandStageBarrierTimedOut = runner.activeBarrier.timedOut === true;
        appendCommandStageJournal(state, {
            eventKind: "stage-warning",
            batchId: runner.activeBarrier.batchId,
            stageId: runner.activeBarrier.stageId,
            status: runner.activeBarrier.timedOut ? "timeout" : "warning",
            barrierPolicy: runner.activeBarrier.policy,
            message: warning
        });
    }

    appendCommandStageJournal(state, {
        eventKind: "stage-complete",
        batchId: runner.activeBarrier.batchId,
        stageId: runner.activeBarrier.stageId,
        status: "completed",
        barrierPolicy: runner.activeBarrier.policy,
        resultId: `${runner.activeBarrier.batchId}:${runner.activeBarrier.stageId}:completed`
    });
    runner.activeBarrier = null;
    return true;
}

function appendJournal(state, item, eventKind, status, resultId = "", message = "") {
    return appendCommandStageJournal(state, {
        eventKind,
        batchId: item?.batchId || "",
        stageId: item?.stage?.stageId || "",
        resultId,
        status,
        barrierPolicy: resolveStageBarrierPolicy(item?.stage),
        message
    });
}

function appendStageResult(runner, item, status, error) {
    const resultId = `${item.batchId || ""}:${item.stage?.stageId || ""}:${status}:${++runner.resultSequence}`;
    const result = {
        resultId,
        batchId: item.batchId || "",
        stageId: item.stage?.stageId || "",
        status,
        barrierPolicy: resolveStageBarrierPolicy(item.stage),
        error: error || ""
    };
    runner.resultLog.push(result);
    if (runner.resultLog.length > 100) {
        runner.resultLog.splice(0, runner.resultLog.length - 100);
    }

    return result;
}

function createCommandStageRunner() {
    return {
        queue: [],
        activeBarrier: null,
        currentBatchId: "",
        currentStageId: "",
        completedStageIds: [],
        failedStageIds: [],
        skippedStageIds: [],
        resultLog: [],
        resultSequence: 0,
        lastStageError: "",
        cancelled: false
    };
}

function roundWait(value) {
    return Math.round((Number(value) || 0) * 1000) / 1000;
}
