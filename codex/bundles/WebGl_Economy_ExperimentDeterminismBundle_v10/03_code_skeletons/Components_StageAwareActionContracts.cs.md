# Skeleton: stage-aware WebGlRun actions

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionStage
{
    public string StageId { get; set; } = string.Empty;
    public int StageIndex { get; set; }
    public string StageGroupId { get; set; } = string.Empty;
    public string CoalescingScope { get; set; } = WebGlRunCoalescingScopes.StageOnly;
    public List<WebGlRunAction> Actions { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlRunCoalescingScopes
{
    public const string None = "none";
    public const string StageOnly = "stage-only";
    public const string Frame = "frame";
}

public sealed class WebGlRunActionPlan
{
    public List<WebGlRunActionStage> Stages { get; set; } = [];
    public List<WebGlSceneCommandBatch> CommandBatches { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public bool IsValid => Errors.Count == 0;
}
```
