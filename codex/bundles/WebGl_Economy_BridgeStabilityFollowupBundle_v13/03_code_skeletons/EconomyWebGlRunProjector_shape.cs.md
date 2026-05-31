# EconomyWebGlRunProjector target shape

```csharp
// Comments in production code must be in English.
public sealed class EconomyWebGlRunProjector : IEconomyWebGlRunProjector
{
    private readonly IEconomyWebGlInitialSceneProjector sceneProjector;
    private readonly IEconomyVisualActionWebGlMapper actionMapper;
    private readonly IWebGlRunActionPlanner actionPlanner;
    private readonly IWebGlRunActionPlanBatchCompiler batchCompiler;

    public WebGlRunDocument Project(EconomyWebGlRunInput input, EconomyWebGlProjectionOptions options)
    {
        EconomyWebGlProjectionValidation.ThrowIfInvalid(input, options);

        EconomyWebGlSceneProjection scene = sceneProjector.ProjectInitialScene(input);
        var document = CreateDocumentHeader(input, options, scene.Scene);

        foreach (EconomyVisualFrame visualFrame in input.Frames.OrderBy(frame => frame.StepIndex))
        {
            WebGlRunFrame frame = CreateFrameHeader(visualFrame);
            foreach (EconomyVisualAction action in ResolveFrameActions(input, visualFrame))
            {
                WebGlRunAction mapped = actionMapper.Map(action, scene.MappingContext);
                WebGlRunActionPlan plan = actionPlanner.Plan(mapped, scene.PlanningContext);
                WebGlSceneCommandBatch batch = batchCompiler.Compile(plan);
                frame.CommandBatches.Add(batch);
            }

            document.Timeline.Frames.Add(frame);
        }

        return document;
    }
}
```
