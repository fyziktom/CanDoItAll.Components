using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunBrowserApplyAdapter(
    IWebGlRunBrowserRuntime runtime,
    WebGlSceneDocument? initialScene = null) : IWebGlRunBrowserApplyAdapter
{
    private const int MaxSnapshotListItems = 100;
    private const int MaxSnapshotJournalEntries = 12;
    public async ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunFrameApplyResult frameApplyResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frameApplyResult);
        cancellationToken.ThrowIfCancellationRequested();

        var result = CreateResult(frameApplyResult);
        result.Warnings.AddRange(frameApplyResult.Warnings);
        result.Errors.AddRange(frameApplyResult.Errors);

        if (result.Errors.Count > 0)
        {
            result.FailureReason = WebGlRunBrowserApplyFailureReasons.PreApplyValidationFailed;
            WebGlRuntimeDiagnostics? failureDiagnostics = await runtime.GetDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
            result.RuntimeSnapshot = BuildSnapshot(frameApplyResult, failureDiagnostics, result);
            result.RuntimeDiagnostics = failureDiagnostics;
            return result;
        }

        WebGlSceneDocument? sceneToReset = frameApplyResult.InitialScene ?? initialScene;
        if (frameApplyResult.RequiresSceneReset)
        {
            if (sceneToReset is null)
            {
                result.Errors.Add("Frame requires a scene reset, but no initial scene was supplied.");
            }
            else
            {
                WebGlSceneCommandResult? importResult = await runtime.ImportSceneAsync(sceneToReset, cancellationToken).ConfigureAwait(false);
                result.AppliedInitialScene = importResult?.Success == true;
                AddCommandOutcome(result, importResult);
            }

            if (result.Errors.Count > 0 || !result.AppliedInitialScene)
            {
                result.FailureReason = WebGlRunBrowserApplyFailureReasons.ResetFailed;
                if (result.Errors.Count == 0)
                {
                    result.Errors.Add("Frame requires a scene reset, but browser scene import did not report success.");
                }

                WebGlRuntimeDiagnostics? resetFailureDiagnostics = await runtime.GetDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
                result.RuntimeSnapshot = BuildSnapshot(frameApplyResult, resetFailureDiagnostics, result);
                result.RuntimeDiagnostics = resetFailureDiagnostics;
                return result;
            }
        }

        WebGlSceneCommandBatchResult? batchResult = await runtime.ApplyCommandBatchAsync(frameApplyResult.CommandBatch, cancellationToken).ConfigureAwait(false);
        result.CommandBatchResult = batchResult;
        AddCommandOutcome(result, batchResult);
        if (result.Errors.Count > 0)
        {
            result.FailureReason = WebGlRunBrowserApplyFailureReasons.BatchFailed;
        }

        WebGlRuntimeDiagnostics? diagnostics = await runtime.GetDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
        result.RuntimeSnapshot = BuildSnapshot(frameApplyResult, diagnostics, result);
        result.RuntimeDiagnostics = diagnostics;
        return result;
    }

    public async ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunPlaybackResult playbackResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(playbackResult);
        cancellationToken.ThrowIfCancellationRequested();

        if (playbackResult.FramesToApply.Count > 1)
        {
            return new WebGlRunBrowserApplyResult
            {
                FrameIndex = playbackResult.TargetFrameIndex,
                FailureReason = WebGlRunBrowserApplyFailureReasons.MultiFramePlaybackRequiresExplicitApply,
                Errors =
                {
                    "Playback result contains multiple frames to apply. Use ApplyPlaybackAsync to apply reset and frames explicitly."
                }
            };
        }

        WebGlRunFrameApplyResult? frame = playbackResult.CurrentFrame is not null
            ? WebGlRunFrameApplyResult.FromFrame(playbackResult.CurrentFrame)
            : playbackResult.FramesToApply.Count > 0
                ? WebGlRunFrameApplyResult.FromFrame(playbackResult.FramesToApply[^1])
                : null;

        if (frame is null)
        {
            return new WebGlRunBrowserApplyResult
            {
                FrameIndex = playbackResult.TargetFrameIndex,
                Errors = ["Playback result does not contain a frame to apply."]
            };
        }

        frame.RequiresSceneReset = playbackResult.RequiresSceneReset;
        frame.Errors.AddRange(playbackResult.Errors);
        frame.Warnings.AddRange(playbackResult.Warnings);
        return await ApplyAsync(frame, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<WebGlRunBrowserPlaybackApplyResult> ApplyPlaybackAsync(
        WebGlRunPlaybackResult playbackResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(playbackResult);
        cancellationToken.ThrowIfCancellationRequested();

        var result = new WebGlRunBrowserPlaybackApplyResult
        {
            TargetFrameIndex = playbackResult.TargetFrameIndex,
            RequestedCommand = playbackResult.RequestedCommand,
            TransactionPolicy = WebGlRunBrowserPlaybackTransactionPolicies.StopOnFirstFailure,
            RequiresSceneReset = playbackResult.RequiresSceneReset
        };
        result.Errors.AddRange(playbackResult.Errors);
        result.Warnings.AddRange(playbackResult.Warnings);
        if (result.Errors.Count > 0)
        {
            result.FailureReason = WebGlRunBrowserApplyFailureReasons.PreApplyValidationFailed;
            result.FailureSnapshot = BuildPlaybackFailureSnapshot(playbackResult, null, result);
            return result;
        }

        if (playbackResult.FramesToApply.Count == 0)
        {
            result.FailureReason = WebGlRunBrowserApplyFailureReasons.PreApplyValidationFailed;
            result.Errors.Add("Playback result does not contain frames to apply.");
            result.FailureSnapshot = BuildPlaybackFailureSnapshot(playbackResult, null, result);
            return result;
        }

        if (playbackResult.RequiresSceneReset)
        {
            WebGlSceneDocument? sceneToReset = initialScene;
            if (sceneToReset is null)
            {
                result.FailureReason = WebGlRunBrowserApplyFailureReasons.ResetFailed;
                result.Errors.Add("Playback result requires a scene reset, but no initial scene was supplied.");
                result.FailureSnapshot = BuildPlaybackFailureSnapshot(playbackResult, null, result);
                return result;
            }

            WebGlSceneCommandResult? importResult;
            try
            {
                importResult = await runtime.ImportSceneAsync(sceneToReset, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return await CompletePlaybackCancellationAsync(
                    result,
                    playbackResult,
                    "scene reset canceled",
                    null).ConfigureAwait(false);
            }

            result.AppliedInitialScene = importResult?.Success == true;
            AddPlaybackCommandOutcome(result, importResult);
            if (result.Errors.Count > 0 || !result.AppliedInitialScene)
            {
                result.FailureReason = WebGlRunBrowserApplyFailureReasons.ResetFailed;
                if (result.Errors.Count == 0)
                {
                    result.Errors.Add("Playback result requires a scene reset, but browser scene import did not report success.");
                }

                WebGlRuntimeDiagnostics? resetFailureDiagnostics = await TryGetDiagnosticsAfterFailureAsync().ConfigureAwait(false);
                result.FailureSnapshot = BuildPlaybackFailureSnapshot(playbackResult, resetFailureDiagnostics, result);
                return result;
            }
        }

        foreach (WebGlRunFrame frame in playbackResult.FramesToApply)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return await CompletePlaybackCancellationAsync(
                    result,
                    playbackResult,
                    "cancellation requested before frame apply",
                    frame.Index).ConfigureAwait(false);
            }

            WebGlRunFrameApplyResult frameApplyResult = WebGlRunFrameApplyResult.FromFrame(frame);
            WebGlRunBrowserApplyResult frameResult;
            try
            {
                frameResult = await ApplyAsync(frameApplyResult, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return await CompletePlaybackCancellationAsync(
                    result,
                    playbackResult,
                    "frame apply canceled",
                    frame.Index).ConfigureAwait(false);
            }

            result.FrameResults.Add(frameResult);
            result.Warnings.AddRange(frameResult.Warnings);
            if (!frameResult.Success)
            {
                result.FailedFrameIndex = frameResult.FrameIndex;
                result.FailureReason = string.IsNullOrWhiteSpace(frameResult.FailureReason)
                    ? WebGlRunBrowserApplyFailureReasons.BatchFailed
                    : frameResult.FailureReason;
                result.Errors.AddRange(frameResult.Errors);
                result.FailureSnapshot = frameResult.RuntimeSnapshot;
                AnnotatePlaybackFailureSnapshot(result.FailureSnapshot, playbackResult, result);
                return result;
            }

            result.LastAppliedFrameIndex = frameResult.FrameIndex;
            if (cancellationToken.IsCancellationRequested)
            {
                return await CompletePlaybackCancellationAsync(
                    result,
                    playbackResult,
                    "frame apply canceled",
                    frameResult.FrameIndex,
                    frameResult.RuntimeSnapshot).ConfigureAwait(false);
            }
        }

        return result;
    }

    private async ValueTask<WebGlRunBrowserPlaybackApplyResult> CompletePlaybackCancellationAsync(
        WebGlRunBrowserPlaybackApplyResult result,
        WebGlRunPlaybackResult playbackResult,
        string cancellationReason,
        long? canceledFrameIndex,
        WebGlRunRuntimeSnapshot? failureSnapshot = null)
    {
        result.Canceled = true;
        result.FailedFrameIndex = canceledFrameIndex;
        result.FailureReason = WebGlRunBrowserApplyFailureReasons.CancellationRequested;
        result.CancellationReason = cancellationReason;
        result.Errors.Add($"Playback apply was canceled: {cancellationReason}.");
        result.FailureSnapshot = failureSnapshot ?? BuildPlaybackFailureSnapshot(
            playbackResult,
            await TryGetDiagnosticsAfterFailureAsync().ConfigureAwait(false),
            result);
        AnnotatePlaybackFailureSnapshot(result.FailureSnapshot, playbackResult, result);
        return result;
    }

    private async ValueTask<WebGlRuntimeDiagnostics?> TryGetDiagnosticsAfterFailureAsync()
    {
        try
        {
            return await runtime.GetDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    private static WebGlRunBrowserApplyResult CreateResult(WebGlRunFrameApplyResult frame)
    {
        WebGlSceneCommandBatch batch = frame.CommandBatch;
        return new()
        {
            FrameIndex = frame.FrameIndex,
            AppliedStageCount = batch.Stages.Count,
            AppliedPatchCount = batch.Patches.Count + batch.Stages.Sum(static stage => stage.Patches.Count),
            AppliedMotionCount = batch.Motions.Count + batch.Stages.Sum(static stage => stage.Motions.Count)
        };
    }

    private static WebGlRunRuntimeSnapshot BuildPlaybackFailureSnapshot(
        WebGlRunPlaybackResult playbackResult,
        WebGlRuntimeDiagnostics? diagnostics,
        WebGlRunBrowserPlaybackApplyResult result)
    {
        var snapshot = new WebGlRunRuntimeSnapshot
        {
            CurrentFrameIndex = result.FailedFrameIndex ?? result.LastAppliedFrameIndex ?? playbackResult.TargetFrameIndex,
            CurrentCommandBatchId = diagnostics?.CurrentCommandBatchId ?? string.Empty,
            CurrentStageId = diagnostics?.CurrentCommandStageId ?? string.Empty,
            ActiveStageIds = TakeFirst(diagnostics?.CompletedCommandStageIds),
            QueuedStageIds = TakeFirst(diagnostics?.CommandStageQueueSnapshot.Select(static item => item.StageId)),
            QueuedStageCount = diagnostics?.QueuedCommandStageCount ?? 0,
            ActiveMotionCount = diagnostics?.ActiveMotionCount ?? 0,
            ActiveMotionIds = TakeFirst(diagnostics?.ActiveMotionIds),
            QueuedMotionCount = diagnostics?.QueuedMotionCount ?? 0,
            QueuedMotionIds = TakeFirst(diagnostics?.QueuedMotionIds),
            MotionQueueSnapshot = TakeFirst(diagnostics?.MotionQueueSnapshot),
            CommandJournalTail = TakeTail(diagnostics?.CommandStageRecentJournalEntries, MaxSnapshotJournalEntries),
            CommandJournalDroppedCount = diagnostics?.CommandStageJournalDroppedCount ?? 0,
            RuntimeErrors = TakeFirst(result.Errors),
            RuntimeWarnings = TakeFirst(result.Warnings),
            Diagnostics = BuildDiagnostics(diagnostics)
        };

        if (diagnostics is not null)
        {
            snapshot.StageBarrier = new()
            {
                Policy = diagnostics.CommandStageBarrierPolicy,
                Target = diagnostics.CommandStageBarrierTarget,
                Blockers = TakeFirst(diagnostics.CommandStageBarrierBlockers),
                EventId = diagnostics.CommandStageBarrierEventId,
                ObjectIds = TakeFirst(diagnostics.CommandStageBarrierObjectIds),
                WaitSeconds = diagnostics.CommandStageWaitSeconds,
                ElapsedSeconds = diagnostics.CommandStageBarrierElapsedSeconds,
                TimeoutSeconds = diagnostics.CommandStageBarrierTimeoutSeconds,
                TimedOut = diagnostics.CommandStageBarrierTimedOut
            };
            AddIfNotEmpty(snapshot.RuntimeWarnings, diagnostics.LastStageBarrierWarning);
            AddIfNotEmpty(snapshot.RuntimeErrors, diagnostics.LastError);
            AddIfNotEmpty(snapshot.RuntimeErrors, diagnostics.LastStageError);
        }

        AnnotatePlaybackFailureSnapshot(snapshot, playbackResult, result);
        return snapshot;
    }

    private static void AnnotatePlaybackFailureSnapshot(
        WebGlRunRuntimeSnapshot snapshot,
        WebGlRunPlaybackResult playbackResult,
        WebGlRunBrowserPlaybackApplyResult result)
    {
        snapshot.Diagnostics["targetFrameIndex"] = playbackResult.TargetFrameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        snapshot.Diagnostics["lastAppliedFrameIndex"] = result.LastAppliedFrameIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "";
        snapshot.Diagnostics["failedFrameIndex"] = result.FailedFrameIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "";
        snapshot.Diagnostics["transactionPolicy"] = result.TransactionPolicy;
        snapshot.Diagnostics["failureReason"] = result.FailureReason;
        snapshot.Diagnostics["cancellationReason"] = result.CancellationReason;
    }

    private static void AddCommandOutcome(WebGlRunBrowserApplyResult result, WebGlSceneCommandResult? commandResult)
    {
        if (commandResult is null)
        {
            return;
        }

        result.Warnings.AddRange(commandResult.Warnings);
        result.Errors.AddRange(commandResult.Errors);
        if (!commandResult.Success && commandResult.Errors.Count == 0)
        {
            result.Errors.Add(string.IsNullOrWhiteSpace(commandResult.Message)
                ? $"Runtime command '{commandResult.CommandId}' failed."
                : commandResult.Message);
        }
    }

    private static void AddPlaybackCommandOutcome(WebGlRunBrowserPlaybackApplyResult result, WebGlSceneCommandResult? commandResult)
    {
        if (commandResult is null)
        {
            return;
        }

        result.Warnings.AddRange(commandResult.Warnings);
        result.Errors.AddRange(commandResult.Errors);
        if (!commandResult.Success && commandResult.Errors.Count == 0)
        {
            result.Errors.Add(string.IsNullOrWhiteSpace(commandResult.Message)
                ? $"Runtime command '{commandResult.CommandId}' failed."
                : commandResult.Message);
        }
    }

    private static WebGlRunRuntimeSnapshot BuildSnapshot(
        WebGlRunFrameApplyResult frame,
        WebGlRuntimeDiagnostics? diagnostics,
        WebGlRunBrowserApplyResult result)
    {
        WebGlSceneCommandBatch batch = frame.CommandBatch;
        var snapshot = new WebGlRunRuntimeSnapshot
        {
            CurrentFrameIndex = frame.FrameIndex,
            CurrentCommandBatchId = FirstNonEmpty(diagnostics?.CurrentCommandBatchId, batch.BatchId),
            CurrentStageId = FirstNonEmpty(diagnostics?.CurrentCommandStageId, batch.Stages.LastOrDefault()?.StageId),
            CurrentStageIds = [.. batch.Stages.Select(static stage => stage.StageId).Where(static value => !string.IsNullOrWhiteSpace(value))],
            ActiveStageIds = TakeFirst(diagnostics?.CompletedCommandStageIds),
            QueuedStageIds = TakeFirst(diagnostics?.CommandStageQueueSnapshot.Select(static item => item.StageId)),
            QueuedStageCount = diagnostics?.QueuedCommandStageCount ?? 0,
            ActiveMotionCount = diagnostics?.ActiveMotionCount ?? 0,
            ActiveMotionIds = TakeFirst(diagnostics?.ActiveMotionIds),
            QueuedMotionCount = diagnostics?.QueuedMotionCount ?? 0,
            QueuedMotionIds = TakeFirst(diagnostics?.QueuedMotionIds),
            MotionQueueSnapshot = TakeFirst(diagnostics?.MotionQueueSnapshot),
            CommandJournalTail = TakeTail(diagnostics?.CommandStageRecentJournalEntries, MaxSnapshotJournalEntries),
            CommandJournalDroppedCount = diagnostics?.CommandStageJournalDroppedCount ?? 0,
            RuntimeErrors = TakeFirst(result.Errors),
            RuntimeWarnings = TakeFirst(result.Warnings),
            Diagnostics = BuildDiagnostics(diagnostics)
        };

        if (diagnostics is not null)
        {
            snapshot.StageBarrier = new()
            {
                Policy = diagnostics.CommandStageBarrierPolicy,
                Target = diagnostics.CommandStageBarrierTarget,
                Blockers = TakeFirst(diagnostics.CommandStageBarrierBlockers),
                EventId = diagnostics.CommandStageBarrierEventId,
                ObjectIds = TakeFirst(diagnostics.CommandStageBarrierObjectIds),
                WaitSeconds = diagnostics.CommandStageWaitSeconds,
                ElapsedSeconds = diagnostics.CommandStageBarrierElapsedSeconds,
                TimeoutSeconds = diagnostics.CommandStageBarrierTimeoutSeconds,
                TimedOut = diagnostics.CommandStageBarrierTimedOut
            };
            AddIfNotEmpty(snapshot.RuntimeWarnings, diagnostics.LastStageBarrierWarning);
            AddIfNotEmpty(snapshot.RuntimeErrors, diagnostics.LastError);
            AddIfNotEmpty(snapshot.RuntimeErrors, diagnostics.LastStageError);
        }

        return snapshot;
    }

    private static Dictionary<string, string> BuildDiagnostics(WebGlRuntimeDiagnostics? diagnostics)
    {
        if (diagnostics is null)
        {
            return [];
        }

        return new(StringComparer.Ordinal)
        {
            ["renderCount"] = diagnostics.RenderCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["batchCommandCount"] = diagnostics.BatchCommandCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["batchStageCount"] = diagnostics.BatchStageCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["batchDurationMs"] = diagnostics.BatchDurationMs.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["commandCountBeforeNormalization"] = diagnostics.CommandCountBeforeNormalization.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["commandCountAfterNormalization"] = diagnostics.CommandCountAfterNormalization.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["coalescedPatchCount"] = diagnostics.CoalescedPatchCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["droppedDuplicateMotionCount"] = diagnostics.DroppedDuplicateMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["preservedOrderedDuplicateMotionCount"] = diagnostics.PreservedOrderedDuplicateMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["interopCallsAvoided"] = diagnostics.InteropCallsAvoided.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["activeMotionCount"] = diagnostics.ActiveMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["queuedMotionCount"] = diagnostics.QueuedMotionCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["queuedCommandStageCount"] = diagnostics.QueuedCommandStageCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["commandStageJournalCount"] = diagnostics.CommandStageJournalCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["commandStageBarrierPolicy"] = diagnostics.CommandStageBarrierPolicy,
            ["lastError"] = diagnostics.LastError,
            ["lastStageError"] = diagnostics.LastStageError
        };
    }

    private static void AddIfNotEmpty(List<string> items, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            items.Count < MaxSnapshotListItems &&
            !items.Contains(value, StringComparer.Ordinal))
        {
            items.Add(value);
        }
    }

    private static List<T> TakeFirst<T>(IEnumerable<T>? items, int maxCount = MaxSnapshotListItems)
        => items is null ? [] : [.. items.Take(maxCount)];

    private static List<T> TakeTail<T>(IEnumerable<T>? items, int maxCount)
    {
        if (items is null)
        {
            return [];
        }

        T[] snapshot = [.. items];
        return snapshot.Length <= maxCount
            ? [.. snapshot]
            : [.. snapshot.Skip(snapshot.Length - maxCount)];
    }

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
}
