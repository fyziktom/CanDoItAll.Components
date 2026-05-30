using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunPlaybackControllerTests
{
    [Fact]
    public void Timeline_validator_rejects_duplicate_frame_indexes()
    {
        var timeline = new WebGlRunTimeline
        {
            FrameRate = 30,
            Frames =
            [
                new WebGlRunFrame { Index = 1, TimeSeconds = 1 },
                new WebGlRunFrame { Index = 1, TimeSeconds = 2 }
            ]
        };

        var result = new WebGlRunTimelineValidator().Validate(timeline);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("Duplicate frame index", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Seek_backward_replays_frames_from_start_to_target()
    {
        var document = CreateRunDocument();
        var controller = new WebGlRunPlaybackController(document);
        await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Seek, TargetFrameIndex = 3 });

        var result = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Seek, TargetFrameIndex = 1 });

        Assert.True(result.Success);
        Assert.Equal(1, controller.State.CurrentFrameIndex);
        Assert.Equal([0, 1], result.FramesToApply.Select(frame => frame.Index).ToArray());
    }

    [Fact]
    public void Playback_speed_changes_delay_but_not_deterministic_identity()
    {
        var document = CreateRunDocument();
        var clock = new WebGlRunPlaybackClock();
        var identityBefore = clock.BuildDeterministicTimelineIdentity(document);
        var normalDelay = clock.ResolveFrameDelay(document.Timeline, playbackSpeed: 1);
        var fastDelay = clock.ResolveFrameDelay(document.Timeline, playbackSpeed: 2);
        var identityAfter = clock.BuildDeterministicTimelineIdentity(document);

        Assert.Equal(identityBefore, identityAfter);
        Assert.True(fastDelay < normalDelay);
    }

    private static WebGlRunDocument CreateRunDocument()
        => new()
        {
            RunId = new("run"),
            InitialScene = new WebGlSceneDocument { Scene = new WebGlSceneModel { SceneId = "scene" } },
            Timeline =
            {
                FrameRate = 1,
                Frames =
                [
                    new WebGlRunFrame { Index = 0, TimeSeconds = 0 },
                    new WebGlRunFrame { Index = 1, TimeSeconds = 1 },
                    new WebGlRunFrame { Index = 3, TimeSeconds = 3 }
                ]
            }
        };
}
