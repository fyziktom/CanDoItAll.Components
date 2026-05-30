# Components skeleton: action planner

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunActionPlanner
{
    WebGlRunActionPlan Plan(WebGlRunAction action, WebGlRunPlanningContext context);
}

public sealed class WebGlRunPlanningContext
{
    public WebGlSceneModel Scene { get; set; } = new();
    public WebGlVisualStateCatalog VisualStates { get; set; } = new();
    public Dictionary<string, WebGlVector3> ObjectPositions { get; set; } = [];
}

public sealed class WebGlRunActionPlan
{
    public string ActionId { get; set; } = string.Empty;
    public bool IsValid => Errors.Count == 0;
    public List<WebGlScenePatch> Patches { get; set; } = [];
    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
```
