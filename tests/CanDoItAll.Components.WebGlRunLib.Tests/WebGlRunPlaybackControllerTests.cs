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
        document.Metadata["inputPackHash"] = "sha256:input";
        document.Metadata["runPlanHash"] = "sha256:run";
        document.Metadata["visualMappingHash"] = "sha256:visual";
        var controller = new WebGlRunPlaybackController(document);
        await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Seek, TargetFrameIndex = 3 });

        var result = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Seek, TargetFrameIndex = 1 });

        Assert.True(result.Success);
        Assert.Equal(WebGlRunPlaybackCommandKinds.Seek, result.RequestedCommand);
        Assert.Equal(1, result.TargetFrameIndex);
        Assert.Equal(2, result.FramesApplied);
        Assert.Equal(2, result.StagesQueued);
        Assert.Equal("sha256:input", result.RunSourceProvenance["inputPackHash"]);
        Assert.Equal("sha256:run", result.RunSourceProvenance["runPlanHash"]);
        Assert.Equal("sha256:visual", result.RunSourceProvenance["visualMappingHash"]);
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

    [Fact]
    public async Task Controller_exports_executable_state_snapshot_for_generic_document()
    {
        var document = CreateExecutableRunDocument();
        var controller = new WebGlRunPlaybackController(document);

        var reset = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Reset });

        Assert.True(reset.Success);
        Assert.True(reset.RequiresSceneReset);
        Assert.True(controller.State.InitialSceneLoaded);
        Assert.Equal("stage.bootstrap", controller.State.CurrentStageId);
        Assert.Contains("action.bootstrap", controller.State.CurrentActionIds);

        var pause = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Pause });
        Assert.True(pause.Success);
        Assert.False(controller.State.IsPlaying);

        var resumed = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Resume });
        var applyResult = WebGlRunFrameApplyResult.FromFrame(resumed.CurrentFrame!);
        var applier = new RecordingFrameApplier();
        await applier.ApplyAsync(applyResult);

        Assert.True(resumed.Success);
        Assert.True(controller.State.IsPlaying);
        Assert.Equal(1, resumed.TargetFrameIndex);
        Assert.Equal("run-frame:1", controller.State.CurrentCommandBatchId);
        Assert.Equal(["stage.move", "stage.symbol"], controller.State.CurrentStageIds.ToArray());
        Assert.Contains("action.move", controller.State.CurrentActionIds);
        Assert.Equal(["stage.move", "stage.symbol"], applyResult.CommandBatch.Stages.Select(stage => stage.StageId).ToArray());
        Assert.Single(applyResult.CommandBatch.Stages[0].Motions);
        Assert.Single(applier.AppliedFrames);

        var snapshot = controller.ExportRuntimeSnapshot();
        Assert.Equal("run.executable", snapshot.RunId);
        Assert.Equal("scene.executable", snapshot.InitialSceneId);
        Assert.Equal(1, snapshot.InitialObjectCount);
        Assert.Equal(1, snapshot.CurrentFrameIndex);
        Assert.Equal("run-frame:1", snapshot.CurrentCommandBatchId);
        Assert.Equal(["stage.move", "stage.symbol"], snapshot.CurrentStageIds.ToArray());
        Assert.Contains("action.move", snapshot.CurrentActionIds);
        Assert.Equal("sha256:input", snapshot.RunSourceProvenance["inputPackHash"]);

        var seek = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Seek, TargetFrameIndex = 0 });
        var step = await controller.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Step });
        Assert.True(seek.RequiresSceneReset);
        Assert.Equal(1, step.TargetFrameIndex);
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
                    Frame(0, 0),
                    Frame(1, 1),
                    Frame(3, 3)
                ]
            }
        };

    private static WebGlRunFrame Frame(long index, double timeSeconds)
        => new()
        {
            Index = index,
            TimeSeconds = timeSeconds,
            Stages =
            {
                new WebGlRunActionStage { StageId = $"stage.{index}" }
            }
        };

    private static WebGlRunDocument CreateExecutableRunDocument()
        => new()
        {
            RunId = new("run.executable"),
            Metadata =
            {
                ["inputPackHash"] = "sha256:input",
                ["runPlanHash"] = "sha256:run",
                ["visualMappingHash"] = "sha256:visual"
            },
            InitialScene = new WebGlSceneDocument
            {
                Scene = new WebGlSceneModel
                {
                    SceneId = "scene.executable",
                    Objects =
                    {
                        new WebGlSceneObject { Id = "actor", Kind = "actor", Position = WebGlVector3.Zero }
                    }
                }
            },
            Timeline =
            {
                FrameRate = 1,
                Frames =
                {
                    new WebGlRunFrame
                    {
                        Index = 0,
                        TimeSeconds = 0,
                        Stages =
                        {
                            new WebGlRunActionStage
                            {
                                StageId = "stage.bootstrap",
                                StageIndex = 0,
                                SequenceId = "action.bootstrap",
                                Metadata = { ["actionId"] = "action.bootstrap" },
                                ScenePatches =
                                {
                                    new WebGlRunFramePatch
                                    {
                                        Id = "patch.bootstrap",
                                        Patch = new WebGlScenePatch { SceneId = "scene.executable" }
                                    }
                                }
                            }
                        }
                    },
                    new WebGlRunFrame
                    {
                        Index = 1,
                        TimeSeconds = 1,
                        Stages =
                        {
                            new WebGlRunActionStage
                            {
                                StageId = "stage.move",
                                StageIndex = 0,
                                SequenceId = "action.move",
                                BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
                                BarrierObjectIds = { "actor" },
                                Motions =
                                {
                                    new WebGlObjectMotionCommand
                                    {
                                        MotionId = "motion.actor.to.target",
                                        ObjectId = "actor",
                                        TargetPosition = new WebGlVector3(4, 0, 0),
                                        DurationSeconds = 1,
                                        QueueMode = WebGlMotionQueueModes.Append,
                                        Metadata = { ["actionId"] = "action.move" }
                                    }
                                }
                            },
                            new WebGlRunActionStage
                            {
                                StageId = "stage.symbol",
                                StageIndex = 1,
                                SequenceId = "action.symbol",
                                Metadata = { ["actionId"] = "action.symbol" },
                                ScenePatches =
                                {
                                    new WebGlRunFramePatch
                                    {
                                        Id = "patch.symbol",
                                        Patch = new WebGlScenePatch
                                        {
                                            SceneId = "scene.executable",
                                            ObjectPatches =
                                            {
                                                new WebGlSceneObjectPatch { ObjectId = "actor", Metadata = new Dictionary<string, string> { ["state"] = "active" } }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

    private sealed class RecordingFrameApplier : IWebGlRunFrameApplier
    {
        public List<WebGlRunFrameApplyResult> AppliedFrames { get; } = [];

        public ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default)
        {
            AppliedFrames.Add(frame);
            return ValueTask.CompletedTask;
        }
    }
}
