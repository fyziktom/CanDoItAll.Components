import { resolveSceneRevision } from "./34-webgl-scene-revisions.js";

const defaultMaxCommandResults = 100;
const defaultMaxFailedCommands = 50;
const defaultMaxBatchChildResults = 25;
const defaultMaxBatchMessages = 25;

export const commandLifecycleStates = Object.freeze({
    accepted: "accepted",
    scheduled: "scheduled",
    active: "active",
    settled: "settled",
    cancelled: "cancelled",
    failed: "failed"
});

export function createCommandResult(state, commandKind, commandId = "") {
    const result = {
        commandId: commandId || buildCommandId(state, commandKind),
        success: true,
        succeeded: true,
        sceneId: state?.sceneModel?.sceneId || "",
        commandKind: commandKind || "command",
        lifecycleState: commandLifecycleStates.accepted,
        settled: false,
        revision: resolveSceneRevision(state?.sceneModel),
        errors: [],
        warnings: [],
        affectedObjectIds: [],
        affectedLinkIds: [],
        diagnostics: {},
        metadata: {}
    };
    syncLifecycleMetadata(result);
    return result;
}

export function warnCommand(result, message) {
    if (message) {
        result.warnings.push(message);
    }

    return result;
}

export function failCommand(state, result, message, runtimeErrorTitle = "WebGL scene command failed.") {
    if (message) {
        result.errors.push(message);
    }

    result.success = false;
    result.succeeded = false;
    setCommandLifecycle(result, commandLifecycleStates.failed, false);
    rememberFailedCommand(state, result);
    if (state?.diagnostics) {
        state.diagnostics.lastError = message || runtimeErrorTitle;
        state.diagnostics.failedPatchCommands?.add?.(message || runtimeErrorTitle);
    }

    state?.notifyRuntimeError?.(runtimeErrorTitle, new Error(message || runtimeErrorTitle));
    state?.scheduleRender?.("command-failed");
    return result;
}

export function setCommandLifecycle(result, lifecycleState, settled = lifecycleState === commandLifecycleStates.settled) {
    if (!result) {
        return result;
    }

    result.lifecycleState = normalizeLifecycleState(lifecycleState);
    result.settled = settled === true;
    syncLifecycleMetadata(result);
    syncLifecycleDiagnostics(result);
    return result;
}

export function completeCommandResult(state, result) {
    const existingDiagnostics = result.diagnostics || {};
    result.success = result.errors.length === 0;
    result.succeeded = result.success;
    if (!result.success) {
        setCommandLifecycle(result, commandLifecycleStates.failed, false);
    } else if (!result.lifecycleState || result.lifecycleState === commandLifecycleStates.accepted) {
        setCommandLifecycle(result, commandLifecycleStates.settled, true);
    } else {
        syncLifecycleMetadata(result);
    }

    result.revision = resolveSceneRevision(state?.sceneModel);
    result.affectedObjectIds = unique(result.affectedObjectIds);
    result.affectedLinkIds = unique(result.affectedLinkIds);
    result.diagnostics = {
        ...existingDiagnostics,
        renderCount: String(state?.diagnostics?.renderCount || 0),
        activeMotionCount: String(state?.motions?.size || 0),
        queuedMotionCount: String(countQueuedMotions(state)),
        queuedCommandStageCount: String(state?.diagnostics?.queuedCommandStageCount || 0),
        currentCommandBatchId: state?.diagnostics?.currentCommandBatchId || "",
        currentCommandStageId: state?.diagnostics?.currentCommandStageId || "",
        commandStageBarrierPolicy: state?.diagnostics?.commandStageBarrierPolicy || "",
        commandStageBarrierBlockers: (state?.diagnostics?.commandStageBarrierBlockers || []).join(","),
        lifecycleState: result.lifecycleState || "",
        settled: String(result.settled === true),
        failedCommandCount: String(state?.diagnostics?.failedCommandDetails?.length || 0),
        assetCacheMode: state?.diagnostics?.assetCacheMode || state?.assetCache?.mode || "state-local",
        retainedSharedTextureCount: String(state?.diagnostics?.retainedSharedTextureCount || 0),
        disposedTextureCount: String(state?.diagnostics?.disposedTextureCount || 0),
        fullSceneRebuildCount: String(state?.diagnostics?.fullSceneRebuildCount || 0),
        transformOnlyPatchCount: String(state?.diagnostics?.transformOnlyPatchCount || 0),
        symbolOnlyPatchCount: String(state?.diagnostics?.symbolOnlyPatchCount || 0),
        linkOnlyPatchCount: String(state?.diagnostics?.linkOnlyPatchCount || 0),
        linkGeometryUpdateCount: String(state?.diagnostics?.linkGeometryUpdateCount || 0),
        lastPatchClassification: state?.diagnostics?.lastPatchClassification || ""
    };
    syncLifecycleDiagnostics(result);
    rememberCommandResult(state, result);
    notifyCommandResult(state, result);
    return result;
}

