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
            try
            {
                await ApplySceneResetAsync(result, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return CompleteCanceledOperation(result, "scene reset canceled", State.PendingStageIds);
            }

            if (!result.Succeeded)
            {
                return result;
            }
        }

        foreach (WebGlRunFrame frame in playback.FramesToApply)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CompleteCanceledOperation(result, "frame apply canceled", OrderedStageIds(frame));
            }

            WebGlRunExecutionResult validation = WebGlRunFrameExecutionValidator.ValidateFrame(frame, knownObjectIds);
            WebGlRunExecutionResultDiagnostics.Merge(result, validation);
            if (!validation.Succeeded)
            {
                State.FailedStageIds.AddRange(validation.ExecutionDiagnostics.FailedMotionIds.Count > 0 ||
                                              validation.ExecutionDiagnostics.FailedPatchIds.Count > 0 ||
                                              validation.ExecutionDiagnostics.FailedLinkIds.Count > 0
                    ? WebGlRunFrameExecutionValidator.ResolveFailedStageIds(frame, validation.ExecutionDiagnostics)
                    : OrderedStageIds(frame));
                SyncStateDiagnostics(result);
                return result;
            }

            WebGlRunFrameApplyResult frameResult = WebGlRunFrameApplyResult.FromFrame(frame);
            result.Errors.AddRange(frameResult.Errors);
            result.Warnings.AddRange(frameResult.Warnings);
            WebGlRunExecutionResultDiagnostics.CopyFrameApplyDiagnostics(frame, frameResult, result);
            if (frameResult.Errors.Count > 0)
            {
                State.FailedStageIds.AddRange(OrderedStageIds(frame));
                SyncStateDiagnostics(result);
                return result;
            }

            try
            {
                if (frameApplier is not null)
                {
                    await frameApplier.ApplyAsync(frameResult, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                return CompleteCanceledOperation(result, "frame apply canceled", OrderedStageIds(frame));
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                result.Errors.Add($"Frame '{frame.Index}' could not be applied: {error.Message}");
                State.FailedStageIds.AddRange(OrderedStageIds(frame));
                SyncStateDiagnostics(result);
                return result;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return CompleteCanceledOperation(result, "frame apply canceled", OrderedStageIds(frame));
            }

            WebGlRunFrameExecutionValidator.ApplyFrameObjectState(frame, knownObjectIds);
            IReadOnlyList<string> appliedStageIds = OrderedStageIds(frame);
            result.AppliedStageIds.AddRange(appliedStageIds);
            State.CompletedStageIds.AddRange(appliedStageIds);
            State.CurrentFrameIndex = frame.Index;
        }

        State.PendingStageIds.Clear();
        pendingPlaybackResult = null;
        State.PlaybackLifecycleState = WebGlRunPlaybackLifecycleStates.Idle;
        State.LastPlaybackCommandKind = result.Operation;
        State.LastPlaybackStopReason = string.Empty;
        result.PlaybackLifecycleState = State.PlaybackLifecycleState;
        result.PlaybackLifecycleReason = State.LastPlaybackStopReason;
        result.Diagnostics["playbackLifecycleState"] = State.PlaybackLifecycleState;
        result.Diagnostics["lastPlaybackCommandKind"] = State.LastPlaybackCommandKind;
        result.Diagnostics["lastPlaybackStopReason"] = State.LastPlaybackStopReason;
        SyncStateDiagnostics(result);
        return result;
    }

    public ValueTask<WebGlRunExecutionResult> StepForwardAsync(CancellationToken cancellationToken = default)
        => StepAsync(WebGlRunPlaybackCommandKinds.Step, "step-forward", cancellationToken);
    public ValueTask<WebGlRunExecutionResult> StepBackwardAsync(CancellationToken cancellationToken = default)
        => StepAsync(WebGlRunPlaybackCommandKinds.Previous, "step-backward", cancellationToken);

    public ValueTask<WebGlRunExecutionResult> PauseAsync(string reason = "paused", CancellationToken cancellationToken = default)
        => ApplyLifecycleControlAsync(WebGlRunPlaybackCommandKinds.Pause, "pause", WebGlRunPlaybackLifecycleStates.Paused, reason, cancellationToken);

    public ValueTask<WebGlRunExecutionResult> CancelAsync(string reason = "canceled", CancellationToken cancellationToken = default)
        => ApplyLifecycleControlAsync(WebGlRunPlaybackCommandKinds.Cancel, "cancel", WebGlRunPlaybackLifecycleStates.Canceled, reason, cancellationToken);

    public ValueTask<WebGlRunExecutionResult> StopAsync(string reason = "stopped", CancellationToken cancellationToken = default)
        => ApplyLifecycleControlAsync(WebGlRunPlaybackCommandKinds.Stop, "stop", WebGlRunPlaybackLifecycleStates.Stopped, reason, cancellationToken);

    private async ValueTask<WebGlRunExecutionResult> ApplyLifecycleControlAsync(
        string commandKind,
        string operation,
        string lifecycleState,
        string reason,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!EnsureLoaded(out WebGlRunExecutionResult result, operation))
        {
            return result;
        }

        IReadOnlyList<string> affectedStageIds = [.. State.ActiveStageIds
            .Concat(State.PendingStageIds)
            .Distinct(StringComparer.Ordinal)];
        WebGlRunPlaybackResult playback = await controller!.ApplyDetailedAsync(
            new WebGlRunPlaybackCommand { Kind = commandKind, Reason = reason },
            cancellationToken).ConfigureAwait(false);
        MergePlaybackResult(result, playback);
        if (result.Succeeded)
        {
            pendingPlaybackResult = null;
            CompleteLifecycleTransition(result, lifecycleState, reason, affectedStageIds);
        }

        AddSnapshotDiagnostics(result);
        SyncStateDiagnostics(result);
        return result;
    }

    private async ValueTask<WebGlRunExecutionResult> StepAsync(
        string commandKind,
        string operation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!EnsureLoaded(out WebGlRunExecutionResult result, operation))
        {
            return result;
        }

        WebGlRunPlaybackResult playback = await controller!.ApplyDetailedAsync(
            new WebGlRunPlaybackCommand { Kind = commandKind },
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
        apply.Operation = operation;
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
        State.CanceledStageIds.Clear();
        State.PlaybackLifecycleState = WebGlRunPlaybackLifecycleStates.Idle;
        State.LastPlaybackCommandKind = string.Empty;
        State.LastPlaybackStopReason = string.Empty;
        State.PlaybackPauseCount = 0;
        State.PlaybackCancelCount = 0;
        State.PlaybackStopCount = 0;
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

    private WebGlRunExecutionResult CompleteCanceledOperation(
        WebGlRunExecutionResult result,
        string reason,
        IEnumerable<string> canceledStageIds)
    {
        result.Warnings.Add($"Operation '{result.Operation}' was canceled.");
        pendingPlaybackResult = null;
        CompleteLifecycleTransition(result, WebGlRunPlaybackLifecycleStates.Canceled, reason, canceledStageIds);
        SyncStateDiagnostics(result);
        return result;
    }

    private void CompleteLifecycleTransition(
        WebGlRunExecutionResult result,
        string lifecycleState,
        string reason,
        IEnumerable<string> affectedStageIds)
    {
        string normalizedReason = string.IsNullOrWhiteSpace(reason)
            ? lifecycleState
            : reason;
        IReadOnlyList<string> stageIds = [.. affectedStageIds
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)];

        result.PlaybackLifecycleState = lifecycleState;
        result.PlaybackLifecycleReason = normalizedReason;
        result.Paused = string.Equals(lifecycleState, WebGlRunPlaybackLifecycleStates.Paused, StringComparison.Ordinal);
        result.Canceled = string.Equals(lifecycleState, WebGlRunPlaybackLifecycleStates.Canceled, StringComparison.Ordinal);
        result.Stopped = string.Equals(lifecycleState, WebGlRunPlaybackLifecycleStates.Stopped, StringComparison.Ordinal);

        State.PlaybackLifecycleState = lifecycleState;
        State.LastPlaybackCommandKind = result.Operation;
        State.LastPlaybackStopReason = normalizedReason;
        State.ActiveStageIds.Clear();
        State.PendingStageIds.Clear();
        State.CurrentCommandBatchId = string.Empty;

        if (result.Paused)
        {
            State.PlaybackPauseCount++;
        }
        else if (result.Canceled)
        {
            State.PlaybackCancelCount++;
            result.CanceledStageIds.AddRange(stageIds);
            State.CanceledStageIds = [.. State.CanceledStageIds
                .Concat(stageIds)
                .Distinct(StringComparer.Ordinal)];
        }
        else if (result.Stopped)
        {
            State.PlaybackStopCount++;
        }

        result.Diagnostics["playbackLifecycleState"] = State.PlaybackLifecycleState;
        result.Diagnostics["lastPlaybackCommandKind"] = State.LastPlaybackCommandKind;
        result.Diagnostics["lastPlaybackStopReason"] = State.LastPlaybackStopReason;
        result.Diagnostics["playbackPauseCount"] = State.PlaybackPauseCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["playbackCancelCount"] = State.PlaybackCancelCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["playbackStopCount"] = State.PlaybackStopCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["canceledStageIds"] = string.Join(",", State.CanceledStageIds);
    }

    private void UpdateStateFromPlayback(WebGlRunPlaybackResult playback)
    {
        if (playback.CurrentFrame is not null)
        {
            State.CurrentFrameIndex = playback.CurrentFrame.Index;
            State.ActiveStageIds = OrderedStageIds(playback.CurrentFrame).ToList();
        }

        State.PendingStageIds = playback.FramesToApply
            .SelectMany(static frame => WebGlRunStageOrderingPolicy.OrderStages(frame))
            .Select(static stage => stage.StageId)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToList();
        State.CurrentCommandBatchId = playback.State.CurrentCommandBatchId;
        State.LastPlaybackCommandKind = playback.RequestedCommand;
        State.Diagnostics["runId"] = State.RunId;
        State.Diagnostics["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["pendingStageCount"] = State.PendingStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["playbackLifecycleState"] = State.PlaybackLifecycleState;
        State.Diagnostics["lastPlaybackCommandKind"] = State.LastPlaybackCommandKind;
        State.Diagnostics["lastPlaybackStopReason"] = State.LastPlaybackStopReason;
    }

    private void SyncStateDiagnostics(WebGlRunExecutionResult result)
    {
        WebGlRunExecutionResultDiagnostics.SyncDiagnosticCounts(result);
        State.ExecutionDiagnostics = result.ExecutionDiagnostics;
        State.Diagnostics["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["completedStageCount"] = State.CompletedStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["failedStageCount"] = State.FailedStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["pendingStageCount"] = State.PendingStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["canceledStageCount"] = State.CanceledStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["playbackLifecycleState"] = State.PlaybackLifecycleState;
        State.Diagnostics["lastPlaybackCommandKind"] = State.LastPlaybackCommandKind;
        State.Diagnostics["lastPlaybackStopReason"] = State.LastPlaybackStopReason;
        State.Diagnostics["playbackPauseCount"] = State.PlaybackPauseCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["playbackCancelCount"] = State.PlaybackCancelCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["playbackStopCount"] = State.PlaybackStopCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        State.Diagnostics["canceledStageIds"] = string.Join(",", State.CanceledStageIds);
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
        result.Diagnostics["playbackLifecycleState"] = snapshot.PlaybackLifecycleState;
        result.Diagnostics["lastPlaybackCommandKind"] = snapshot.LastPlaybackCommandKind;
        result.Diagnostics["lastPlaybackStopReason"] = snapshot.LastPlaybackStopReason;
        result.Diagnostics["playbackPauseCount"] = snapshot.PlaybackPauseCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["playbackCancelCount"] = snapshot.PlaybackCancelCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        result.Diagnostics["playbackStopCount"] = snapshot.PlaybackStopCount.ToString(System.Globalization.CultureInfo.InvariantCulture);
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

    private static IReadOnlyList<string> OrderedStageIds(WebGlRunFrame frame)
        => [.. WebGlRunStageOrderingPolicy.OrderStages(frame)
            .Select(static stage => stage.StageId)
            .Where(static id => !string.IsNullOrWhiteSpace(id))];

    private WebGlRunExecutionResult CreateResult(string operation)
        => new()
        {
            Operation = operation,
            CurrentFrameIndex = State.CurrentFrameIndex,
            PlaybackLifecycleState = State.PlaybackLifecycleState,
            PlaybackLifecycleReason = State.LastPlaybackStopReason,
            Diagnostics =
            {
                ["runId"] = State.RunId,
                ["currentFrameIndex"] = State.CurrentFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["playbackLifecycleState"] = State.PlaybackLifecycleState,
                ["lastPlaybackCommandKind"] = State.LastPlaybackCommandKind,
                ["lastPlaybackStopReason"] = State.LastPlaybackStopReason
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
        target.PlaybackLifecycleState = playback.PlaybackLifecycleState;
        target.PlaybackLifecycleReason = playback.PlaybackLifecycleReason;
        target.Diagnostics["playbackLifecycleState"] = playback.PlaybackLifecycleState;
        target.Diagnostics["lastPlaybackStopReason"] = playback.PlaybackLifecycleReason;
    }

}
