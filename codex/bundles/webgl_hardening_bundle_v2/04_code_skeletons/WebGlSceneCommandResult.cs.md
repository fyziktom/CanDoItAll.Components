# WebGlSceneCommandResult.cs skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneCommandResult
{
    public bool Success { get; set; }

    public string SceneId { get; set; } = string.Empty;

    public string CommandKind { get; set; } = string.Empty;

    public int Revision { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public List<string> AffectedObjectIds { get; set; } = [];

    public List<string> AffectedLinkIds { get; set; } = [];

    public Dictionary<string, string> Diagnostics { get; set; } = [];
}
```

Use this for detailed patch/import/motion command APIs. Keep boolean APIs for compatibility if necessary.
