using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public string SubjectObjectId { get; set; } = string.Empty;
    public string TargetObjectId { get; set; } = string.Empty;
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunAction
{
    public string ActionId { get; set; } = string.Empty;
    public string ActionKind { get; set; } = string.Empty;
    public string SubjectObjectId { get; set; } = string.Empty;
    public string TargetObjectId { get; set; } = string.Empty;
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionPlan
{
    public int FrameRate { get; set; } = 1;
    public List<WebGlRunAction> Actions { get; set; } = [];
    public List<WebGlRunObjectBinding> ObjectBindings { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunObjectBinding
{
    public string ObjectId { get; set; } = string.Empty;
    public WebGlVector3? Position { get; set; }
    public WebGlVector3? AnchorPosition { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionMapping
{
    public string EventKind { get; set; } = string.Empty;
    public string ActionKind { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = [];
}

public static class WebGlRunActionKinds
{
    public const string MoveToObject = "move-to-object";
    public const string MoveToPosition = "move-to-position";
    public const string ReturnToAnchor = "return-to-anchor";
    public const string SetAsset = "set-asset";
    public const string SetPose = "set-pose";
    public const string ShowSymbol = "show-symbol";
    public const string HideSymbol = "hide-symbol";
    public const string PulseLink = "pulse-link";
    public const string ResourceTransferVisual = "resource-transfer-visual";
    public const string Wait = "wait";
    public const string ApplyScenePatch = "apply-scene-patch";
}
