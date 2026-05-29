# WebGlScene models skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneModel
{
    public string SceneId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public WebGlAssetCatalog AssetCatalog { get; set; } = new();

    public WebGlSceneEnvironment Environment { get; set; } = new();

    public WebGlSceneCamera Camera { get; set; } = new();

    public WebGlSceneUiState UiState { get; set; } = new();

    public WebGlSceneInteractionState Interaction { get; set; } = new();

    public List<WebGlSceneObject> Objects { get; set; } = [];

    public List<WebGlSceneLink> Links { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneObject
{
    public string Id { get; set; } = string.Empty;

    public string Kind { get; set; } = "object";

    public string Family { get; set; } = "generic";

    public string Title { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string AssetId { get; set; } = string.Empty;

    public WebGlVector3 Position { get; set; } = new();

    public WebGlVector3 Rotation { get; set; } = new();

    public WebGlVector3 Scale { get; set; } = WebGlVector3.One;

    public WebGlVector3 Size { get; set; } = new(1, 1, 1);

    public string Tone { get; set; } = "neutral";

    public string Color { get; set; } = "#ffffff";

    public bool IsSelectable { get; set; } = true;

    public bool IsDraggable { get; set; }

    public List<WebGlStatusSymbol> Symbols { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneLink
{
    public string Id { get; set; } = string.Empty;

    public string SourceObjectId { get; set; } = string.Empty;

    public string TargetObjectId { get; set; } = string.Empty;

    public string Kind { get; set; } = "link";

    public string Label { get; set; } = string.Empty;

    public string Color { get; set; } = "#94a3b8";

    public double Width { get; set; } = 1.0;

    public double Opacity { get; set; } = 0.75;

    public bool IsDirectional { get; set; }
}

public readonly record struct WebGlVector3(double X, double Y, double Z)
{
    public static WebGlVector3 Zero { get; } = new(0, 0, 0);

    public static WebGlVector3 One { get; } = new(1, 1, 1);
}
```
