export function appendCommandStageJournal(state, entry) {
    const journal = state.commandStageJournal ??= createCommandStageJournal();
    const normalized = {
        sequence: ++journal.sequence,
        timestampMs: Math.round(performance.now()),
        eventKind: entry.eventKind || "",
        batchId: entry.batchId || "",
        stageId: entry.stageId || "",
        resultId: entry.resultId || "",
        status: entry.status || "",
        barrierPolicy: entry.barrierPolicy || "",
        message: entry.message || ""
    };

    journal.entries.push(normalized);
    if (journal.entries.length > maxJournalEntries(state)) {
        const dropCount = journal.entries.length - maxJournalEntries(state);
        journal.entries.splice(0, dropCount);
        journal.droppedCount += dropCount;
    }

    incrementCounters(journal, normalized);
    syncCommandJournalDiagnostics(state);
    return normalized;
}

export function syncCommandJournalDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    const journal = state.commandStageJournal || createCommandStageJournal();
    const recentEntries = journal.entries.slice(-12);
    state.diagnostics.commandStageJournalCount = journal.entries.length;
    state.diagnostics.commandStageJournalDroppedCount = journal.droppedCount;
    state.diagnostics.commandStageJournalCounters = { ...journal.counters };
    state.diagnostics.commandStageRecentResultIds = recentEntries
        .map(entry => entry.resultId)
        .filter(Boolean);
    state.diagnostics.commandStageRecentJournalEntries = recentEntries;
}

export function resetCommandStageJournal(state) {
    state.commandStageJournal = createCommandStageJournal();
    syncCommandJournalDiagnostics(state);
}

function createCommandStageJournal() {
    return {
        sequence: 0,
        droppedCount: 0,
        counters: {
            started: 0,
            applied: 0,
            completed: 0,
            warnings: 0,
            failures: 0
        },
        entries: []
    };
}

function incrementCounters(journal, entry) {
    switch (entry.eventKind) {
        case "stage-start":
            journal.counters.started += 1;
            break;
        case "stage-apply":
            journal.counters.applied += 1;
            break;
        case "stage-complete":
            journal.counters.completed += 1;
            break;
        case "stage-warning":
            journal.counters.warnings += 1;
            break;
        case "stage-failure":
            journal.counters.failures += 1;
            break;
    }

    if (entry.status === "failed") {
        journal.counters.failures += 1;
    } else if (entry.status === "warning" || entry.status === "skipped") {
        journal.counters.warnings += 1;
    }
}

function maxJournalEntries(state) {
    const configured = Number(state?.options?.maxCommandStageJournalEntries);
    return Number.isFinite(configured) && configured > 0
        ? Math.min(1000, Math.max(20, Math.floor(configured)))
        : 200;
}
