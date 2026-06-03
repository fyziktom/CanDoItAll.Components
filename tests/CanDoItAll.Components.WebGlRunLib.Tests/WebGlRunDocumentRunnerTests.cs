using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunDocumentRunnerTests
{
    [Fact]
    public async Task Runner_applies_generic_frame_stages_with_traceable_diagnostics()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateRunDocument();

        WebGlRunExecutionResult load = await runner.LoadAsync(document);
        WebGlRunExecutionResult seek = await runner.SeekAsync(1);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.True(load.Succeeded);
        Assert.True(seek.Succeeded);
        Assert.True(apply.Succeeded);
        Assert.True(runner.State.InitialSceneLoaded);
        Assert.Equal(1, runner.State.CurrentFrameIndex);
        Assert.Equal(["stage.move", "stage.pose", "stage.return"], apply.AppliedStageIds.ToArray());
        Assert.Equal(["stage.move", "stage.pose", "stage.return"], runner.State.CompletedStageIds.ToArray());
        Assert.Empty(runner.State.PendingStageIds);
        Assert.Equal("run-frame:1", apply.Diagnostics["commandBatchId"]);
        Assert.Equal("3", apply.Diagnostics["stageCount"]);
        Assert.Equal("0", apply.Diagnostics["droppedDuplicateMotionCount"]);
        Assert.Equal("source.frame.1", apply.Diagnostics["sourceFrameId"]);
        Assert.Equal("source.stage.move,source.stage.pose,source.stage.return", apply.Diagnostics["sourceStageIds"]);

        WebGlRunFrameApplyResult appliedFrame = Assert.Single(applier.AppliedFrames);
        Assert.Equal(1, appliedFrame.FrameIndex);
        Assert.Equal(["stage.move", "stage.pose", "stage.return"], appliedFrame.CommandBatch.Stages.Select(stage => stage.StageId).ToArray());
        Assert.Equal([1, 0, 1], appliedFrame.CommandBatch.Stages.Select(stage => stage.Motions.Count).ToArray());
        Assert.Equal(WebGlSceneStageBarrierPolicies.WaitForObjectMotions, appliedFrame.CommandBatch.Stages[0].BarrierPolicy);
        Assert.Equal(WebGlSceneStageBarrierPolicies.WaitSeconds, appliedFrame.CommandBatch.Stages[1].BarrierPolicy);
        Assert.Equal(WebGlSceneStageBarrierPolicies.WaitForObjectMotions, appliedFrame.CommandBatch.Stages[2].BarrierPolicy);
        Assert.All(appliedFrame.CommandBatch.Stages, stage => Assert.Equal(WebGlSceneBatchingPolicies.PreserveOrder, stage.BatchingPolicy));
    }

    [Fact]
    public async Task Runner_steps_backward_with_scene_reset_and_traceable_stage_ids()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateRunDocument();

        await runner.LoadAsync(document);
        await runner.SeekAsync(1);
        WebGlRunExecutionResult forward = await runner.ApplyCurrentFrameAsync();
        WebGlRunExecutionResult backward = await runner.StepBackwardAsync();

        Assert.True(forward.Succeeded);
        Assert.True(backward.Succeeded);
        Assert.Equal("step-backward", backward.Operation);
        Assert.Equal(0, runner.State.CurrentFrameIndex);
        Assert.Equal(["stage.bootstrap"], backward.AppliedStageIds.ToArray());
        Assert.Equal("source.stage.bootstrap", backward.Diagnostics["sourceStageIds"]);
        Assert.True(backward.AppliedInitialScene);
    }

    [Fact]
    public async Task Runner_reports_unresolved_runtime_targets_without_applying_frame()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateRunDocument();
        document.Timeline.Frames[1].Stages[0].Motions[0].ObjectId = "missing-actor";

        await runner.LoadAsync(document);
        await runner.SeekAsync(1);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.False(apply.Succeeded);
        Assert.Contains(apply.Errors, error => error.Contains("missing-actor", StringComparison.Ordinal));
        Assert.Equal("1", apply.Diagnostics["failedMotionCount"]);
        Assert.Empty(applier.AppliedFrames);
        Assert.Equal(["stage.move"], runner.State.FailedStageIds.ToArray());
    }

    [Fact]
    public async Task Runner_reports_unresolved_direct_frame_motion_without_applying_frame()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateRunDocument();
        document.Timeline.Frames.Clear();
        document.Timeline.Frames.Add(new()
        {
            Index = 0,
            TimeSeconds = 0,
            Motions =
            {
                new()
                {
                    MotionId = "motion.dynamic.direct",
                    ObjectId = "object.dynamic",
                    TargetPosition = new WebGlVector3(2, 0, 0)
                }
            }
        });

        await runner.LoadAsync(document);
        await runner.SeekAsync(0);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.False(apply.Succeeded);
        Assert.Contains(apply.Errors, error => error.Contains("object.dynamic", StringComparison.Ordinal));
        Assert.Equal("1", apply.Diagnostics["failedMotionCount"]);
        Assert.Empty(applier.AppliedFrames);
    }

    [Fact]
    public async Task Runner_validates_dynamic_object_lifecycle_in_playback_order_not_input_order()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateDynamicObjectRunDocument(motionInSameStage: false);
        WebGlRunFrame frame = document.Timeline.Frames.Single();
        frame.Stages.Reverse();

        WebGlRunExecutionResult load = await runner.LoadAsync(document);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.True(load.Succeeded);
        Assert.True(apply.Succeeded, string.Join(Environment.NewLine, apply.Errors));
        Assert.Equal(["stage.add-object", "stage.move-dynamic"], apply.AppliedStageIds.ToArray());
        Assert.Equal("source.stage.add-object,source.stage.move-dynamic", apply.Diagnostics["sourceStageIds"]);
        WebGlRunFrameApplyResult appliedFrame = Assert.Single(applier.AppliedFrames);
        Assert.Equal(["stage.add-object", "stage.move-dynamic"], appliedFrame.CommandBatch.Stages.Select(static stage => stage.StageId).ToArray());
    }

    [Fact]
    public async Task Runner_rejects_same_stage_motion_to_object_created_by_that_stage()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateDynamicObjectRunDocument(motionInSameStage: true);

        WebGlRunExecutionResult load = await runner.LoadAsync(document);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.True(load.Succeeded);
        Assert.False(apply.Succeeded);
        Assert.Contains(apply.Errors, error => error.Contains("stage.add-and-move", StringComparison.Ordinal) && error.Contains("object.dynamic", StringComparison.Ordinal));
        Assert.Empty(applier.AppliedFrames);
    }

    [Fact]
    public async Task Runner_does_not_apply_when_frame_conversion_fails_after_execution_validation()
    {
        var applier = new RecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);
        WebGlRunDocument document = CreateRunDocument();
        WebGlRunFrame frame = document.Timeline.Frames[1];
        frame.Motions.Add(new()
        {
            MotionId = "motion.direct.actor",
            ObjectId = "actor",
            TargetPosition = new WebGlVector3(2, 0, 0)
        });

        await runner.LoadAsync(document);
        await runner.SeekAsync(1);
        WebGlRunExecutionResult apply = await runner.ApplyCurrentFrameAsync();

        Assert.False(apply.Succeeded);
        Assert.Contains(apply.Errors, error => error.Contains("cannot mix frame-level commands with staged commands", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(applier.AppliedFrames);
        Assert.Contains("stage.move", runner.State.FailedStageIds);
    }

    private static WebGlRunDocument CreateRunDocument()
        => new()
        {
            RunId = new("run.generic"),
            InitialScene = new WebGlSceneDocument
            {
                Scene = new WebGlSceneModel
                {
                    SceneId = "scene.generic",
                    Objects =
                    {
                        new WebGlSceneObject { Id = "actor", Kind = "actor", Position = WebGlVector3.Zero },
                        new WebGlSceneObject { Id = "target", Kind = "target", Position = new WebGlVector3(4, 0, 0) }
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
                        Metadata = { ["sourceFrameId"] = "source.frame.0" },
                        Stages =
                        {
                            new WebGlRunActionStage
                            {
                                StageId = "stage.bootstrap",
                                StageIndex = 0,
                                SequenceId = "action.bootstrap",
                                Metadata =
                                {
                                    ["actionId"] = "action.bootstrap",
                                    ["sourceStageId"] = "source.stage.bootstrap"
                                },
                                ScenePatches =
                                {
                                    new WebGlRunFramePatch
                                    {
                                        Id = "patch.bootstrap",
                                        Patch = new WebGlScenePatch { SceneId = "scene.generic" }
                                    }
                                }
                            }
                        }
                    },
                    new WebGlRunFrame
                    {
                        Index = 1,
                        TimeSeconds = 1,
                        Metadata = { ["sourceFrameId"] = "source.frame.1" },
                        Stages =
                        {
                            new WebGlRunActionStage
                            {
                                StageId = "stage.move",
                                StageIndex = 0,
                                SequenceId = "action.move",
                                BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
                                BarrierObjectIds = { "actor" },
                                Metadata =
                                {
                                    ["actionId"] = "action.move",
                                    ["sourceStageId"] = "source.stage.move"
                                },
                                Motions =
                                {
                                    new WebGlObjectMotionCommand
                                    {
                                        MotionId = "motion.actor.to.target",
                                        ObjectId = "actor",
                                        TargetPosition = new WebGlVector3(4, 0, 0),
                                        DurationSeconds = 0.25,
                                        QueueMode = WebGlMotionQueueModes.Append,
                                        Metadata = { ["actionId"] = "action.move" }
                                    }
                                }
                            },
                            new WebGlRunActionStage
                            {
                                StageId = "stage.pose",
                                StageIndex = 1,
                                SequenceId = "action.pose",
                                WaitSeconds = 0.25,
                                BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitSeconds,
                                Metadata =
                                {
                                    ["actionId"] = "action.pose",
                                    ["sourceStageId"] = "source.stage.pose"
                                },
                                ScenePatches =
                                {
                                    new WebGlRunFramePatch
                                    {
                                        Id = "patch.pose",
                                        Patch = new WebGlScenePatch
                                        {
                                            SceneId = "scene.generic",
                                            ObjectPatches =
                                            {
                                                new WebGlSceneObjectPatch
                                                {
                                                    ObjectId = "actor",
                                                    Metadata = new Dictionary<string, string> { ["poseKey"] = "active" }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new WebGlRunActionStage
                            {
                                StageId = "stage.return",
                                StageIndex = 2,
                                SequenceId = "action.return",
                                BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
                                BarrierObjectIds = { "actor" },
                                Metadata =
                                {
                                    ["actionId"] = "action.return",
                                    ["sourceStageId"] = "source.stage.return"
                                },
                                Motions =
                                {
                                    new WebGlObjectMotionCommand
                                    {
                                        MotionId = "motion.actor.home",
                                        ObjectId = "actor",
                                        TargetPosition = WebGlVector3.Zero,
                                        DurationSeconds = 0.25,
                                        QueueMode = WebGlMotionQueueModes.Append,
                                        Metadata = { ["actionId"] = "action.return" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

    private static WebGlRunDocument CreateDynamicObjectRunDocument(bool motionInSameStage)
    {
        var addStage = new WebGlRunActionStage
        {
            StageId = motionInSameStage ? "stage.add-and-move" : "stage.add-object",
            StageIndex = 0,
            Metadata = { ["sourceStageId"] = motionInSameStage ? "source.stage.add-and-move" : "source.stage.add-object" },
            ScenePatches =
            {
                new()
                {
                    Id = "patch.add-object",
                    Patch = new WebGlScenePatch
                    {
                        SceneId = "scene.dynamic",
                        AddObjects =
                        {
                            new WebGlSceneObject
                            {
                                Id = "object.dynamic",
                                Kind = "target",
                                Position = new WebGlVector3(2, 0, 0)
                            }
                        }
                    }
                }
            }
        };
        if (motionInSameStage)
        {
            addStage.Motions.Add(DynamicObjectMotion("motion.dynamic.same-stage"));
        }

        var frame = new WebGlRunFrame
        {
            Index = 0,
            TimeSeconds = 0,
            Metadata = { ["sourceFrameId"] = "source.frame.dynamic" },
            Stages =
            {
                addStage
            }
        };
        if (!motionInSameStage)
        {
            frame.Stages.Add(new()
            {
                StageId = "stage.move-dynamic",
                StageIndex = 1,
                Metadata = { ["sourceStageId"] = "source.stage.move-dynamic" },
                Motions =
                {
                    DynamicObjectMotion("motion.dynamic.after-add")
                }
            });
        }

        return new()
        {
            RunId = new("run.dynamic-object"),
            InitialScene = new WebGlSceneDocument
            {
                Scene = new WebGlSceneModel
                {
                    SceneId = "scene.dynamic",
                    Objects =
                    {
                        new WebGlSceneObject { Id = "actor", Kind = "actor", Position = WebGlVector3.Zero }
                    }
                }
            },
            Timeline =
            {
                FrameRate = 1,
                Frames = { frame }
            }
        };
    }

    private static WebGlObjectMotionCommand DynamicObjectMotion(string motionId)
        => new()
        {
            MotionId = motionId,
            ObjectId = "object.dynamic",
            TargetPosition = new WebGlVector3(4, 0, 0),
            DurationSeconds = 0.25
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
