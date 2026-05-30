# Components ordered run stage skeleton

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunStage
{
    public string StageId { get; set; } = string.Empty;
    public long FrameIndex { get; set; }
    public int Order { get; set; }
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public WebGlSceneCommandBatch CommandBatch { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunFrameV2
{
    public long Index { get; set; }
    public double TimeSeconds { get; set; }
    public List<WebGlRunStage> Stages { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```

All source code comments must be in English.
