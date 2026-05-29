# Skeleton — Generic WebGlRunLib Contracts

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

using CanDoItAll.Components.WebGlLib;

public sealed record WebGlRunId(string Value);

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
    public List<WebGlScenePatch> ScenePatches { get; set; } = [];
    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunPlaybackState
{
    public WebGlRunId RunId { get; set; } = new(string.Empty);
    public long CurrentFrameIndex { get; set; }
    public bool IsPlaying { get; set; }
    public double PlaybackSpeed { get; set; } = 1.0;
}

public interface IWebGlRunFrameSource
{
    ValueTask<WebGlRunFrame?> GetFrameAsync(WebGlRunId runId, long frameIndex, CancellationToken cancellationToken = default);
}

public interface IWebGlRunSceneProjector<in TFrame>
{
    WebGlScenePatch ProjectPatch(TFrame frame);
}
```

No economy, ledger, account, market, well, or process terms belong here.
