```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionStage
{
    public string StageId { get; set; } = string.Empty;
    public string ParentActionId { get; set; } = string.Empty;
    public int StageIndex { get; set; }
    public string ExecutionPolicy { get; set; } = WebGlRunStageExecutionPolicies.PreserveOrder;
    public List<WebGlRunAction> Actions { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlRunStageExecutionPolicies
{
    public const string PreserveOrder = "preserve-order";
    public const string Parallel = "parallel";
    public const string CoalesceWithinStage = "coalesce-within-stage";
}
```
