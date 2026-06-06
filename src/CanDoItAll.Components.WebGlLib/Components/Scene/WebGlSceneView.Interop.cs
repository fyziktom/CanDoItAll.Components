using Microsoft.JSInterop;

namespace CanDoItAll.Components.WebGlLib;

public partial class WebGlSceneView
{
    public Task FitViewAsync()
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.fitView", host).AsTask();

    public Task FocusObjectAsync(string objectId)
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.focusObject", host, objectId).AsTask();

    public Task ResetCameraAsync()
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.resetCamera", host).AsTask();

    public Task<string?> CaptureImageAsync()
        => JsRuntime.InvokeAsync<string?>("CanDoItAll.webglScene.exportImageData", host).AsTask();

    public Task<WebGlSceneProofSnapshot?> GetProofSnapshotAsync()
        => JsRuntime.InvokeAsync<WebGlSceneProofSnapshot?>("CanDoItAll.webglScene.getProofSnapshot", host).AsTask();

    public Task<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync()
        => JsRuntime.InvokeAsync<WebGlRuntimeDiagnostics?>("CanDoItAll.webglScene.getDiagnostics", host).AsTask();

    public Task<WebGlSceneModel?> ExportSceneAsync()
        => JsRuntime.InvokeAsync<WebGlSceneModel?>("CanDoItAll.webglScene.exportScene", host).AsTask();

    public async Task<bool> ImportSceneAsync(WebGlSceneModel sceneModel)
    {
        ArgumentNullException.ThrowIfNull(sceneModel);
        bool imported = await JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.importScene", host, sceneModel, Options);
        if (imported)
        {
            MarkExternalImportApplied(sceneModel, Options);
        }

        return imported;
    }

    public async Task<WebGlSceneCommandResult?> ImportSceneDetailedAsync(WebGlSceneModel sceneModel)
    {
        ArgumentNullException.ThrowIfNull(sceneModel);
        WebGlSceneCommandResult? result = await JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.importSceneDetailed", host, sceneModel, Options);
        if (result?.Success == true)
        {
            MarkExternalImportApplied(sceneModel, Options);
        }

        return result;
    }

    public async Task<bool> ImportSceneDocumentAsync(WebGlSceneDocument sceneDocument)
    {
        ArgumentNullException.ThrowIfNull(sceneDocument);
        WebGlRuntimeOptions importOptions = sceneDocument.RuntimeOptions ?? Options;
        bool imported = await JsRuntime.InvokeAsync<bool>(
            "CanDoItAll.webglScene.importScene",
            host,
            sceneDocument.Scene,
            importOptions);
        if (imported)
        {
            MarkExternalImportApplied(sceneDocument.Scene, importOptions);
        }

        return imported;
    }

    public async Task<WebGlSceneCommandResult?> ImportSceneDocumentDetailedAsync(WebGlSceneDocument sceneDocument)
    {
        ArgumentNullException.ThrowIfNull(sceneDocument);
        WebGlRuntimeOptions importOptions = sceneDocument.RuntimeOptions ?? Options;
        WebGlSceneCommandResult? result = await JsRuntime.InvokeAsync<WebGlSceneCommandResult?>(
            "CanDoItAll.webglScene.importSceneDetailed",
            host,
            sceneDocument.Scene,
            importOptions);
        if (result?.Success == true)
        {
            MarkExternalImportApplied(sceneDocument.Scene, importOptions);
        }

        return result;
    }

    public Task<bool> ApplyPatchAsync(WebGlScenePatch patch)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.applyPatch", host, patch).AsTask();

    public Task<WebGlSceneCommandResult?> ApplyPatchDetailedAsync(WebGlScenePatch patch)
        => JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.applyPatchDetailed", host, patch).AsTask();

    public Task<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(WebGlSceneCommandBatch batch)
        => JsRuntime.InvokeAsync<WebGlSceneCommandBatchResult?>("CanDoItAll.webglScene.applyCommandBatch", host, batch).AsTask();

