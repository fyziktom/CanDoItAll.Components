const defaultMaxCommandResults = 100;
const defaultMaxFailedCommands = 50;
const defaultMaxBatchChildResults = 25;
const defaultMaxBatchMessages = 25;

export function createCommandResult(state, commandKind, commandId = "") {
    return {
        commandId: commandId || buildCommandId(state, commandKind),
        success: true,
        succeeded: true,
        sceneId: state?.sceneModel?.sceneId || "",
        commandKind: commandKind || "command",
        revision: state?.sceneModel?.uiState?.revision || 0,
        errors: [],
        warnings: [],
        affectedObjectIds: [],
        affectedLinkIds: [],
        diagnostics: {},
        metadata: {}
    };
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
    rememberFailedCommand(state, result);
    if (state?.diagnostics) {
        state.diagnostics.lastError = message || runtimeErrorTitle;
        state.diagnostics.failedPatchCommands?.add?.(message || runtimeErrorTitle);
    }

    state?.notifyRuntimeError?.(runtimeErrorTitle, new Error(message || runtimeErrorTitle));
    state?.scheduleRender?.("command-failed");
    return result;
}

export function completeCommandResult(state, result) {
    result.success = result.errors.length === 0;
    result.succeeded = result.success;
    result.revision = state?.sceneModel?.uiState?.revision || 0;
    result.affectedObjectIds = unique(result.affectedObjectIds);
    result.affectedLinkIds = unique(result.affectedLinkIds);
    result.diagnostics = {
        renderCount: String(state?.diagnostics?.renderCount || 0),
        activeMotionCount: String(state?.motions?.size || 0),
        failedCommandCount: String(state?.diagnostics?.failedCommandDetails?.length || 0)
    };
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
        .invokeMethodAsync(methodName, JSON.stringify(result))
        .catch(error => console.warn(`WebGL scene ${methodName} callback failed.`, error));
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
