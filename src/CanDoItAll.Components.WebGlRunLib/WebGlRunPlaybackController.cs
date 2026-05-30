namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackController : IWebGlRunPlaybackController
{
    private readonly IWebGlRunFrameSource frameSource;
    private readonly WebGlRunTimeline? timeline;
    private readonly WebGlRunTimelineValidator timelineValidator;
    private readonly WebGlRunFrameResolver frameResolver;

    public WebGlRunPlaybackController(WebGlRunDocument document)
        : this(
            new WebGlRunDocumentFrameSource(document),
            new WebGlRunTimelineValidator(),
            new WebGlRunFrameResolver(),
            document.Timeline,
            document.RunId)
    {
    }

    public WebGlRunPlaybackController(
        IWebGlRunFrameSource frameSource,
        WebGlRunTimelineValidator timelineValidator,
        WebGlRunFrameResolver? frameResolver = null,
        WebGlRunTimeline? timeline = null,
        WebGlRunId? runId = null)
    {
        this.frameSource = frameSource;
        this.timelineValidator = timelineValidator;
        this.frameResolver = frameResolver ?? new WebGlRunFrameResolver();
        this.timeline = timeline;
        State.RunId = runId ?? new WebGlRunId(string.Empty);
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

        var result = new WebGlRunPlaybackResult { State = State };
        if (timeline is not null)
        {
            var validation = timelineValidator.Validate(timeline);
            if (!validation.Success)
            {
                result.Errors.AddRange(validation.Errors);
                return result;
            }
        }

        var targetFrameIndex = ResolveTargetFrameIndex(command);
        State.IsPlaying = string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Play, StringComparison.OrdinalIgnoreCase) ||
                          (State.IsPlaying && !string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Pause, StringComparison.OrdinalIgnoreCase));
        if (string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Pause, StringComparison.OrdinalIgnoreCase))
        {
            State.IsPlaying = false;
        }

        var frame = await frameSource.GetFrameAsync(State.RunId, targetFrameIndex, cancellationToken).ConfigureAwait(false);
        if (frame is null)
        {
            result.Errors.Add($"Frame '{targetFrameIndex}' was not found for run '{State.RunId.Value}'.");
            return result;
        }

        if (timeline is not null && targetFrameIndex < State.CurrentFrameIndex)
        {
            result.FramesToApply.AddRange(frameResolver.ResolveReplayFrames(timeline, targetFrameIndex));
        }
        else
        {
            result.FramesToApply.Add(frame);
        }

        State.CurrentFrameIndex = frame.Index;
        result.CurrentFrame = frame;
        return result;
    }

    private long ResolveTargetFrameIndex(WebGlRunPlaybackCommand command)
    {
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
            string.Equals(command.Kind, WebGlRunPlaybackCommandKinds.Next, StringComparison.OrdinalIgnoreCase))
        {
            return frameResolver.ResolveNextFrame(timeline, State.CurrentFrameIndex)?.Index ?? State.CurrentFrameIndex;
        }

        return command.TargetFrameIndex ?? State.CurrentFrameIndex;
    }
}
