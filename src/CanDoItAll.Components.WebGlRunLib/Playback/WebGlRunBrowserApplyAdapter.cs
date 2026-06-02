using System.Text.Json;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunBrowserApplyAdapter
{
    ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunFrameApplyResult frameApplyResult,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunPlaybackResult playbackResult,
        CancellationToken cancellationToken = default);
}

public interface IWebGlRunBrowserRuntime
{
    ValueTask<WebGlSceneCommandResult?> ImportSceneAsync(
        WebGlSceneDocument sceneDocument,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(
        WebGlSceneCommandBatch batch,
        CancellationToken cancellationToken = default);

    ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default);
}

public sealed class WebGlRunBrowserApplyAdapter(
    IWebGlRunBrowserRuntime runtime,
    WebGlSceneDocument? initialScene = null) : IWebGlRunBrowserApplyAdapter
{
    private const int MaxSnapshotListItems = 100;
    private const int MaxSnapshotJournalEntries = 12;
    private const string InitialSceneRuntimeOptionsExternalWarning =
        "Initial scene runtime options are external to WebGlRun browser reset and were not applied.";

    public async ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunFrameApplyResult frameApplyResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frameApplyResult);
        cancellationToken.ThrowIfCancellationRequested();

        var result = CreateResult(frameApplyResult);
        result.Warnings.AddRange(frameApplyResult.Warnings);
        result.Errors.AddRange(frameApplyResult.Errors);

        WebGlSceneDocument? sceneToReset = frameApplyResult.InitialScene ?? initialScene;
        if (frameApplyResult.RequiresSceneReset)
        {
            if (sceneToReset is null)
            {
                result.Errors.Add("Frame requires a scene reset, but no initial scene was supplied.");
            }
            else
            {
                if (HasNonDefaultRuntimeOptions(sceneToReset.RuntimeOptions))
                {
                    result.Warnings.Add(InitialSceneRuntimeOptionsExternalWarning);
                }

                WebGlSceneDocument resetDocument = CreateSceneResetDocument(sceneToReset);
                WebGlSceneCommandResult? importResult = await runtime.ImportSceneAsync(resetDocument, cancellationToken).ConfigureAwait(false);
                result.AppliedInitialScene = importResult?.Success == true;
                AddCommandOutcome(result, importResult);
            }
        }

        WebGlSceneCommandBatchResult? batchResult = await runtime.ApplyCommandBatchAsync(frameApplyResult.CommandBatch, cancellationToken).ConfigureAwait(false);
        result.CommandBatchResult = batchResult;
        AddCommandOutcome(result, batchResult);

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

    private static WebGlSceneDocument CreateSceneResetDocument(WebGlSceneDocument source)
        => new()
        {
            SchemaVersion = source.SchemaVersion,
            DocumentId = source.DocumentId,
            Scene = source.Scene,
            RuntimeOptions = new WebGlRuntimeOptions(),
            Diagnostics = new WebGlRuntimeDiagnostics(),
            SavedAtUtc = source.SavedAtUtc,
            Source = source.Source,
            SceneContentHash = source.SceneContentHash,
            DocumentHash = source.DocumentHash,
            ContentHash = source.ContentHash,
            Metadata = source.Metadata is null
                ? []
                : new Dictionary<string, string>(source.Metadata, StringComparer.Ordinal)
        };

    private static bool HasNonDefaultRuntimeOptions(WebGlRuntimeOptions? runtimeOptions)
        => runtimeOptions is not null &&
           !string.Equals(
               JsonSerializer.Serialize(runtimeOptions),
               JsonSerializer.Serialize(new WebGlRuntimeOptions()),
               StringComparison.Ordinal);

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

public sealed class WebGlRunBrowserApplyResult
{
    public bool Success => Errors.Count == 0;

    public long FrameIndex { get; set; }

    public bool AppliedInitialScene { get; set; }

    public int AppliedStageCount { get; set; }

    public int AppliedPatchCount { get; set; }

    public int AppliedMotionCount { get; set; }

    public WebGlRunRuntimeSnapshot RuntimeSnapshot { get; set; } = new();

    public WebGlRuntimeDiagnostics? RuntimeDiagnostics { get; set; }

    public WebGlSceneCommandBatchResult? CommandBatchResult { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}

public sealed class WebGlSceneViewBrowserRuntime(WebGlSceneView sceneView) : IWebGlRunBrowserRuntime
{
    public async ValueTask<WebGlSceneCommandResult?> ImportSceneAsync(
        WebGlSceneDocument sceneDocument,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sceneDocument);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.ImportSceneDetailedAsync(sceneDocument.Scene).ConfigureAwait(false);
    }

    public async ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(
        WebGlSceneCommandBatch batch,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(batch);
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.ApplyCommandBatchAsync(batch).ConfigureAwait(false);
    }

    public async ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await sceneView.GetDiagnosticsAsync().ConfigureAwait(false);
    }
}
