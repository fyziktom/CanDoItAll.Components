using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackStopRequest
{
    public string Reason { get; init; } = "playback-stop";

    public Func<string, bool, ValueTask<WebGlSceneCommandResult?>> StopRuntimeAsync { get; init; } =
        static (_, _) => ValueTask.FromResult<WebGlSceneCommandResult?>(null);

    public Action CancelPlayback { get; init; } = static () => { };

    public Task? PlaybackTask { get; init; }

    public TimeSpan PlaybackDrainTimeout { get; init; } = TimeSpan.FromSeconds(2);

    public TimeSpan LateApplyDrainDelay { get; init; } = TimeSpan.Zero;
}

public sealed class WebGlRunPlaybackStopResult
{
    public WebGlSceneCommandResult? ImmediateStopResult { get; init; }

    public WebGlSceneCommandResult? FinalStopResult { get; init; }

    public WebGlSceneCommandResult? LateDrainStopResult { get; init; }

    public bool PlaybackTaskTimedOut { get; init; }

    public bool PlaybackTaskCanceled { get; init; }

    public List<string> Errors { get; } = [];

    public bool Success =>
        !PlaybackTaskTimedOut &&
        Errors.Count == 0 &&
        ImmediateStopResult?.Success != false &&
        FinalStopResult?.Success != false &&
        LateDrainStopResult?.Success != false;
}

public sealed class WebGlRunPlaybackStopCoordinator
{
    public async ValueTask<WebGlRunPlaybackStopResult> StopAsync(WebGlRunPlaybackStopRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var errors = new List<string>();
        string reason = string.IsNullOrWhiteSpace(request.Reason) ? "playback-stop" : request.Reason;
        WebGlSceneCommandResult? immediateStop = await TryStopAsync(request, reason, waitForIdle: false, errors).ConfigureAwait(false);

        try
        {
            request.CancelPlayback();
        }
        catch (Exception ex)
        {
            errors.Add($"Playback cancel failed: {ex.Message}");
        }

        WebGlSceneCommandResult? finalStop = await TryStopAsync(request, reason, waitForIdle: true, errors).ConfigureAwait(false);
        (bool timedOut, bool canceled) = await DrainPlaybackTaskAsync(request.PlaybackTask, request.PlaybackDrainTimeout, errors).ConfigureAwait(false);
        WebGlSceneCommandResult? lateDrainStop = null;
        if (request.LateApplyDrainDelay > TimeSpan.Zero)
        {
            await Task.Delay(request.LateApplyDrainDelay).ConfigureAwait(false);
            lateDrainStop = await TryStopAsync(request, $"{reason} Late apply drain.", waitForIdle: true, errors).ConfigureAwait(false);
        }

        var result = new WebGlRunPlaybackStopResult
        {
            ImmediateStopResult = immediateStop,
            FinalStopResult = finalStop,
            LateDrainStopResult = lateDrainStop,
            PlaybackTaskTimedOut = timedOut,
            PlaybackTaskCanceled = canceled
        };
        result.Errors.AddRange(errors);
        return result;
    }

    private static async ValueTask<WebGlSceneCommandResult?> TryStopAsync(
        WebGlRunPlaybackStopRequest request,
        string reason,
        bool waitForIdle,
        List<string> errors)
    {
        try
        {
            WebGlSceneCommandResult? result = await request.StopRuntimeAsync(reason, waitForIdle).ConfigureAwait(false);
            if (result?.Success == false)
            {
                errors.AddRange(result.Errors.Count == 0
                    ? [$"Runtime stop failed for '{reason}'."]
                    : result.Errors);
            }

            return result;
        }
        catch (Exception ex)
        {
            errors.Add($"Runtime stop failed for '{reason}': {ex.Message}");
            return null;
        }
    }

    private static async ValueTask<(bool TimedOut, bool Canceled)> DrainPlaybackTaskAsync(
        Task? playbackTask,
        TimeSpan timeout,
        List<string> errors)
    {
        if (playbackTask is null || playbackTask.IsCompleted)
        {
            return (false, playbackTask?.IsCanceled == true);
        }

        try
        {
            await playbackTask.WaitAsync(timeout).ConfigureAwait(false);
            return (false, playbackTask.IsCanceled);
        }
        catch (OperationCanceledException)
        {
            return (false, true);
        }
        catch (TimeoutException)
        {
            errors.Add("Playback task did not settle before timeout.");
            return (true, false);
        }
    }
}
