using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunFrameApplyResult
{
    public long FrameIndex { get; set; }

    public double TimeSeconds { get; set; }

    public bool RequiresSceneReset { get; set; }

    public WebGlSceneDocument? InitialScene { get; set; }

    public WebGlSceneCommandBatch CommandBatch { get; set; } = new();

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public static WebGlRunFrameApplyResult FromFrame(WebGlRunFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        bool hasMixedCommands = WebGlRunFrameCommandPolicy.HasMixedDirectAndStagedCommands(frame);
        if (hasMixedCommands)
        {
            return new()
            {
                FrameIndex = frame.Index,
                TimeSeconds = frame.TimeSeconds,
                CommandBatch = new()
                {
                    BatchId = $"run-frame:{frame.Index}",
                    Metadata =
                    {
                        ["frameIndex"] = frame.Index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["timeSeconds"] = frame.TimeSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        ["blockedByPolicy"] = "mixed-direct-and-staged-commands"
                    }
                },
                Errors = [WebGlRunFrameCommandPolicy.CreateMixedDirectAndStagedCommandsError(frame.Index)]
            };
        }

        WebGlSceneCommandBatchNormalizationResult normalized = WebGlSceneCommandBatchNormalizer.Normalize(new WebGlSceneCommandBatch
        {
            BatchId = $"run-frame:{frame.Index}",
            OrderingMode = ResolveOrderingMode(frame),
            BatchingPolicy = frame.Metadata.GetValueOrDefault("batchingPolicy", WebGlSceneBatchingPolicies.PreserveOrder),
            Stages =
            [
                .. WebGlRunStageOrderingPolicy.OrderStages(frame)
                    .Select(stage => new WebGlSceneCommandBatchStage
                    {
                        StageId = stage.StageId,
                        OrderingMode = ResolveOrderingMode(stage),
                        BatchingPolicy = ResolveBatchingPolicy(stage),
                        Patches = [.. stage.ScenePatches.Select(item => item.Patch)],
                        Motions = [.. stage.Motions],
                        WaitSeconds = stage.WaitSeconds,
                        BarrierPolicy = stage.BarrierPolicy,
                        BarrierObjectIds = [.. stage.BarrierObjectIds],
                        BarrierEventId = stage.BarrierEventId,
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
        => stage.CoalescingScope switch
        {
            WebGlRunCoalescingScopes.None => WebGlSceneBatchingPolicies.PreserveOrder,
            WebGlRunCoalescingScopes.Frame => WebGlSceneBatchingPolicies.Parallel,
            _ => !string.IsNullOrWhiteSpace(stage.ExecutionPolicy)
                ? stage.ExecutionPolicy
                : stage.Metadata.GetValueOrDefault("batchingPolicy", WebGlSceneBatchingPolicies.PreserveOrder)
        };

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
