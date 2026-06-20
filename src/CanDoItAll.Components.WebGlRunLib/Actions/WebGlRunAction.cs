using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunAction
{
    public string ActionId { get; set; } = string.Empty;

    public string SequenceId { get; set; } = string.Empty;

    public string ParentActionId { get; set; } = string.Empty;

    public int StageIndex { get; set; } = -1;

    public string StageGroupId { get; set; } = string.Empty;

    public string CoalescingScope { get; set; } = WebGlRunCoalescingScopes.StageOnly;

    public int OrderIndex { get; set; } = -1;

    public string ExecutionPolicy { get; set; } = WebGlRunStageExecutionPolicies.PreserveOrder;

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

public static class WebGlRunStageExecutionPolicies
{
    public const string PreserveOrder = "preserve-order";
    public const string Parallel = "parallel";
    public const string CoalesceWithinStage = "coalesce-within-stage";
}
