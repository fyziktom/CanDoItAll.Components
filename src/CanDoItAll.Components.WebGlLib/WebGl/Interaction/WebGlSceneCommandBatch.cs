namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneCommandBatch
{
    public string BatchId { get; set; } = string.Empty;

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public bool AllowDuplicateMotionsPerObject { get; set; }

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
            AllowDuplicateMotionsPerObject = batch.AllowDuplicateMotionsPerObject,
            Metadata = new Dictionary<string, string>(batch.Metadata, StringComparer.Ordinal)
        };
        var result = new WebGlSceneCommandBatchNormalizationResult
        {
            Batch = normalized,
            Metrics =
            {
                BatchCommandCount = batch.Patches.Count + batch.Motions.Count,
                EstimatedHostInteropCallCount = batch.Patches.Count + batch.Motions.Count > 0 ? 1 : 0
            }
        };

        normalized.Patches.AddRange(CoalescePatches(batch.Patches, result.Metrics));
        normalized.Motions.AddRange(DeduplicateMotions(batch.Motions, batch.AllowDuplicateMotionsPerObject, result));
        normalized.Metadata["batchCommandCount"] = result.Metrics.BatchCommandCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["coalescedPatchCount"] = result.Metrics.CoalescedPatchCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["droppedDuplicateMotionCount"] = result.Metrics.DroppedDuplicateMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        normalized.Metadata["estimatedHostInteropCallCount"] = result.Metrics.EstimatedHostInteropCallCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return result;
    }

    private static IReadOnlyList<WebGlScenePatch> CoalescePatches(IReadOnlyList<WebGlScenePatch> patches, WebGlSceneCommandBatchMetrics metrics)
    {
        if (patches.Count <= 1)
        {
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
        metrics.CoalescedPatchCount = Math.Max(0, originalPatchCount - objectPatches.Count);
        return [coalesced];
    }

    private static IReadOnlyList<WebGlObjectMotionCommand> DeduplicateMotions(
        IReadOnlyList<WebGlObjectMotionCommand> motions,
        bool allowDuplicateMotionsPerObject,
        WebGlSceneCommandBatchNormalizationResult result)
    {
        if (allowDuplicateMotionsPerObject)
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
}