export function compactBatchResultForInterop(state, result) {
    const totalCommandResultCount = result.commandResults.length;
    const totalWarningCount = result.warnings.length;
    const totalErrorCount = result.errors.length;
    const childLimit = normalizeLimit(state?.options?.maxCommandBatchChildResults, defaultMaxBatchChildResults);
    const messageLimit = normalizeLimit(state?.options?.maxCommandBatchMessages, defaultMaxBatchMessages);

    result.commandResults = result.commandResults.slice(0, childLimit).map(compactChildCommandResult);
    result.warnings = limitWithOverflow(result.warnings, messageLimit, totalWarningCount, "warning");
    result.errors = limitWithOverflow(result.errors, messageLimit, totalErrorCount, "error");
    result.metadata = {
        ...result.metadata,
        totalCommandResultCount: String(totalCommandResultCount),
        returnedCommandResultCount: String(result.commandResults.length),
        totalWarningCount: String(totalWarningCount),
        returnedWarningCount: String(Math.min(totalWarningCount, messageLimit)),
        totalErrorCount: String(totalErrorCount),
        returnedErrorCount: String(Math.min(totalErrorCount, messageLimit))
    };
}

function compactChildCommandResult(result) {
    return {
        commandId: result?.commandId || "",
        success: result?.success !== false,
        succeeded: result?.success !== false,
        lifecycleState: result?.lifecycleState || commandLifecycleStates.settled,
        settled: result?.settled === true,
        sceneId: result?.sceneId || "",
        commandKind: result?.commandKind || "",
        revision: Number(result?.revision) || 0,
        errors: limitWithOverflow(result?.errors || [], defaultMaxBatchMessages, result?.errors?.length || 0, "error"),
        warnings: limitWithOverflow(result?.warnings || [], defaultMaxBatchMessages, result?.warnings?.length || 0, "warning"),
        affectedObjectIds: limitWithOverflow(result?.affectedObjectIds || [], defaultMaxBatchMessages, result?.affectedObjectIds?.length || 0, "affected object"),
        affectedLinkIds: limitWithOverflow(result?.affectedLinkIds || [], defaultMaxBatchMessages, result?.affectedLinkIds?.length || 0, "affected link"),
        diagnostics: result?.diagnostics || {},
        metadata: {
            ...(result?.metadata || {}),
            totalAffectedObjectCount: String(result?.affectedObjectIds?.length || 0),
            totalAffectedLinkCount: String(result?.affectedLinkIds?.length || 0)
        }
    };
}

function limitWithOverflow(values, limit, totalCount, label) {
    const items = Array.isArray(values) ? values : [];
    if (items.length <= limit) {
        return items;
    }

    return [
        ...items.slice(0, limit),
        `${Math.max(0, totalCount - limit)} additional ${label} item(s) omitted from the interop result.`
    ];
}

function rememberCommandResult(state, result) {
    if (!state) {
        return;
    }

    const limit = normalizeLimit(state.options?.maxCommandResultHistory, defaultMaxCommandResults);
    state.commandResults = Array.isArray(state.commandResults) ? state.commandResults : [];
    state.commandResults.push(result);
    trimToLimit(state.commandResults, limit);
}

