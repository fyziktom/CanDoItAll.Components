# Components skeleton — WebGlRunLib contracts

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocument
{
    public string SchemaVersion { get; set; } = "webgl-run/v1";
    public string RunId { get; set; } = string.Empty;
    public WebGlRunManifest Manifest { get; set; } = new();
    public WebGlRunTimeline Timeline { get; set; } = new();
    public WebGlRunPlaybackState Playback { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunManifest
{
    public string Title { get; set; } = string.Empty;
    public string ScenarioKey { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunFrame
{
    public int StepIndex { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    public WebGlSceneModel Scene { get; set; } = new();
    public WebGlScenePatch? PatchFromPrevious { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public interface IWebGlRunFrameSource
{
    ValueTask<WebGlRunFrame?> GetFrameAsync(int stepIndex, CancellationToken cancellationToken = default);
}
```

No economy/domain comments or references.
```