    public Task<WebGlSceneCommandBatchResult?> ApplyCommandBatchAndWaitAsync(
        WebGlSceneCommandBatch batch,
        int timeoutMs = 2_000,
        int pollIntervalMs = 16,
        string reason = "command-batch-settled",
        bool requireRuntimeIdle = true,
        string runtimeIdlePolicyMode = WebGlRuntimeIdlePolicyModes.VisualStrict)
        => JsRuntime.InvokeAsync<WebGlSceneCommandBatchResult?>(
            "CanDoItAll.webglScene.applyCommandBatchAndWait",
            host,
            batch,
            new
            {
                timeoutMs,
                pollIntervalMs,
                reason,
                policyMode = runtimeIdlePolicyMode,
                requireRuntimeIdle,
                hardFailOnIdleTimeout = requireRuntimeIdle
            }).AsTask();

    public Task<bool> SetObjectTransformAsync(string objectId, WebGlSceneObjectPatch transform)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.setObjectTransform", host, objectId, transform).AsTask();

    public Task<bool> MoveObjectAsync(string objectId, WebGlVector3 position)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.moveObject", host, objectId, position).AsTask();

    public Task<bool> EnqueueMotionAsync(WebGlObjectMotionCommand command)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.enqueueMotion", host, command).AsTask();

    public Task<WebGlSceneCommandResult?> EnqueueMotionDetailedAsync(WebGlObjectMotionCommand command)
        => JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.enqueueMotionDetailed", host, command).AsTask();

    public Task<WebGlSceneCommandResult?> CancelCommandStagesAsync(string reason = "cancelled")
        => JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.cancelCommandStages", host, reason).AsTask();

    public async Task<WebGlSceneCommandResult?> StopRuntimeActivityAsync(
        string reason = "runtime-stop",
        bool waitForIdle = false,
        int timeoutMs = 2_000,
        int pollIntervalMs = 16,
        bool requireIdle = true,
        string runtimeIdlePolicyMode = WebGlRuntimeIdlePolicyModes.VisualStrict)
    {
        WebGlSceneCommandResult? result = await JsRuntime.InvokeAsync<WebGlSceneCommandResult?>(
            "CanDoItAll.webglScene.stopRuntimeActivity",
            host,
            reason);
        if (!waitForIdle)
        {
            return result;
        }

        WebGlRuntimeIdleResult? idleResult = await WaitForRuntimeIdleAsync(timeoutMs, pollIntervalMs, reason, runtimeIdlePolicyMode).ConfigureAwait(false);
        if (result is null || idleResult is null)
        {
            return result;
        }

        AnnotateRuntimeIdleResult(result, idleResult, reason, requireIdle);

        return result;
    }

    public Task<WebGlRuntimeIdleResult?> WaitForRuntimeIdleAsync(
        int timeoutMs = 2_000,
        int pollIntervalMs = 16,
        string reason = "runtime-idle",
        string policyMode = WebGlRuntimeIdlePolicyModes.AllowFinalRenderDrain)
        => JsRuntime.InvokeAsync<WebGlRuntimeIdleResult?>(
            "CanDoItAll.webglScene.waitForRuntimeIdle",
            host,
            new
            {
                timeoutMs,
                pollIntervalMs,
                reason,
                policyMode
            }).AsTask();

    public Task<WebGlRuntimeIdleResult?> WaitForRuntimeIdleAsync(WebGlRuntimeIdleOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return JsRuntime.InvokeAsync<WebGlRuntimeIdleResult?>(
            "CanDoItAll.webglScene.waitForRuntimeIdle",
            host,
            options).AsTask();
    }

    public Task<bool> ClearMotionsAsync(string? objectId = null)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.clearMotions", host, objectId).AsTask();

    public Task<WebGlSceneCommandResult?> ClearMotionsDetailedAsync(string? objectId = null)
        => JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.clearMotionsDetailed", host, objectId).AsTask();

    public Task<bool> CancelMotionAsync(string motionId)
        => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.cancelMotion", host, motionId).AsTask();

    public Task<WebGlSceneCommandResult?> CancelMotionDetailedAsync(string motionId)
        => JsRuntime.InvokeAsync<WebGlSceneCommandResult?>("CanDoItAll.webglScene.cancelMotionDetailed", host, motionId).AsTask();
}
