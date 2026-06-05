using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunPlaybackStopCoordinatorTests
{
    [Fact]
    public async Task Stop_async_stops_runtime_before_cancel_then_final_idle_stop_before_task_drain()
    {
        var calls = new List<string>();
        var playbackRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task playbackTask = playbackRelease.Task;
        var coordinator = new WebGlRunPlaybackStopCoordinator();

        WebGlRunPlaybackStopResult result = await coordinator.StopAsync(new()
        {
            Reason = "Paused.",
            StopRuntimeAsync = (reason, waitForIdle) =>
            {
                calls.Add($"stop:{waitForIdle.ToString().ToLowerInvariant()}:{reason}");
                return ValueTask.FromResult<WebGlSceneCommandResult?>(new()
                {
                    Success = true,
                    Metadata = { ["runtimeStopGeneration"] = waitForIdle ? "2" : "1" }
                });
            },
            CancelPlayback = () =>
            {
                calls.Add("cancel");
                playbackRelease.SetResult();
            },
            PlaybackTask = playbackTask,
            PlaybackDrainTimeout = TimeSpan.FromSeconds(1)
        });

        Assert.True(result.Success);
        Assert.Equal(
            [
                "stop:false:Paused.",
                "cancel",
                "stop:true:Paused."
            ],
            calls);
    }

    [Fact]
    public async Task Stop_async_runs_late_drain_after_playback_task_settles()
    {
        var calls = new List<string>();
        var coordinator = new WebGlRunPlaybackStopCoordinator();

        WebGlRunPlaybackStopResult result = await coordinator.StopAsync(new()
        {
            Reason = "Paused.",
            StopRuntimeAsync = (reason, waitForIdle) =>
            {
                calls.Add($"stop:{waitForIdle.ToString().ToLowerInvariant()}:{reason}");
                return ValueTask.FromResult<WebGlSceneCommandResult?>(new() { Success = true });
            },
            CancelPlayback = () => calls.Add("cancel"),
            PlaybackTask = Task.CompletedTask,
            LateApplyDrainDelay = TimeSpan.FromMilliseconds(1)
        });

        Assert.True(result.Success);
        Assert.Equal("stop:true:Paused. Late apply drain.", calls[^1]);
    }

    [Fact]
    public void Runtime_stop_generation_policy_rejects_stale_callbacks()
    {
        var stale = new WebGlSceneCommandResult
        {
            Metadata = { ["runtimeStopGeneration"] = "3" }
        };
        var current = new WebGlSceneCommandResult
        {
            Diagnostics = { ["runtimeStopGeneration"] = "4" }
        };

        Assert.True(WebGlRunRuntimeStopGenerationPolicy.IsStale(stale, currentGeneration: 4));
        Assert.False(WebGlRunRuntimeStopGenerationPolicy.IsStale(current, currentGeneration: 4));
    }
}
