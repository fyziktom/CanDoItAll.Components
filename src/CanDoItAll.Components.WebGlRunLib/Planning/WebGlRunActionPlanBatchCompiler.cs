using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionPlanBatchCompiler
{
    private readonly WebGlRunActionPlanBatchBuilder builder = new();

    public WebGlSceneCommandBatch Compile(WebGlRunActionPlan plan)
        => builder.Build(plan);
}
