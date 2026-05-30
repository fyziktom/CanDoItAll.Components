const defaultMaxCommandResults = 100;
const defaultMaxFailedCommands = 50;

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
