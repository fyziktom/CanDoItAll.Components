# Components WebGlRun action planner shape

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunActionPlanner
{
    WebGlRunActionPlan Plan(WebGlRunAction action, WebGlRunPlanningContext context);
    WebGlSceneCommandBatch PlanBatch(IEnumerable<WebGlRunAction> actions, WebGlRunPlanningContext context);
}

public sealed class WebGlRunPlanningContext
{
    public WebGlSceneModel Scene { get; set; } = new();
    public WebGlVisualStateCatalog VisualStates { get; set; } = new();
    public Dictionary<string, WebGlVector3> RuntimeAnchors { get; set; } = [];
}

public sealed class WebGlRunTargetResolver
{
    public WebGlRunTargetResolutionResult Resolve(WebGlRunAction action, WebGlRunPlanningContext context)
    {
        // Resolve explicit position, object anchor, metadata anchor, or object center.
        throw new NotImplementedException();
    }
}
```
