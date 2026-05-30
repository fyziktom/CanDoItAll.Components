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

    public string Kind { get; set; } = string.Empty;

    public string ActionKind { get; set; } = string.Empty;

    public string ObjectId { get; set; } = string.Empty;

    public string SubjectObjectId { get; set; } = string.Empty;

    public WebGlRunActionTarget Target { get; set; } = new();

    public string TargetObjectId { get; set; } = string.Empty;

    public string PoseKey { get; set; } = string.Empty;

    public string SymbolKey { get; set; } = string.Empty;

    public double StartsAtSeconds { get; set; }

    public double DurationSeconds { get; set; }

    public string Easing { get; set; } = WebGlMotionEasings.Linear;

    public List<WebGlRunAction> Steps { get; set; } = [];

    public Dictionary<string, string> Parameters { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];

    public string ResolvedKind => !string.IsNullOrWhiteSpace(Kind) ? Kind : ActionKind;

    public string ResolvedObjectId => !string.IsNullOrWhiteSpace(ObjectId) ? ObjectId : SubjectObjectId;

    public string ResolvedTargetObjectId => !string.IsNullOrWhiteSpace(Target.ObjectId) ? Target.ObjectId : TargetObjectId;
}

public sealed class WebGlRunActionTarget
{
    public string ObjectId { get; set; } = string.Empty;

    public string AnchorKey { get; set; } = WebGlRunAnchorKeys.Center;

    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3? Position { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionStep
{
    public string StepId { get; set; } = string.Empty;

    public WebGlRunAction Action { get; set; } = new();

    public double DelaySeconds { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionPlan
{
    public int FrameRate { get; set; } = 1;

    public string ActionId { get; set; } = string.Empty;

    public bool IsValid => Errors.Count == 0;

    public List<WebGlRunAction> Actions { get; set; } = [];

    public List<WebGlRunObjectBinding> ObjectBindings { get; set; } = [];

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

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
    public const string Sequence = "sequence";
    public const string Parallel = "parallel";
    public const string ApplyPatch = "apply-patch";
    public const string MoveToObject = "move-to-object";
    public const string MoveToPosition = "move-to-position";
    public const string ReturnToAnchor = "return-to-anchor";
    public const string SetAsset = "set-asset";
    public const string SetPose = "set-pose";
    public const string ChangePose = "change-pose";
    public const string ShowSymbol = "show-symbol";
    public const string HideSymbol = "hide-symbol";
    public const string UpdateSymbol = "update-symbol";
    public const string SetLayerVisibility = "set-layer-visibility";
    public const string PulseLink = "pulse-link";
    public const string ResourceTransferVisual = "resource-transfer-visual";
    public const string Wait = "wait";
    public const string ApplyScenePatch = "apply-scene-patch";
}

public static class WebGlRunAnchorKeys
{
    public const string Center = "center";
    public const string Base = "base";
    public const string Top = "top";
    public const string Front = "front";
    public const string Back = "back";
    public const string Left = "left";
    public const string Right = "right";
    public const string Home = "home";
    public const string Work = "work";
    public const string Use = "use";
    public const string Admin = "admin";
}

public sealed class WebGlVisualStateCatalog
{
    public List<WebGlPoseDefinition> Poses { get; set; } = [];

    public List<WebGlSymbolDefinition> Symbols { get; set; } = [];

    public List<WebGlActionBinding> ActionBindings { get; set; } = [];
}

public sealed class WebGlPoseDefinition
{
    public string PoseKey { get; set; } = string.Empty;

    public string AssetVariantId { get; set; } = string.Empty;

    public string AssetId { get; set; } = string.Empty;

    public WebGlVector3? Rotation { get; set; }

    public WebGlVector3? Scale { get; set; }

    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;

    public string SymbolKey { get; set; } = string.Empty;

    public bool IsNoOpFallback { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSymbolDefinition
{
    public string SymbolKey { get; set; } = string.Empty;

    public string SemanticKind { get; set; } = string.Empty;

    public string SymbolAssetId { get; set; } = string.Empty;

    public string Color { get; set; } = "#facc15";

    public string EffectKey { get; set; } = WebGlSymbolEffects.Pulse;

    public string Tooltip { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlActionBinding
{
    public string ActionKind { get; set; } = string.Empty;

    public string PoseKey { get; set; } = string.Empty;

    public string SymbolKey { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}
