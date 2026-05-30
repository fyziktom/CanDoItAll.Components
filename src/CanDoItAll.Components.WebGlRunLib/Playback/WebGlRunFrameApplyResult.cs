using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunFrameApplyResult
{
    public long FrameIndex { get; set; }

    public double TimeSeconds { get; set; }

    public WebGlSceneCommandBatch CommandBatch { get; set; } = new();

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public static WebGlRunFrameApplyResult FromFrame(WebGlRunFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        WebGlSceneCommandBatchNormalizationResult normalized = WebGlSceneCommandBatchNormalizer.Normalize(new WebGlSceneCommandBatch
        {
            BatchId = $"run-frame:{frame.Index}",
            OrderingMode = ResolveOrderingMode(frame),
            BatchingPolicy = frame.Metadata.GetValueOrDefault("batchingPolicy", WebGlSceneBatchingPolicies.PreserveOrder),
            Stages =
            [
                .. frame.Stages
                    .OrderBy(static stage => stage.StartsAtSeconds)
                    .ThenBy(static stage => stage.StageIndex < 0 ? int.MaxValue : stage.StageIndex)
                    .ThenBy(static stage => stage.OrderIndex < 0 ? int.MaxValue : stage.OrderIndex)
                    .ThenBy(static stage => stage.StageId, StringComparer.Ordinal)
                    .Select(stage => new WebGlSceneCommandBatchStage
                    {
                        StageId = stage.StageId,
                        OrderingMode = ResolveOrderingMode(stage),
                        BatchingPolicy = ResolveBatchingPolicy(stage),
                        Patches = [.. stage.ScenePatches.Select(item => item.Patch)],
                        Motions = [.. stage.Motions],
                        WaitSeconds = stage.WaitSeconds,
                        Metadata = new Dictionary<string, string>(stage.Metadata, StringComparer.Ordinal)
                    })
            ],
            Patches = frame.Stages.Count == 0 ? [.. frame.ScenePatches.Select(item => item.Patch)] : [],
            Motions = frame.Stages.Count == 0 ? [.. frame.Motions] : [],
            Metadata =
            {
                ["frameIndex"] = frame.Index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["timeSeconds"] = frame.TimeSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        });

        return new()
        {
            FrameIndex = frame.Index,
            TimeSeconds = frame.TimeSeconds,
            CommandBatch = normalized.Batch,
            Warnings = [.. normalized.Warnings]
        };
    }

    private static BatchOrderingMode ResolveOrderingMode(WebGlRunActionStage stage)
    {
        string policy = ResolveBatchingPolicy(stage);
        if (string.Equals(policy, WebGlSceneBatchingPolicies.PreserveOrder, StringComparison.OrdinalIgnoreCase))
        {
            return BatchOrderingMode.PreserveOrder;
        }

        if (stage.Metadata.TryGetValue("orderingMode", out string? value) &&
            Enum.TryParse(value, ignoreCase: true, out BatchOrderingMode parsed))
        {
            return parsed;
        }

        return stage.Motions.GroupBy(static motion => motion.ObjectId, StringComparer.Ordinal).Any(static group => group.Count() > 1)
            ? BatchOrderingMode.Sequential
            : BatchOrderingMode.CoalesceIndependent;
    }

    private static string ResolveBatchingPolicy(WebGlRunActionStage stage)
        => !string.IsNullOrWhiteSpace(stage.ExecutionPolicy)
            ? stage.ExecutionPolicy
            : stage.Metadata.GetValueOrDefault("batchingPolicy", WebGlSceneBatchingPolicies.PreserveOrder);

    private static BatchOrderingMode ResolveOrderingMode(WebGlRunFrame frame)
    {
        if (frame.Metadata.TryGetValue("orderingMode", out string? value) &&
            Enum.TryParse(value, ignoreCase: true, out BatchOrderingMode parsed))
        {
            return parsed;
        }

        return frame.Stages.Count > 0 ||
               frame.Motions.GroupBy(static motion => motion.ObjectId, StringComparer.Ordinal).Any(static group => group.Count() > 1)
            ? BatchOrderingMode.Sequential
            : BatchOrderingMode.CoalesceIndependent;
    }
}
