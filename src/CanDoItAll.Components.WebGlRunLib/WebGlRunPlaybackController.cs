using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackController : IWebGlRunPlaybackController
{
    private readonly IWebGlRunFrameSource frameSource;
    private readonly WebGlSceneDocument? initialScene;
    private readonly WebGlRunTimeline? timeline;
    private readonly WebGlRunTimelineValidator timelineValidator;
    private readonly WebGlRunFrameResolver frameResolver;
    private readonly long? maxFrameIndex;
    private readonly Dictionary<string, string> runSourceProvenance;

    public WebGlRunPlaybackController(WebGlRunDocument document)
        : this(
            new WebGlRunDocumentFrameSource(document),
            new WebGlRunTimelineValidator(),
            new WebGlRunFrameResolver(),
            document.InitialScene,
            document.Timeline,
            document.RunId,
            ExtractRunSourceProvenance(document.Metadata))
    {
    }

    public WebGlRunPlaybackController(
        IWebGlRunFrameSource frameSource,
        WebGlRunTimelineValidator timelineValidator,
        WebGlRunFrameResolver? frameResolver = null,
        WebGlSceneDocument? initialScene = null,
        WebGlRunTimeline? timeline = null,
        WebGlRunId? runId = null,
        IReadOnlyDictionary<string, string>? runSourceProvenance = null)
    {
        this.frameSource = frameSource;
        this.initialScene = initialScene;
        this.timelineValidator = timelineValidator;
        this.frameResolver = frameResolver ?? new WebGlRunFrameResolver();
        this.timeline = timeline;
        this.runSourceProvenance = new Dictionary<string, string>(runSourceProvenance ?? new Dictionary<string, string>(), StringComparer.Ordinal);
        maxFrameIndex = timeline?.Frames.Count > 0 ? timeline.Frames.Max(static item => item.Index) : null;
        State.RunId = runId ?? new WebGlRunId(string.Empty);
        State.InitialSceneId = initialScene?.Scene.SceneId ?? string.Empty;
    }

    public WebGlRunPlaybackState State { get; } = new();

    public async ValueTask<WebGlRunFrame?> ApplyAsync(WebGlRunPlaybackCommand command, CancellationToken cancellationToken = default)
        => (await ApplyDetailedAsync(command, cancellationToken).ConfigureAwait(false)).CurrentFrame;

    public async ValueTask<WebGlRunPlaybackResult> ApplyDetailedAsync(
        WebGlRunPlaybackCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(command);

        var result = new WebGlRunPlaybackResult
        {
            RequestedCommand = command.Kind,
            State = State,
            RunSourceProvenance = new Dictionary<string, string>(runSourceProvenance, StringComparer.Ordinal)
        };
        if (timeline is not null)
        {
            var validation = timelineValidator.Validate(timeline);
            if (!validation.Success)
            {
                result.Errors.AddRange(validation.Errors);
                return result;
            }
        }

        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Pause, StringComparison.OrdinalIgnoreCase))
        {
            State.IsPlaying = false;
            result.CurrentFrame = timeline?.Frames.FirstOrDefault(frame => frame.Index == State.CurrentFrameIndex);
            return result;
        }

        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Reset, StringComparison.OrdinalIgnoreCase))
        {
            State.IsPlaying = false;
            State.CurrentFrameIndex = 0;
            State.InitialSceneLoaded = true;
            result.RequiresSceneReset = true;
        }

        var targetFrameIndex = ResolveTargetFrameIndex(command);
        result.TargetFrameIndex = targetFrameIndex;
        State.IsPlaying = string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Play, StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Resume, StringComparison.OrdinalIgnoreCase) ||
                          State.IsPlaying;

        var frame = await frameSource.GetFrameAsync(State.RunId, targetFrameIndex, cancellationToken).ConfigureAwait(false);
        if (frame is null)
        {
            result.Errors.Add($"Frame '{targetFrameIndex}' was not found for run '{State.RunId.Value}'.");
            return result;
        }

        if (timeline is not null && targetFrameIndex < State.CurrentFrameIndex)
        {
            result.RequiresSceneReset = true;
            result.FramesToApply.AddRange(frameResolver.ResolveReplayFrames(timeline, targetFrameIndex));
        }
        else
        {
            result.FramesToApply.Add(frame);
        }

        State.CurrentFrameIndex = frame.Index;
        result.CurrentFrame = frame;
        result.FramesApplied = result.FramesToApply.Count;
        result.StagesQueued = result.FramesToApply.Sum(static item => item.Stages.Count);
        UpdateRuntimeState(frame, result.StagesQueued);
        return result;
    }

    public WebGlRunRuntimeSnapshot ExportRuntimeSnapshot()
        => new()
        {
            RunId = State.RunId.Value,
            InitialSceneId = State.InitialSceneId,
            InitialObjectCount = initialScene?.Scene.Objects.Count ?? 0,
            InitialLinkCount = initialScene?.Scene.Links.Count ?? 0,
            CurrentFrameIndex = State.CurrentFrameIndex,
            CurrentCommandBatchId = State.CurrentCommandBatchId,
            CurrentStageId = State.CurrentStageId,
            CurrentStageIds = [.. State.CurrentStageIds],
            CurrentActionIds = [.. State.CurrentActionIds],
            QueuedStageCount = State.QueuedStageCount,
            IsPlaying = State.IsPlaying,
            PlaybackSpeed = State.PlaybackSpeed,
            InitialSceneLoaded = State.InitialSceneLoaded,
            RunSourceProvenance = new Dictionary<string, string>(runSourceProvenance, StringComparer.Ordinal),
            Diagnostics =
            {
                ["maxFrameIndex"] = maxFrameIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "",
                ["hasTimeline"] = (timeline is not null).ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["frameCount"] = (timeline?.Frames.Count ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        };

    public async ValueTask<WebGlRunPlaybackResult> PlayToEndAsync(
        IWebGlRunFrameApplier frameApplier,
        WebGlRunPlaybackOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frameApplier);
        options ??= new WebGlRunPlaybackOptions();

        WebGlRunPlaybackResult result = await ApplyDetailedAsync(
            new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Play },
            cancellationToken).ConfigureAwait(false);
        if (!result.Success)
        {
            return result;
        }

        await ApplyFramesAsync(frameApplier, result, cancellationToken).ConfigureAwait(false);

        while (State.IsPlaying)
        {
            if (options.StopAtTimelineEnd && maxFrameIndex is { } endFrameIndex && State.CurrentFrameIndex >= endFrameIndex)
            {
                await ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Pause }, cancellationToken).ConfigureAwait(false);
                break;
            }

            TimeSpan delay = timeline is null
                ? TimeSpan.Zero
                : new WebGlRunPlaybackClock().ResolveFrameDelay(timeline, State.PlaybackSpeed);
            if (delay > TimeSpan.Zero)
            {
                if (options.DelayAsync is null)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await options.DelayAsync(delay, cancellationToken).ConfigureAwait(false);
                }
            }

            result = await ApplyDetailedAsync(
                new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Next },
                cancellationToken).ConfigureAwait(false);
            if (!result.Success)
            {
                return result;
            }

            await ApplyFramesAsync(frameApplier, result, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    private long ResolveTargetFrameIndex(WebGlRunPlaybackCommand command)
    {
        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Reset, StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Seek, StringComparison.OrdinalIgnoreCase))
        {
            return command.TargetFrameIndex ?? State.CurrentFrameIndex;
        }

        if (timeline is null)
        {
            return command.TargetFrameIndex ?? State.CurrentFrameIndex + 1;
        }

        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Previous, StringComparison.OrdinalIgnoreCase))
        {
            return frameResolver.ResolvePreviousFrame(timeline, State.CurrentFrameIndex)?.Index ?? State.CurrentFrameIndex;
        }

        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Play, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Resume, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Step, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Next, StringComparison.OrdinalIgnoreCase))
        {
            return frameResolver.ResolveNextFrame(timeline, State.CurrentFrameIndex)?.Index ?? State.CurrentFrameIndex;
        }

        return command.TargetFrameIndex ?? State.CurrentFrameIndex;
    }

    private static async ValueTask ApplyFramesAsync(
        IWebGlRunFrameApplier frameApplier,
        WebGlRunPlaybackResult result,
        CancellationToken cancellationToken)
    {
        foreach (WebGlRunFrame frame in result.FramesToApply)
        {
            await frameApplier.ApplyAsync(WebGlRunFrameApplyResult.FromFrame(frame), cancellationToken).ConfigureAwait(false);
        }
    }

    private static Dictionary<string, string> ExtractRunSourceProvenance(IReadOnlyDictionary<string, string> metadata)
    {
        string[] keys = ["inputPackHash", "runPlanHash", "visualMappingHash"];
        return keys
            .Where(metadata.ContainsKey)
            .ToDictionary(static key => key, key => metadata[key], StringComparer.Ordinal);
    }

    private void UpdateRuntimeState(WebGlRunFrame frame, int queuedStageCount)
    {
        State.CurrentCommandBatchId = $"run-frame:{frame.Index.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        State.CurrentStageIds = [.. WebGlRunStageOrderingPolicy.OrderStages(frame).Select(static stage => stage.StageId).Where(static id => !string.IsNullOrWhiteSpace(id))];
        State.CurrentStageId = State.CurrentStageIds.FirstOrDefault() ?? string.Empty;
        State.CurrentActionIds = [.. ResolveCurrentActionIds(frame).Distinct(StringComparer.Ordinal)];
        State.QueuedStageCount = queuedStageCount;
    }

    private static IEnumerable<string> ResolveCurrentActionIds(WebGlRunFrame frame)
    {
        foreach (WebGlRunActionStage stage in WebGlRunStageOrderingPolicy.OrderStages(frame))
        {
            foreach (string? value in new[] { stage.SequenceId, stage.ParentActionId, stage.Metadata.GetValueOrDefault("actionId") })
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value;
                }
            }

            foreach (WebGlObjectMotionCommand motion in stage.Motions)
            {
                if (motion.Metadata.TryGetValue("actionId", out string? motionActionId) &&
                    !string.IsNullOrWhiteSpace(motionActionId))
                {
                    yield return motionActionId;
                }
            }
        }
    }
}
