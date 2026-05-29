# Skeleton — `20-webgl-scene-command-results.js`

```js
const defaultMaxCommandResults = 100;

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

export function failCommand(state, result, message, runtimeErrorTitle = "WebGL scene command failed.") {
    result.success = false;
    result.succeeded = false;
    result.errors.push(message);
    rememberFailedCommand(state, result);
    state.diagnostics.lastError = message;
    state.notifyRuntimeError?.(runtimeErrorTitle, new Error(message));
    state.scheduleRender?.("command-failed");
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
        activeMotionCount: String(state?.motions?.size || 0)
    };
    rememberCommandResult(state, result);
    return result;
}

function rememberCommandResult(state, result) {
    const limit = state?.options?.maxCommandResultHistory || defaultMaxCommandResults;
    state.commandResults = Array.isArray(state.commandResults) ? state.commandResults : [];
    state.commandResults.push(result);
    while (state.commandResults.length > limit) {
        state.commandResults.shift();
    }
}

function rememberFailedCommand(state, result) {
    state.diagnostics.failedCommandDetails = Array.isArray(state.diagnostics.failedCommandDetails)
        ? state.diagnostics.failedCommandDetails
        : [];
    state.diagnostics.failedCommandDetails.push(result);
    while (state.diagnostics.failedCommandDetails.length > 50) {
        state.diagnostics.failedCommandDetails.shift();
    }
}

function buildCommandId(state, commandKind) {
    if (state?.options?.deterministicMode) {
        state.nextCommandSequence = (state.nextCommandSequence || 0) + 1;
        return `${commandKind}:${state.nextCommandSequence}`;
    }

    return `${commandKind}:${Date.now()}`;
}

function unique(values) {
    return Array.from(new Set((values || []).filter(Boolean)));
}
```
