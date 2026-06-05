using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlSceneViewBrowserRuntime(WebGlSceneView sceneView) : IWebGlRunBrowserRuntime
{
    public async ValueTask<WebGlSceneCommandResult?> ImportSceneAsync(
        WebGlSceneDocument sceneDocument,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sceneDocument);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.ImportSceneDocumentDetailedAsync(sceneDocument).ConfigureAwait(false);
    }

    public async ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(
        WebGlSceneCommandBatch batch,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(batch);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.ApplyCommandBatchAsync(batch).ConfigureAwait(false);
    }

    public async ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAndWaitAsync(
        WebGlSceneCommandBatch batch,
        WebGlRunRuntimeIdleWaitOptions options,
        bool requireRuntimeIdle = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(batch);
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.ApplyCommandBatchAndWaitAsync(
            batch,
            options.TimeoutMs,
            options.PollIntervalMs,
            options.Reason,
            requireRuntimeIdle).ConfigureAwait(false);
    }

    public async ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.GetDiagnosticsAsync().ConfigureAwait(false);
    }

    public async ValueTask<WebGlRuntimeIdleResult?> WaitForRuntimeIdleAsync(
        WebGlRunRuntimeIdleWaitOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.WaitForRuntimeIdleAsync(
            options.TimeoutMs,
            options.PollIntervalMs,
            options.Reason).ConfigureAwait(false);
    }
}
