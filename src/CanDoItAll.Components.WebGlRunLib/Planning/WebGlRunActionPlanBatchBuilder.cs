using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionPlanBatchBuilder
{
    private readonly WebGlRunActionCompiler compiler = new();

    public WebGlSceneCommandBatch Build(WebGlRunActionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (plan.Actions.Count == 0)
        {
            return WebGlSceneCommandBatchNormalizer.Normalize(new WebGlSceneCommandBatch
            {
                BatchId = ResolveBatchId(plan),
                OrderingMode = ResolveOrderingMode(plan),
                BatchingPolicy = plan.Metadata.GetValueOrDefault("batchingPolicy", WebGlSceneBatchingPolicies.PreserveOrder),
                Patches = [.. plan.Patches],
                Motions = [.. plan.Motions],
                Metadata = new Dictionary<string, string>(plan.Metadata, StringComparer.Ordinal)
                {
                    ["actionId"] = plan.ActionId
                }
            }).Batch;
        }

        WebGlRunTimeline timeline = compiler.Compile(plan);
        List<WebGlSceneCommandBatch> frameBatches = [.. timeline.Frames.Select(frame => WebGlRunFrameApplyResult.FromFrame(frame).CommandBatch)];
        if (frameBatches.Count == 1)
        {
            WebGlSceneCommandBatch batch = frameBatches[0];
            batch.BatchId = ResolveBatchId(plan);
            batch.Metadata["actionId"] = plan.ActionId;
            return batch;
        }

        return WebGlSceneCommandBatchNormalizer.Normalize(new WebGlSceneCommandBatch
        {
            BatchId = ResolveBatchId(plan),
            OrderingMode = BatchOrderingMode.Sequential,
            BatchingPolicy = WebGlSceneBatchingPolicies.PreserveOrder,
            Stages = [.. frameBatches.SelectMany(static batch => batch.Stages)],
            Metadata = new Dictionary<string, string>(plan.Metadata, StringComparer.Ordinal)
            {
                ["actionId"] = plan.ActionId,
                ["frameBatchCount"] = frameBatches.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        }).Batch;
    }

    private static string ResolveBatchId(WebGlRunActionPlan plan)
        => string.IsNullOrWhiteSpace(plan.ActionId) ? "run-plan" : $"run-plan:{plan.ActionId}";

    private static BatchOrderingMode ResolveOrderingMode(WebGlRunActionPlan plan)
        => plan.Metadata.TryGetValue("orderingMode", out string? value) &&
           Enum.TryParse(value, ignoreCase: true, out BatchOrderingMode parsed)
            ? parsed
            : BatchOrderingMode.PreserveOrder;
}

