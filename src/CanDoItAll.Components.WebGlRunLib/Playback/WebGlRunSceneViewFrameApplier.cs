using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunSceneViewFrameApplier(WebGlSceneView sceneView, WebGlRunDocument document) :
    IWebGlRunFrameApplier,
    IWebGlRunInitialSceneApplier
{
    public async ValueTask ApplyInitialSceneAsync(WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sceneDocument);
        cancellationToken.ThrowIfCancellationRequested();
        await sceneView.ImportSceneAsync(sceneDocument.Scene).ConfigureAwait(false);
    }

    public async ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frame);
        cancellationToken.ThrowIfCancellationRequested();
        await sceneView.ApplyCommandBatchAsync(frame.CommandBatch).ConfigureAwait(false);
    }

    public async ValueTask ApplyAsync(WebGlRunPlaybackResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        cancellationToken.ThrowIfCancellationRequested();

        if (result.RequiresSceneReset)
        {
            await ApplyInitialSceneAsync(document.InitialScene, cancellationToken).ConfigureAwait(false);
        }

        foreach (WebGlRunFrame frame in result.FramesToApply)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await sceneView.ApplyCommandBatchAsync(WebGlRunFrameApplyResult.FromFrame(frame).CommandBatch).ConfigureAwait(false);
        }
    }
}
