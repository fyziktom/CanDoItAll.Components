using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocumentRunner(IWebGlRunFrameApplier? frameApplier = null) : IWebGlRunDocumentRunner
{
    private readonly IWebGlRunFrameApplier? frameApplier = frameApplier;
    private WebGlRunPlaybackController? controller;
    private WebGlRunPlaybackResult? pendingPlaybackResult;
    private readonly HashSet<string> knownObjectIds = new(StringComparer.Ordinal);

    public WebGlRunExecutionState State { get; } = new();

    public async ValueTask<WebGlRunExecutionResult> LoadAsync(
        WebGlRunDocument document,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        ResetState(document);
        var result = CreateResult("load");
        controller = new WebGlRunPlaybackController(document);
        WebGlRunPlaybackResult playback = await controller.ApplyDetailedAsync(
            new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Reset },
            cancellationToken).ConfigureAwait(false);

        MergePlaybackResult(result, playback);
        if (!result.Succeeded)
        {
            return result;
        }

        try
        {
            if (frameApplier is IWebGlRunInitialSceneApplier initialSceneApplier)
            {
                await initialSceneApplier.ApplyInitialSceneAsync(document.InitialScene, cancellationToken).ConfigureAwait(false);
            }

            result.AppliedInitialScene = true;
            State.InitialSceneLoaded = true;
            pendingPlaybackResult = playback;
            UpdateStateFromPlayback(playback);
            AddSnapshotDiagnostics(result);
            return result;
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            result.Errors.Add($"Initial scene could not be applied: {error.Message}");
            State.Diagnostics["lastError"] = error.Message;
            return result;
        }
    }

    public async ValueTask<WebGlRunExecutionResult> SeekAsync(long frameIndex, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!EnsureLoaded(out WebGlRunExecutionResult result, "seek"))
        {
            return result;
        }

        WebGlRunPlaybackResult playback = await controller!.ApplyDetailedAsync(
            new WebGlRunPlaybackCommand
            {
                Kind = WebGlRunPlaybackCommandKinds.Seek,
                TargetFrameIndex = frameIndex
            },
            cancellationToken).ConfigureAwait(false);
        MergePlaybackResult(result, playback);
        if (result.Succeeded)
        {
            pendingPlaybackResult = playback;
            UpdateStateFromPlayback(playback);
        }

        AddSnapshotDiagnostics(result);
        return result;
    }

    public async ValueTask<WebGlRunExecutionResult> ApplyCurrentFrameAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!EnsureLoaded(out WebGlRunExecutionResult result, "apply-current-frame"))
        {
            return result;
        }

        WebGlRunPlaybackResult playback = pendingPlaybackResult ??
            await controller!.ApplyDetailedAsync(
                new WebGlRunPlaybackCommand
                {
                    Kind = WebGlRunPlaybackCommandKinds.Seek,
                    TargetFrameIndex = State.CurrentFrameIndex
                },
                cancellationToken).ConfigureAwait(false);
        MergePlaybackResult(result, playback);
        if (!result.Succeeded)
        {
            AddSnapshotDiagnostics(result);
            return result;
        }

        if (playback.RequiresSceneReset)
        {
            await ApplySceneResetAsync(result, cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        foreach (WebGlRunFrame frame in playback.FramesToApply)
        {
            WebGlRunExecutionResult validation = WebGlRunFrameExecutionValidator.ValidateFrame(frame, knownObjectIds);
            MergeExecutionResult(result, validation);
            if (!validation.Succeeded)
            {
                State.FailedStageIds.AddRange(validation.ExecutionDiagnostics.FailedMotionIds.Count > 0 ||
                                              validation.ExecutionDiagnostics.FailedPatchIds.Count > 0 ||
                                              validation.ExecutionDiagnostics.FailedLinkIds.Count > 0
                    ? WebGlRunFrameExecutionValidator.ResolveFailedStageIds(frame, validation.ExecutionDiagnostics)
                    : frame.Stages.Select(static stage => stage.StageId).Where(static id => !string.IsNullOrWhiteSpace(id)));
                SyncStateDiagnostics(result);
                return result;
            }

            WebGlRunFrameApplyResult frameResult = WebGlRunFrameApplyResult.FromFrame(frame);
            result.Warnings.AddRange(frameResult.Warnings);
            WebGlRunExecutionResultDiagnostics.CopyFrameApplyDiagnostics(frame, frameResult, result);

            try
            {
                if (frameApplier is not null)
                {
                    await frameApplier.ApplyAsync(frameResult, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                result.Errors.Add($"Frame '{frame.Index}' could not be applied: {error.Message}");
                State.FailedStageIds.AddRange(frame.Stages.Select(static stage => stage.StageId).Where(static id => !string.IsNullOrWhiteSpace(id)));
                SyncStateDiagnostics(result);
                return result;
            }

            WebGlRunFrameExecutionValidator.ApplyFrameObjectState(frame, knownObjectIds);
            IReadOnlyList<string> appliedStageIds = frame.Stages
                .Select(static stage => stage.StageId)
                .Where(static id => !string.IsNullOrWhiteSpace(id))
                .ToArray();
            result.AppliedStageIds.AddRange(appliedStageIds);
            State.CompletedStageIds.AddRange(appliedStageIds);
            State.CurrentFrameIndex = frame.Index;
        }

        State.PendingStageIds.Clear();
        pendingPlaybackResult = null;
        SyncStateDiagnostics(result);
        return result;
    }

    public async ValueTask<WebGlRunExecutionResult> StepForwardAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!EnsureLoaded(out WebGlRunExecutionResult result, "step-forward"))
        {
            return result;
        }

        WebGlRunPlaybackResult playback = await controller!.ApplyDetailedAsync(
            new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Step },
            cancellationToken).ConfigureAwait(false);
        MergePlaybackResult(result, playback);
        if (!result.Succeeded)
        {
            AddSnapshotDiagnostics(result);
            return result;
        }

        pendingPlaybackResult = playback;
        UpdateStateFromPlayback(playback);
        WebGlRunExecutionResult apply = await ApplyCurrentFrameAsync(cancellationToken).ConfigureAwait(false);
        apply.Operation = "step-forward";
        return apply;
    }

    private void ResetState(WebGlRunDocument document)
    {
        State.Document = document;
        State.RunId = document.RunId.Value;
        State.CurrentFrameIndex = 0;
        State.InitialSceneLoaded = false;
        State.CurrentCommandBatchId = string.Empty;
        State.ActiveStageIds.Clear();
        State.PendingStageIds.Clear();
        State.CompletedStageIds.Clear();
        State.FailedStageIds.Clear();
        State.SkippedStageIds.Clear();
        State.Diagnostics.Clear();
        State.ExecutionDiagnostics = new();
        knownObjectIds.Clear();
        foreach (WebGlSceneObject sceneObject in document.InitialScene.Scene.Objects)
        {
            if (!string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                knownObjectIds.Add(sceneObject.Id);
            }
        }
    }

    private async ValueTask ApplySceneResetAsync(WebGlRunExecutionResult result, CancellationToken cancellationToken)
    {
        knownObjectIds.Clear();
        foreach (WebGlSceneObject sceneObject in State.Document.InitialScene.Scene.Objects)
        {
            if (!string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                knownObjectIds.Add(sceneObject.Id);
            }
        }

        if (frameApplier is not IWebGlRunInitialSceneApplier initialSceneApplier)
        {
            result.AppliedInitialScene = true;
            return;
        }

        try
        {
            await initialSceneApplier.ApplyInitialSceneAsync(State.Document.InitialScene, cancellationToken).ConfigureAwait(false);
            result.AppliedInitialScene = true;
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            result.Errors.Add($"Initial scene could not be reset: {error.Message}");
        }
    }

    private void UpdateStateFromPlayback(WebGlRunPlaybackResult playback)
    {
        if (playback.CurrentFrame is not null)
        {
            State.CurrentFrameIndex = playback.CurrentFrame.Index;
            State.ActiveStageIds = playback.CurrentFrame.Stages
                .Select(static stage => stage.StageId)
                .Where(static id => !string.IsNullOrWhiteSpace(id))
                .ToList();
        }

        State.PendingStageIds = playback.FramesToApply
            .SelectMany(static frame => frame.Stages)
            .Select(static stage => stage.StageId)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToList();
        State.CurrentCommandBatchId = playback.State.CurrentCommandBatchId;
        State.Diagnostics["runId"] = State.RunId;
        State.Diagnostics["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["pendingStageCount"] = State.PendingStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private void SyncStateDiagnostics(WebGlRunExecutionResult result)
    {
        WebGlRunExecutionResultDiagnostics.SyncDiagnosticCounts(result);
        State.ExecutionDiagnostics = result.ExecutionDiagnostics;
        State.Diagnostics["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["completedStageCount"] = State.CompletedStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["failedStageCount"] = State.FailedStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["pendingStageCount"] = State.PendingStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        foreach (KeyValuePair<string, string> item in result.Diagnostics)
        {
            State.Diagnostics[item.Key] = item.Value;
        }
    }

    private void AddSnapshotDiagnostics(WebGlRunExecutionResult result)
    {
        if (controller is null)
        {
            return;
        }

        WebGlRunRuntimeSnapshot snapshot = controller.ExportRuntimeSnapshot();
        result.Diagnostics["runId"] = snapshot.RunId;
        result.Diagnostics["initialSceneId"] = snapshot.InitialSceneId;
        result.Diagnostics["initialObjectCount"] = snapshot.InitialObjectCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["queuedStageCount"] = snapshot.QueuedStageCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["currentStageIds"] = string.Join(",", snapshot.CurrentStageIds);
    }

    private bool EnsureLoaded(out WebGlRunExecutionResult result, string operation)
    {
        result = CreateResult(operation);
        if (controller is not null)
        {
            return true;
        }

        result.Errors.Add("A WebGL run document must be loaded before this operation.");
        return false;
    }

    private WebGlRunExecutionResult CreateResult(string operation)
        => new()
        {
            Operation = operation,
            CurrentFrameIndex = State.CurrentFrameIndex,
            Diagnostics =
            {
                ["runId"] = State.RunId,
                ["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        };

    private static void MergePlaybackResult(WebGlRunExecutionResult target, WebGlRunPlaybackResult playback)
    {
        target.Errors.AddRange(playback.Errors);
        target.Warnings.AddRange(playback.Warnings);
        target.CurrentFrameIndex = playback.CurrentFrame?.Index ?? playback.TargetFrameIndex;
        target.Diagnostics["playbackCommand"] = playback.RequestedCommand;
        target.Diagnostics["framesAppliedByController"] = playback.FramesApplied.ToString(System.Globalization.CultureInfo.InvariantCulture);
        target.Diagnostics["stagesQueued"] = playback.StagesQueued.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void MergeExecutionResult(WebGlRunExecutionResult target, WebGlRunExecutionResult source)
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