function rememberFailedCommand(state, result) {
    if (!state?.diagnostics) {
        return;
    }

    const limit = normalizeLimit(state.options?.maxCommandResultHistory, defaultMaxFailedCommands);
    state.diagnostics.failedCommandDetails = Array.isArray(state.diagnostics.failedCommandDetails)
        ? state.diagnostics.failedCommandDetails
        : [];
    state.diagnostics.failedCommandDetails.push({
        ...result,
        errors: [...result.errors],
        warnings: [...result.warnings],
        affectedObjectIds: unique(result.affectedObjectIds),
        affectedLinkIds: unique(result.affectedLinkIds)
    });
    trimToLimit(state.diagnostics.failedCommandDetails, Math.min(limit, defaultMaxFailedCommands));
}

function notifyCommandResult(state, result) {
    if (!state?.dotNetRef) {
        return;
    }

    const methodName = result.success ? "OnCommandCompleted" : "OnCommandFailed";
    state.dotNetRef
        .invokeMethodAsync(methodName, JSON.stringify(compactCommandResultForCallback(result)))
        .catch(error => console.warn(`WebGL scene ${methodName} callback failed.`, error));
}

function compactCommandResultForCallback(result) {
    const totalAffectedObjectCount = result?.affectedObjectIds?.length || 0;
    const totalAffectedLinkCount = result?.affectedLinkIds?.length || 0;
    const affectedObjectIds = limitWithOverflow(
        result?.affectedObjectIds || [],
        defaultMaxBatchMessages,
        totalAffectedObjectCount,
        "affected object");
    const affectedLinkIds = limitWithOverflow(
        result?.affectedLinkIds || [],
        defaultMaxBatchMessages,
        totalAffectedLinkCount,
        "affected link");

    return {
        commandId: result?.commandId || "",
        success: result?.success !== false,
        succeeded: result?.success !== false,
        lifecycleState: result?.lifecycleState || commandLifecycleStates.settled,
        settled: result?.settled === true,
        sceneId: result?.sceneId || "",
        commandKind: result?.commandKind || "",
        revision: Number(result?.revision) || 0,
        errors: limitWithOverflow(result?.errors || [], defaultMaxBatchMessages, result?.errors?.length || 0, "error"),
        warnings: limitWithOverflow(result?.warnings || [], defaultMaxBatchMessages, result?.warnings?.length || 0, "warning"),
        affectedObjectIds,
        affectedLinkIds,
        diagnostics: result?.diagnostics || {},
        metadata: {
            ...(result?.metadata || {}),
            totalAffectedObjectCount: String(totalAffectedObjectCount),
            returnedAffectedObjectCount: String(affectedObjectIds.length),
            totalAffectedLinkCount: String(totalAffectedLinkCount),
            returnedAffectedLinkCount: String(affectedLinkIds.length)
        }
    };
}

function syncLifecycleMetadata(result) {
    result.metadata = result.metadata || {};
    result.lifecycleState = normalizeLifecycleState(result.lifecycleState);
    result.metadata.lifecycleState = result.lifecycleState;
    result.metadata.settled = String(result.settled === true);
}

function syncLifecycleDiagnostics(result) {
    if (!result?.diagnostics) {
        return;
    }

    result.diagnostics.lifecycleState = result.lifecycleState || commandLifecycleStates.accepted;
    result.diagnostics.settled = String(result.settled === true);
}

function normalizeLifecycleState(value) {
    const text = String(value || "").trim().toLowerCase();
    return Object.values(commandLifecycleStates).includes(text)
        ? text
        : commandLifecycleStates.accepted;
}

function countQueuedMotions(state) {
    let count = 0;
    for (const queue of state?.motionQueuesByObjectId?.values?.() || []) {
        count += queue?.length || 0;
    }

    return count;
}

function buildCommandId(state, commandKind) {
    if (state?.options?.deterministicMode !== false) {
        state.nextCommandSequence = (state.nextCommandSequence || 0) + 1;
        return `${commandKind || "command"}:${state.nextCommandSequence}`;
    }

    return `${commandKind || "command"}:${Date.now()}`;
}

function normalizeLimit(value, fallback) {
    const limit = Number(value);
    return Number.isFinite(limit) && limit > 0 ? Math.min(Math.floor(limit), 1000) : fallback;
}

function trimToLimit(items, limit) {
    while (items.length > limit) {
        items.shift();
    }
}

function unique(values) {
    return Array.from(new Set((values || []).filter(Boolean)));
}
