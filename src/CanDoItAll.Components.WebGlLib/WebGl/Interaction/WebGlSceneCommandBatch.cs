namespace CanDoItAll.Components.WebGlLib;

public enum BatchOrderingMode
{
    CoalesceIndependent = 0,
    PreserveOrder = 1,
    Sequential = 2
}

public sealed class WebGlSceneCommandBatch
{
    public string BatchId { get; set; } = string.Empty;

    public BatchOrderingMode OrderingMode { get; set; } = BatchOrderingMode.CoalesceIndependent;

    public List<WebGlSceneCommandBatchStage> Stages { get; set; } = [];

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public bool AllowDuplicateMotionsPerObject { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneCommandBatchStage
{
    public string StageId { get; set; } = string.Empty;

    public BatchOrderingMode OrderingMode { get; set; } = BatchOrderingMode.CoalesceIndependent;

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public bool AllowDuplicateMotionsPerObject { get; set; }

    public double WaitSeconds { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneCommandBatchResult : WebGlSceneCommandResult
{
    public List<WebGlSceneCommandResult> CommandResults { get; set; } = [];

    public WebGlSceneProofSnapshot? ProofSnapshot { get; set; }

    public WebGlSceneCommandBatchMetrics Metrics { get; set; } = new();
}

public sealed class WebGlSceneCommandBatchMetrics
{
    public int BatchCommandCount { get; set; }

    public int StageCount { get; set; }

    public int BatchDurationMs { get; set; }

    public int CoalescedPatchCount { get; set; }

    public int DroppedDuplicateMotionCount { get; set; }

    public int EstimatedHostInteropCallCount { get; set; }
}

public sealed class WebGlSceneCommandBatchNormalizationResult
{
    public WebGlSceneCommandBatch Batch { get; set; } = new();

    public WebGlSceneCommandBatchMetrics Metrics { get; set; } = new();

    public List<string> Warnings { get; set; } = [];
}

public static class WebGlSceneCommandBatchNormalizer
{
    public static WebGlSceneCommandBatchNormalizationResult Normalize(WebGlSceneCommandBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        var normalized = new WebGlSceneCommandBatch
        {
            BatchId = batch.BatchId,
            OrderingMode = batch.OrderingMode,
            AllowDuplicateMotionsPerObject = batch.AllowDuplicateMotionsPerObject,
            Metadata = new Dictionary<string, string>(batch.Metadata, StringComparer.Ordinal)
        };
        var result = new WebGlSceneCommandBatchNormalizationResult
        {
            Batch = normalized,
            Metrics =
            {
                BatchCommandCount = CountCommands(batch),
                StageCount = batch.Stages.Count,
                EstimatedHostInteropCallCount = CountCommands(batch) > 0 ? 1 : 0
            }
        };

        normalized.Patches.AddRange(CoalescePatches(batch.Patches, batch.Motions.Count > 0, batch.OrderingMode, result));
        normalized.Motions.AddRange(DeduplicateMotions(batch.Motions, batch.AllowDuplicateMotionsPerObject, batch.OrderingMode, result));
        foreach (WebGlSceneCommandBatchStage stage in batch.Stages)
        {
            normalized.Stages.Add(NormalizeStage(stage, batch, result));
        }

        normalized.Metadata["orderingMode"] = batch.OrderingMode.ToString();
        normalized.Metadata["batchCommandCount"] = result.Metrics.BatchCommandCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["stageCount"] = result.Metrics.StageCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["coalescedPatchCount"] = result.Metrics.CoalescedPatchCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["droppedDuplicateMotionCount"] = result.Metrics.DroppedDuplicateMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["estimatedHostInteropCallCount"] = result.Metrics.EstimatedHostInteropCallCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return result;
    }

    private static WebGlSceneCommandBatchStage NormalizeStage(
        WebGlSceneCommandBatchStage stage,
        WebGlSceneCommandBatch parent,
        WebGlSceneCommandBatchNormalizationResult result)
    {
        BatchOrderingMode orderingMode = stage.OrderingMode;
        var normalized = new WebGlSceneCommandBatchStage
        {
            StageId = stage.StageId,
            OrderingMode = orderingMode,
            AllowDuplicateMotionsPerObject = stage.AllowDuplicateMotionsPerObject || parent.AllowDuplicateMotionsPerObject,
            WaitSeconds = stage.WaitSeconds,
            Metadata = new Dictionary<string, string>(stage.Metadata, StringComparer.Ordinal)
        };
        var stageResult = new WebGlSceneCommandBatchNormalizationResult
        {
            Batch = new WebGlSceneCommandBatch { BatchId = $"{parent.BatchId}:{stage.StageId}" }
        };

        normalized.Patches.AddRange(CoalescePatches(stage.Patches, stage.Motions.Count > 0, orderingMode, stageResult));
        normalized.Motions.AddRange(DeduplicateMotions(
            stage.Motions,
            normalized.AllowDuplicateMotionsPerObject,
            orderingMode,
            stageResult));
        normalized.Metadata["orderingMode"] = orderingMode.ToString();
        normalized.Metadata["stageCommandCount"] = (stage.Patches.Count + stage.Motions.Count).ToString(System.Globalization.CultureInfo.InvariantCulture);

        result.Metrics.CoalescedPatchCount += stageResult.Metrics.CoalescedPatchCount;
        result.Metrics.DroppedDuplicateMotionCount += stageResult.Metrics.DroppedDuplicateMotionCount;
        result.Warnings.AddRange(stageResult.Warnings);
        return normalized;
    }

    private static int CountCommands(WebGlSceneCommandBatch batch)
        => batch.Patches.Count +
           batch.Motions.Count +
           batch.Stages.Sum(static stage => stage.Patches.Count + stage.Motions.Count);

    private static IReadOnlyList<WebGlScenePatch> CoalescePatches(
        IReadOnlyList<WebGlScenePatch> patches,
        bool containsMotions,
        BatchOrderingMode orderingMode,
        WebGlSceneCommandBatchNormalizationResult result)
    {
        if (patches.Count <= 1 || orderingMode is BatchOrderingMode.PreserveOrder or BatchOrderingMode.Sequential)
        {
            return patches;
        }

        if (!CanCoalescePatches(patches, containsMotions))
        {
            result.Warnings.Add($"Patch coalescing was skipped for batch '{result.Batch.BatchId}' because the patch set has ordered semantics.");
            return patches;
        }

        var coalesced = new WebGlScenePatch();
        var objectPatches = new Dictionary<string, WebGlSceneObjectPatch>(StringComparer.Ordinal);
        var originalPatchCount = 0;

        foreach (WebGlScenePatch patch in patches)
        {
            coalesced.SceneId = string.IsNullOrWhiteSpace(coalesced.SceneId) ? patch.SceneId : coalesced.SceneId;
            coalesced.BaseRevision = coalesced.BaseRevision == 0 ? patch.BaseRevision : coalesced.BaseRevision;
            coalesced.NextRevision = patch.NextRevision != 0 ? patch.NextRevision : coalesced.NextRevision;
            coalesced.AddObjects.AddRange(patch.AddObjects);
            coalesced.RemoveObjectIds.AddRange(patch.RemoveObjectIds);
            coalesced.AddLinks.AddRange(patch.AddLinks);
            coalesced.RemoveLinkIds.AddRange(patch.RemoveLinkIds);
            foreach (KeyValuePair<string, string> item in patch.Metadata)
            {
                coalesced.Metadata[item.Key] = item.Value;
            }

            foreach (WebGlSceneObjectPatch objectPatch in patch.ObjectPatches)
            {
                originalPatchCount++;
                if (string.IsNullOrWhiteSpace(objectPatch.ObjectId))
                {
                    coalesced.ObjectPatches.Add(objectPatch);
                    continue;
                }

                if (!objectPatches.TryGetValue(objectPatch.ObjectId, out WebGlSceneObjectPatch? existing))
                {
                    objectPatches[objectPatch.ObjectId] = CloneObjectPatch(objectPatch);
                }
                else
                {
                    MergeObjectPatch(existing, objectPatch);
                }
            }
        }

        coalesced.ObjectPatches.AddRange(objectPatches.Values);
        result.Metrics.CoalescedPatchCount = Math.Max(0, originalPatchCount - objectPatches.Count);
        return [coalesced];
    }

    private static bool CanCoalescePatches(IReadOnlyList<WebGlScenePatch> patches, bool containsMotions)
    {
        var baseRevision = 0;
        var nextRevision = 0;
        foreach (WebGlScenePatch patch in patches)
        {
            if (patch.AddObjects.Count > 0 ||
                patch.RemoveObjectIds.Count > 0 ||
                patch.AddLinks.Count > 0 ||
                patch.RemoveLinkIds.Count > 0)
            {
                return false;
            }

            if (IsTruthy(patch.Metadata.GetValueOrDefault("preserveOrder")) ||
                IsTruthy(patch.Metadata.GetValueOrDefault("requiresOrderedSemantics")) ||
                IsTruthy(patch.Metadata.GetValueOrDefault("dependsOnIntermediateState")))
            {
                return false;
            }

            if (patch.BaseRevision > 0)
            {
                baseRevision = baseRevision == 0 ? patch.BaseRevision : baseRevision;
                if (baseRevision != patch.BaseRevision)
                {
                    return false;
                }
            }

            if (patch.NextRevision > 0)
            {
                nextRevision = nextRevision == 0 ? patch.NextRevision : nextRevision;
                if (nextRevision != patch.NextRevision)
                {
                    return false;
                }
            }

            if (containsMotions && patch.ObjectPatches.Any(static item =>
                    item.AssetId is not null ||
                    item.Symbols is not null ||
                    item.Metadata?.ContainsKey("poseKey") == true))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<WebGlObjectMotionCommand> DeduplicateMotions(
        IReadOnlyList<WebGlObjectMotionCommand> motions,
        bool allowDuplicateMotionsPerObject,
        BatchOrderingMode orderingMode,
        WebGlSceneCommandBatchNormalizationResult result)
    {
        if (allowDuplicateMotionsPerObject || orderingMode is BatchOrderingMode.PreserveOrder or BatchOrderingMode.Sequential)
        {
            return motions;
        }

        var seenObjectIds = new HashSet<string>(StringComparer.Ordinal);
        List<WebGlObjectMotionCommand> deduplicated = [];
        foreach (WebGlObjectMotionCommand motion in motions)
        {
            if (string.IsNullOrWhiteSpace(motion.ObjectId) || seenObjectIds.Add(motion.ObjectId))
            {
                deduplicated.Add(motion);
            }
            else
            {
                result.Metrics.DroppedDuplicateMotionCount++;
                result.Warnings.Add($"Duplicate motion for object '{motion.ObjectId}' was dropped from batch '{result.Batch.BatchId}'.");
            }
        }

        return deduplicated;
    }

    private static WebGlSceneObjectPatch CloneObjectPatch(WebGlSceneObjectPatch patch)
        => new()
        {
            ObjectId = patch.ObjectId,
            Position = patch.Position,
            Rotation = patch.Rotation,
            Scale = patch.Scale,
            Size = patch.Size,
            AssetId = patch.AssetId,
            Color = patch.Color,
            Symbols = patch.Symbols is null ? null : [.. patch.Symbols],
            Metadata = patch.Metadata is null ? null : new Dictionary<string, string>(patch.Metadata, StringComparer.Ordinal)
        };

    private static void MergeObjectPatch(WebGlSceneObjectPatch target, WebGlSceneObjectPatch source)
    {
        target.Position = source.Position ?? target.Position;
        target.Rotation = source.Rotation ?? target.Rotation;
        target.Scale = source.Scale ?? target.Scale;
        target.Size = source.Size ?? target.Size;
        target.AssetId = source.AssetId ?? target.AssetId;
        target.Color = source.Color ?? target.Color;
        target.Symbols = source.Symbols ?? target.Symbols;
        if (source.Metadata is not null)
        {
            target.Metadata ??= [];
            foreach (KeyValuePair<string, string> item in source.Metadata)
            {
                target.Metadata[item.Key] = item.Value;
            }
        }
    }

    private static bool IsTruthy(string? value)
        => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
}
