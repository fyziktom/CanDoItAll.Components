# Future WebGlRunLib contracts - do not implement in this hardening unless explicitly requested

These contracts document the second phase boundary.

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

using CanDoItAll.Components.WebGlLib;

public sealed class WebGlRunModel
{
    public string RunId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public WebGlSceneModel Scene { get; set; } = new();

    public WebGlRunClock Clock { get; set; } = new();

    public List<WebGlRunFrame> Frames { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunClock
{
    public double TimeSeconds { get; set; }

    public double Speed { get; set; } = 1.0;

    public bool IsPlaying { get; set; }

    public bool Loop { get; set; }
}

public sealed class WebGlRunFrame
{
    public double TimeSeconds { get; set; }

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];
}
```

Do not put economy/game/process concepts here.
