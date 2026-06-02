using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

internal static class WebGlRunFrameExecutionValidator
{
    public static WebGlRunExecutionResult ValidateFrame(WebGlRunFrame frame, HashSet<string> knownObjectIds)
    {
        var result = new WebGlRunExecutionResult { Operation = "validate-frame" };
        var idsAfterStage = new HashSet<string>(knownObjectIds, StringComparer.Ordinal);
        if (frame.Metadata.TryGetValue("sourceFrameId", out string? sourceFrameId) &&
            !string.IsNullOrWhiteSpace(sourceFrameId))
        {
            result.ExecutionDiagnostics.SourceFrameIds.Add(sourceFrameId);
            result.Diagnostics["sourceFrameId"] = sourceFrameId;
        }

        foreach (WebGlRunActionStage stage in frame.Stages)
        {
            if (stage.Metadata.TryGetValue("sourceStageId", out string? sourceStageId) &&
                !string.IsNullOrWhiteSpace(sourceStageId))
            {
                result.ExecutionDiagnostics.SourceStageIds.Add(sourceStageId);
            }

            ValidateMotions(stage.StageId, stage.Motions, idsAfterStage, result);
            foreach (WebGlRunFramePatch stagePatch in stage.ScenePatches)
            {
                ValidatePatch(stage, stagePatch, idsAfterStage, result);
                ApplyPatchObjectIds(stagePatch.Patch, idsAfterStage);
            }
        }

        ValidateMotions("frame", frame.Motions, idsAfterStage, result);
        foreach (WebGlRunFramePatch patch in frame.ScenePatches)
        {
            ValidatePatch(null, patch, idsAfterStage, result);
            ApplyPatchObjectIds(patch.Patch, idsAfterStage);
        }

        if (result.ExecutionDiagnostics.SourceStageIds.Count > 0)
        {
            result.Diagnostics["sourceStageIds"] = string.Join(",", result.ExecutionDiagnostics.SourceStageIds);
        }

        SyncDiagnosticCounts(result);
        return result;
    }

    public static void ApplyFrameObjectState(WebGlRunFrame frame, HashSet<string> knownObjectIds)
    {
        foreach (WebGlRunActionStage stage in frame.Stages)
        {
            foreach (WebGlRunFramePatch patch in stage.ScenePatches)
            {
                ApplyPatchObjectIds(patch.Patch, knownObjectIds);
            }
        }

        foreach (WebGlRunFramePatch patch in frame.ScenePatches)
        {
            ApplyPatchObjectIds(patch.Patch, knownObjectIds);
        }
    }

    public static IReadOnlyList<string> ResolveFailedStageIds(WebGlRunFrame frame, WebGlRunExecutionDiagnostics diagnostics)
    {
        var failedIds = new List<string>();
        foreach (WebGlRunActionStage stage in frame.Stages)
        {
            bool stageFailed = stage.Motions.Any(motion => diagnostics.FailedMotionIds.Contains(motion.MotionId, StringComparer.Ordinal)) ||
                               stage.ScenePatches.Any(patch => diagnostics.FailedPatchIds.Contains(patch.Id, StringComparer.Ordinal)) ||
                               stage.ScenePatches.Any(patch => patch.Patch.AddLinks.Any(link => diagnostics.FailedLinkIds.Contains(link.Id, StringComparer.Ordinal)));
            if (stageFailed && !string.IsNullOrWhiteSpace(stage.StageId))
            {
                failedIds.Add(stage.StageId);
            }
        }

        return failedIds;
    }

    private static void ValidateMotions(
        string stageId,
        IEnumerable<WebGlObjectMotionCommand> motions,
        HashSet<string> objectIds,
        WebGlRunExecutionResult result)
    {
        foreach (WebGlObjectMotionCommand motion in motions)
        {
            if (string.IsNullOrWhiteSpace(motion.ObjectId) || objectIds.Contains(motion.ObjectId))
            {
                continue;
            }

            result.Errors.Add($"Stage '{stageId}' motion '{motion.MotionId}' targets unresolved object '{motion.ObjectId}'.");
            result.ExecutionDiagnostics.UnresolvedObjectIds.Add(motion.ObjectId);
            result.ExecutionDiagnostics.FailedMotionIds.Add(string.IsNullOrWhiteSpace(motion.MotionId) ? stageId : motion.MotionId);
        }
    }

