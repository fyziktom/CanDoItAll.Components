using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunBrowserApplyAdapter
{
    ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunFrameApplyResult frameApplyResult,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunPlaybackResult playbackResult,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlRunBrowserPlaybackApplyResult> ApplyPlaybackAsync(
        WebGlRunPlaybackResult playbackResult,
        CancellationToken cancellationToken = default);
}

public interface IWebGlRunBrowserRuntime
{
    ValueTask<WebGlSceneCommandResult?> ImportSceneAsync(
        WebGlSceneDocument sceneDocument,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(
        WebGlSceneCommandBatch batch,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAndWaitAsync(
        WebGlSceneCommandBatch batch,
        WebGlRunRuntimeIdleWaitOptions options,
        bool requireRuntimeIdle = true,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default);

    ValueTask<WebGlRuntimeIdleResult?> WaitForRuntimeIdleAsync(
        WebGlRunRuntimeIdleWaitOptions options,
        CancellationToken cancellationToken = default);
}
