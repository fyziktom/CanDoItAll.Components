using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunActionCompilerTests
{
    [Fact]
    public void Compiler_maps_generic_movement_symbol_pose_and_return_actions()
    {
        var timeline = new WebGlRunActionCompiler().Compile(new WebGlRunActionPlan
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding
                {
                    ObjectId = "actor",
                    Position = new WebGlVector3(0, 0, 0),
                    AnchorPosition = new WebGlVector3(0, 0, 0)
                },
                new WebGlRunObjectBinding
                {
                    ObjectId = "target",
                    Position = new WebGlVector3(4, 0, 1)
                }
            ],
            Actions =
            [
                Action("move", WebGlRunActionKinds.MoveToObject, 0, "actor", "target"),
                Action("pose", WebGlRunActionKinds.SetPose, 1, "actor", parameters: new() { ["poseKey"] = "active" }),
                Action("symbol", WebGlRunActionKinds.ShowSymbol, 1, "actor", parameters: new() { ["symbolKind"] = "status" }),
                Action("return", WebGlRunActionKinds.ReturnToAnchor, 2, "actor")
            ]
        });

        Assert.Equal([0, 1, 2], timeline.Frames.Select(frame => frame.Index).ToArray());
        Assert.Equal(new WebGlVector3(4, 0, 1), timeline.Frames[0].Motions[0].TargetPosition);
        Assert.Equal("move-to-object", timeline.Frames[0].Motions[0].Metadata["actionKind"]);
        Assert.Equal("active", timeline.Frames[1].ScenePatches[0].Patch.ObjectPatches[0].Metadata!["poseKey"]);
        Assert.Equal("status", timeline.Frames[1].ScenePatches[1].Patch.ObjectPatches[0].Symbols![0].SemanticKind);
        Assert.Equal(new WebGlVector3(0, 0, 0), timeline.Frames[2].Motions[0].TargetPosition);
    }

    [Fact]
    public void Compiler_keeps_pose_move_admin_and_return_as_ordered_stages()
    {
        var timeline = new WebGlRunActionCompiler().Compile(new WebGlRunActionPlan
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "actor", Position = new WebGlVector3(0, 0, 0), AnchorPosition = new WebGlVector3(0, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "target", Position = new WebGlVector3(4, 0, 0) }
            ],
            Actions =
            [
                Action("pose.walk", WebGlRunActionKinds.ChangePose, 0, "actor", parameters: new() { ["poseKey"] = "walking" }),
                Action("move.target", WebGlRunActionKinds.MoveToObject, 0, "actor", "target"),
                Action("admin.symbol", WebGlRunActionKinds.ShowSymbol, 0, "actor", parameters: new() { ["symbolKind"] = "document" }),
                Action("return.home", WebGlRunActionKinds.ReturnToAnchor, 0, "actor")
            ]
        });

        WebGlRunFrame frame = timeline.Frames.Single();
        WebGlSceneCommandBatch batch = WebGlRunFrameApplyResult.FromFrame(frame).CommandBatch;

        Assert.Equal(["pose.walk", "move.target", "admin.symbol", "return.home"], frame.Stages.Select(stage => stage.StageId).ToArray());
        Assert.Equal(4, batch.Stages.Count);
        Assert.Empty(batch.Motions);
        Assert.Equal([0, 1, 0, 1], batch.Stages.Select(stage => stage.Motions.Count).ToArray());
        Assert.Equal(0, batch.Metadata.GetValueOrDefault("droppedDuplicateMotionCount") is { } value ? int.Parse(value) : -1);
        Assert.All(batch.Stages, stage => Assert.Equal(WebGlSceneBatchingPolicies.PreserveOrder, stage.BatchingPolicy));
        Assert.Equal([0, 1, 2, 3], frame.Stages.Select(stage => stage.StageIndex).ToArray());
    }

    [Fact]
    public void Compiler_preserves_home_well_admin_home_motion_sequence_for_same_actor()
    {
        var timeline = new WebGlRunActionCompiler().Compile(new WebGlRunActionPlan
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "actor", Position = new WebGlVector3(0, 0, 0), AnchorPosition = new WebGlVector3(0, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "well", Position = new WebGlVector3(4, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "admin", Position = new WebGlVector3(8, 0, 0) }
            ],
            Actions =
            [
                new()
                {
                    ActionId = "sequence.shared-resource",
                    ActionKind = WebGlRunActionKinds.Sequence,
                    Steps =
                    [
                        Action("home.to.well", WebGlRunActionKinds.MoveToObject, 0, "actor", "well"),
                        Action("well.to.admin", WebGlRunActionKinds.MoveToObject, 0, "actor", "admin"),
                        Action("admin.to.home", WebGlRunActionKinds.ReturnToAnchor, 0, "actor")
                    ]
                }
            ]
        });

        WebGlSceneCommandBatch batch = WebGlRunFrameApplyResult.FromFrame(timeline.Frames.Single()).CommandBatch;

        Assert.Equal(["home.to.well", "well.to.admin", "admin.to.home"], batch.Stages.Select(stage => stage.StageId).ToArray());
        Assert.Equal([new WebGlVector3(4, 0, 0), new WebGlVector3(8, 0, 0), new WebGlVector3(0, 0, 0)], batch.Stages.Select(stage => stage.Motions.Single().TargetPosition).ToArray());
        Assert.Equal(0, int.Parse(batch.Metadata["droppedDuplicateMotionCount"]));
    }

    [Fact]
    public void Compiler_projects_action_durations_to_stage_waits_for_runtime_scheduling()
    {
        var timeline = new WebGlRunActionCompiler().Compile(new WebGlRunActionPlan
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "actor", Position = new WebGlVector3(0, 0, 0), AnchorPosition = new WebGlVector3(0, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "target", Position = new WebGlVector3(4, 0, 0) }
            ],
            Actions =
            [
                Action("move.target", WebGlRunActionKinds.MoveToObject, 0, "actor", "target"),
                Action("admin.symbol", WebGlRunActionKinds.ShowSymbol, 0, "actor", parameters: new() { ["symbolKind"] = "document" }),
                Action("return.home", WebGlRunActionKinds.ReturnToAnchor, 0, "actor")
            ]
        });

        WebGlSceneCommandBatch batch = WebGlRunFrameApplyResult.FromFrame(timeline.Frames.Single()).CommandBatch;

        Assert.Equal([0.25, 0.25, 0.25], batch.Stages.Select(stage => stage.WaitSeconds).ToArray());
        Assert.Equal(["move.target", "admin.symbol", "return.home"], batch.Stages.Select(stage => stage.StageId).ToArray());
    }

    [Fact]
    public void Compiler_projects_stage_group_and_coalescing_scope_to_command_batch_stages()
    {
        var timeline = new WebGlRunActionCompiler().Compile(new WebGlRunActionPlan
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "actor", Position = new WebGlVector3(0, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "target", Position = new WebGlVector3(4, 0, 0) }
            ],
            Actions =
            [
                new()
                {
                    ActionId = "move.actor",
                    ActionKind = WebGlRunActionKinds.MoveToObject,
                    SubjectObjectId = "actor",
                    TargetObjectId = "target",
                    StartsAtSeconds = 0,
                    DurationSeconds = 0.25,
                    StageGroupId = "event.1",
                    CoalescingScope = WebGlRunCoalescingScopes.None
                }
            ]
        });

        WebGlRunActionStage stage = timeline.Frames.Single().Stages.Single();
        WebGlSceneCommandBatchStage batchStage = WebGlRunFrameApplyResult.FromFrame(timeline.Frames.Single()).CommandBatch.Stages.Single();

        Assert.Equal("event.1", stage.StageGroupId);
        Assert.Equal(WebGlRunCoalescingScopes.None, stage.CoalescingScope);
        Assert.Equal("event.1", stage.Metadata["stageGroupId"]);
        Assert.Equal(WebGlRunCoalescingScopes.None, stage.Metadata["coalescingScope"]);
        Assert.Equal(WebGlSceneBatchingPolicies.PreserveOrder, batchStage.BatchingPolicy);
    }

    [Fact]
    public void Batch_builder_converts_sequence_action_plan_to_traceable_staged_batch()
    {
        var plan = new WebGlRunActionPlan
        {
            ActionId = "resource.sequence",
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "actor", Position = new WebGlVector3(0, 0, 0), AnchorPosition = new WebGlVector3(0, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "target", Position = new WebGlVector3(4, 0, 0) }
            ],
            Actions =
            [
                new()
                {
                    ActionId = "sequence.resource",
                    ActionKind = WebGlRunActionKinds.Sequence,
                    Metadata = { ["visualActionId"] = "visual.1", ["sourceEventId"] = "event.1" },
                    Steps =
                    [
                        Action("pose.carry", WebGlRunActionKinds.ChangePose, 0, "actor", parameters: new() { ["poseKey"] = "carry" }),
                        Action("move.target", WebGlRunActionKinds.MoveToObject, 0, "actor", "target"),
                        Action("return.home", WebGlRunActionKinds.ReturnToAnchor, 0, "actor")
                    ]
                }
            ]
        };
        plan.Actions[0].Steps[1].Metadata["visualActionId"] = "visual.1";
        plan.Actions[0].Steps[1].Metadata["sourceEventId"] = "event.1";

        WebGlSceneCommandBatch batch = new WebGlRunActionPlanBatchCompiler().Compile(plan);

        Assert.Equal("run-plan:resource.sequence", batch.BatchId);
        Assert.Equal(["pose.carry", "move.target", "return.home"], batch.Stages.Select(stage => stage.StageId).ToArray());
        WebGlObjectMotionCommand motion = batch.Stages[1].Motions.Single();
        Assert.Equal(WebGlMotionQueueModes.Append, motion.QueueMode);
        Assert.Equal("move.target", motion.Metadata["actionId"]);
        Assert.Equal("sequence.resource", motion.Metadata["parentActionId"]);
        Assert.Equal("move.target", motion.Metadata["stageId"]);
        Assert.Equal("1", motion.Metadata["stageIndex"]);
        Assert.Equal("visual.1", motion.Metadata["visualActionId"]);
        Assert.Equal("event.1", motion.Metadata["sourceEventId"]);
    }

    private static WebGlRunAction Action(
        string id,
        string kind,
        double startsAt,
        string subject,
        string target = "",
        Dictionary<string, string>? parameters = null)
        => new()
        {
            ActionId = id,
            ActionKind = kind,
            SubjectObjectId = subject,
            TargetObjectId = target,
            StartsAtSeconds = startsAt,
            DurationSeconds = 0.25,
            Parameters = parameters ?? []
        };
}
