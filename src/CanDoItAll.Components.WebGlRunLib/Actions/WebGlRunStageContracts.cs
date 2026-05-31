namespace CanDoItAll.Components.WebGlRunLib;

public static class WebGlRunCoalescingScopes
{
    public const string None = "none";
    public const string StageOnly = "stage-only";
    public const string Frame = "frame";
}

public sealed class WebGlRunActionStagePolicy
{
    public string StageGroupId { get; set; } = string.Empty;
    public string CoalescingScope { get; set; } = WebGlRunCoalescingScopes.StageOnly;
    public string ExecutionPolicy { get; set; } = WebGlRunStageExecutionPolicies.PreserveOrder;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionStageBoundary
{
    public string BoundaryId { get; set; } = string.Empty;
    public int StageIndex { get; set; } = -1;
    public string StageGroupId { get; set; } = string.Empty;
    public string CoalescingScope { get; set; } = WebGlRunCoalescingScopes.StageOnly;
    public Dictionary<string, string> Metadata { get; set; } = [];
}
