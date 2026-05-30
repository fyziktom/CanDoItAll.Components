# Components skeleton: WebGlRunAction contracts

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunAction
{
    public string ActionId { get; set; } = string.Empty;
    public string Kind { get; set; } = WebGlRunActionKinds.ApplyPatch;
    public string ObjectId { get; set; } = string.Empty;
    public WebGlRunActionTarget Target { get; set; } = new();
    public string PoseKey { get; set; } = string.Empty;
    public string SymbolKey { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public string Easing { get; set; } = "linear";
    public List<WebGlRunAction> Steps { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionTarget
{
    public string ObjectId { get; set; } = string.Empty;
    public string AnchorKey { get; set; } = WebGlRunAnchorKeys.Center;
    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;
    public WebGlVector3? Position { get; set; }
}

public static class WebGlRunActionKinds
{
    public const string Sequence = "sequence";
    public const string Parallel = "parallel";
    public const string Wait = "wait";
    public const string ApplyPatch = "apply-patch";
    public const string MoveToPosition = "move-to-position";
    public const string MoveToObject = "move-to-object";
    public const string ReturnToAnchor = "return-to-anchor";
    public const string ChangePose = "change-pose";
    public const string ShowSymbol = "show-symbol";
    public const string HideSymbol = "hide-symbol";
    public const string UpdateSymbol = "update-symbol";
    public const string SetLayerVisibility = "set-layer-visibility";
}

public static class WebGlRunAnchorKeys
{
    public const string Center = "center";
    public const string Base = "base";
    public const string Top = "top";
    public const string Home = "home";
    public const string Work = "work";
    public const string Use = "use";
    public const string Admin = "admin";
}
```
