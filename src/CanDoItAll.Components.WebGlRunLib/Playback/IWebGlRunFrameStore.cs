using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunFrameSource
{
    ValueTask<WebGlRunFrame?> GetFrameAsync(WebGlRunId runId, long frameIndex, CancellationToken cancellationToken = default);
}

public interface IWebGlRunFrameStore : IWebGlRunFrameSource
{
    ValueTask<IReadOnlyList<WebGlRunFrame>> ListFramesAsync(WebGlRunId runId, CancellationToken cancellationToken = default);
}

public interface IWebGlRunFrameApplier
{
    ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default);
}

public interface IWebGlRunSceneProjector<in TFrame>
{
    WebGlScenePatch ProjectPatch(TFrame frame);
}

public interface IWebGlRunSnapshotStore
{
    ValueTask SaveAsync(WebGlRunPlaybackState state, WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default);
}
