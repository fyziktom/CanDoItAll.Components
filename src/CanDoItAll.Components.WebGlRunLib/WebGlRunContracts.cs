using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public readonly record struct WebGlRunId(string Value);

public sealed class WebGlRunDocument
{
    public string SchemaVersion { get; set; } = "webgl-run-document/v1";

    public WebGlRunId RunId { get; set; } = new(string.Empty);

    public WebGlSceneDocument InitialScene { get; set; } = new();

    public WebGlRunTimeline Timeline { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunTimeline
{
    public int FrameRate { get; set; } = 30;

    public List<WebGlRunFrame> Frames { get; set; } = [];
}

public sealed class WebGlRunFrame
{
    public long Index { get; set; }

    public double TimeSeconds { get; set; }

    public List<WebGlRunFramePatch> ScenePatches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunFramePatch
{
    public string Id { get; set; } = string.Empty;

    public WebGlScenePatch Patch { get; set; } = new();
}

public sealed class WebGlRunPlaybackState
{
    public WebGlRunId RunId { get; set; } = new(string.Empty);

    public long CurrentFrameIndex { get; set; }

    public bool IsPlaying { get; set; }

    public double PlaybackSpeed { get; set; } = 1.0;
}

public sealed class WebGlRunPlaybackCommand
{
    public string Kind { get; set; } = string.Empty;

    public long? TargetFrameIndex { get; set; }
}

public interface IWebGlRunPlaybackController
{
    WebGlRunPlaybackState State { get; }

    ValueTask<WebGlRunFrame?> ApplyAsync(WebGlRunPlaybackCommand command, CancellationToken cancellationToken = default);
}

public interface IWebGlRunFrameSource
{
    ValueTask<WebGlRunFrame?> GetFrameAsync(WebGlRunId runId, long frameIndex, CancellationToken cancellationToken = default);
}

public interface IWebGlRunSceneProjector<in TFrame>
{
    WebGlScenePatch ProjectPatch(TFrame frame);
}

public interface IWebGlRunSnapshotStore
{
    ValueTask SaveAsync(WebGlRunPlaybackState state, WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default);
}
