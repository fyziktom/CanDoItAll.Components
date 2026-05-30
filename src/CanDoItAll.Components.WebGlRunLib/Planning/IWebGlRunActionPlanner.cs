namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunActionPlanner
{
    WebGlRunActionPlan Plan(WebGlRunAction action, WebGlRunPlanningContext context);
}
