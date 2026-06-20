namespace CanDoItAll.Components.WebGlRunLib;

internal static class WebGlRunExecutionResultDiagnostics
{
    public static void CopyFrameApplyDiagnostics(
        WebGlRunFrame frame,
        WebGlRunFrameApplyResult frameResult,
        WebGlRunExecutionResult result)
    {
        result.Diagnostics["commandBatchId"] = frameResult.CommandBatch.BatchId;
        result.Diagnostics["frameIndex"] = frame.Index.ToString(System.Globalization.CultureInfo.InvariantCulture);
        foreach (KeyValuePair<string, string> item in frameResult.CommandBatch.Metadata)
        {
            result.Diagnostics[item.Key] = item.Value;
        }

        if (frame.Metadata.TryGetValue("sourceFrameId", out string? sourceFrameId) &&
            !string.IsNullOrWhiteSpace(sourceFrameId))
        {
            result.Diagnostics["sourceFrameId"] = sourceFrameId;
        }

        var sourceStageIds = WebGlRunStageOrderingPolicy.OrderStages(frame)
            .Select(static stage => stage.Metadata.GetValueOrDefault("sourceStageId"))
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        if (sourceStageIds.Length > 0)
        {
            result.Diagnostics["sourceStageIds"] = string.Join(",", sourceStageIds);
        }
    }

    public static void SyncDiagnosticCounts(WebGlRunExecutionResult result)
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

    public static void Merge(WebGlRunExecutionResult target, WebGlRunExecutionResult source)
    {
        target.Errors.AddRange(source.Errors);
        target.Warnings.AddRange(source.Warnings);
        target.ExecutionDiagnostics.UnresolvedObjectIds.AddRange(source.ExecutionDiagnostics.UnresolvedObjectIds);
        target.ExecutionDiagnostics.FailedMotionIds.AddRange(source.ExecutionDiagnostics.FailedMotionIds);
        target.ExecutionDiagnostics.FailedPatchIds.AddRange(source.ExecutionDiagnostics.FailedPatchIds);
        target.ExecutionDiagnostics.FailedLinkIds.AddRange(source.ExecutionDiagnostics.FailedLinkIds);
        target.ExecutionDiagnostics.SourceFrameIds.AddRange(source.ExecutionDiagnostics.SourceFrameIds);
        target.ExecutionDiagnostics.SourceStageIds.AddRange(source.ExecutionDiagnostics.SourceStageIds);
        foreach (KeyValuePair<string, string> item in source.Diagnostics)
        {
            target.Diagnostics[item.Key] = item.Value;
        }
    }
}