    private static void ValidatePatch(
        WebGlRunActionStage? stage,
        WebGlRunFramePatch framePatch,
        HashSet<string> objectIds,
        WebGlRunExecutionResult result)
    {
        string stageId = stage?.StageId ?? "frame";
        foreach (WebGlSceneObjectPatch objectPatch in framePatch.Patch.ObjectPatches)
        {
            if (string.IsNullOrWhiteSpace(objectPatch.ObjectId) || objectIds.Contains(objectPatch.ObjectId))
            {
                continue;
            }

            result.Errors.Add($"Stage '{stageId}' patch '{framePatch.Id}' targets unresolved object '{objectPatch.ObjectId}'.");
            result.ExecutionDiagnostics.UnresolvedObjectIds.Add(objectPatch.ObjectId);
            result.ExecutionDiagnostics.FailedPatchIds.Add(string.IsNullOrWhiteSpace(framePatch.Id) ? stageId : framePatch.Id);
        }

        foreach (string objectId in framePatch.Patch.RemoveObjectIds)
        {
            if (string.IsNullOrWhiteSpace(objectId) || objectIds.Contains(objectId))
            {
                continue;
            }

            result.Warnings.Add($"Stage '{stageId}' patch '{framePatch.Id}' removes object '{objectId}' that is not currently known.");
        }

        var candidateIds = new HashSet<string>(objectIds, StringComparer.Ordinal);
        foreach (WebGlSceneObject sceneObject in framePatch.Patch.AddObjects)
        {
            if (!string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                candidateIds.Add(sceneObject.Id);
            }
        }

        foreach (WebGlSceneLink link in framePatch.Patch.AddLinks)
        {
            if (candidateIds.Contains(link.SourceObjectId) && candidateIds.Contains(link.TargetObjectId))
            {
                continue;
            }

            result.Errors.Add($"Stage '{stageId}' link '{link.Id}' has unresolved endpoints '{link.SourceObjectId}' -> '{link.TargetObjectId}'.");
            result.ExecutionDiagnostics.FailedLinkIds.Add(string.IsNullOrWhiteSpace(link.Id) ? stageId : link.Id);
            AddMissingEndpoint(link.SourceObjectId, candidateIds, result);
            AddMissingEndpoint(link.TargetObjectId, candidateIds, result);
        }
    }

    private static void AddMissingEndpoint(string objectId, HashSet<string> candidateIds, WebGlRunExecutionResult result)
    {
        if (!string.IsNullOrWhiteSpace(objectId) && !candidateIds.Contains(objectId))
        {
            result.ExecutionDiagnostics.UnresolvedObjectIds.Add(objectId);
        }
    }

    private static void ApplyPatchObjectIds(WebGlScenePatch patch, HashSet<string> objectIds)
    {
        foreach (string objectId in patch.RemoveObjectIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            objectIds.Remove(objectId);
        }

        foreach (WebGlSceneObject sceneObject in patch.AddObjects)
        {
            if (!string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                objectIds.Add(sceneObject.Id);
            }
        }
    }

    private static void SyncDiagnosticCounts(WebGlRunExecutionResult result)
    {
        result.ExecutionDiagnostics.UnresolvedObjectIds = [.. result.ExecutionDiagnostics.UnresolvedObjectIds.Distinct(StringComparer.Ordinal)];
        result.ExecutionDiagnostics.FailedMotionIds = [.. result.ExecutionDiagnostics.FailedMotionIds.Distinct(StringComparer.Ordinal)];
        result.ExecutionDiagnostics.FailedPatchIds = [.. result.ExecutionDiagnostics.FailedPatchIds.Distinct(StringComparer.Ordinal)];
        result.ExecutionDiagnostics.FailedLinkIds = [.. result.ExecutionDiagnostics.FailedLinkIds.Distinct(StringComparer.Ordinal)];
        result.ExecutionDiagnostics.FailedMotionCount = result.ExecutionDiagnostics.FailedMotionIds.Count;
        result.ExecutionDiagnostics.FailedPatchCount = result.ExecutionDiagnostics.FailedPatchIds.Count;
        result.ExecutionDiagnostics.FailedLinkCount = result.ExecutionDiagnostics.FailedLinkIds.Count;
        result.Diagnostics["failedMotionCount"] = result.ExecutionDiagnostics.FailedMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["failedPatchCount"] = result.ExecutionDiagnostics.FailedPatchCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["failedLinkCount"] = result.ExecutionDiagnostics.FailedLinkCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["unresolvedObjectIds"] = string.Join(",", result.ExecutionDiagnostics.UnresolvedObjectIds);
    }
}
