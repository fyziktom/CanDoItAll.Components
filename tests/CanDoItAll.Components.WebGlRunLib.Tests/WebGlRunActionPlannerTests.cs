using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunActionPlannerTests
{
    [Fact]
    public void Planner_moves_actor_to_target_object_anchor_and_back_home()
    {
        var planner = new WebGlRunActionPlanner();
        WebGlRunPlanningContext context = CreatePlanningContext();

        WebGlRunActionPlan move = planner.Plan(new()
        {
            ActionId = "move.actor.to.target",
            Kind = WebGlRunActionKinds.MoveToObject,
            ObjectId = "actor",
            Target = new() { ObjectId = "target", AnchorKey = WebGlRunAnchorKeys.Use, Offset = new WebGlVector3(0.25, 0, 0) },
            DurationSeconds = 0.5
        }, context);
        WebGlRunActionPlan back = planner.Plan(new()
        {
            ActionId = "return.actor.home",
            Kind = WebGlRunActionKinds.ReturnToAnchor,
            ObjectId = "actor",
            Target = new() { AnchorKey = WebGlRunAnchorKeys.Home },
            DurationSeconds = 0.5
        }, context);

        Assert.True(move.IsValid, string.Join(Environment.NewLine, move.Errors));
        Assert.True(back.IsValid, string.Join(Environment.NewLine, back.Errors));
        Assert.Equal(new WebGlVector3(4.25, 0, 1), move.Motions.Single().TargetPosition);
        Assert.Equal(new WebGlVector3(-3, 0, 0), back.Motions.Single().TargetPosition);
    }

    [Fact]
    public void Planner_maps_sequence_pose_symbol_and_movement_through_catalog()
    {
        var planner = new WebGlRunActionPlanner();
        WebGlRunPlanningContext context = CreatePlanningContext();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "sequence.work",
            Kind = WebGlRunActionKinds.Sequence,
            Steps =
            [
                new() { ActionId = "pose.work", Kind = WebGlRunActionKinds.ChangePose, ObjectId = "actor", PoseKey = "working" },
                new() { ActionId = "symbol.resource", Kind = WebGlRunActionKinds.ShowSymbol, ObjectId = "actor", SymbolKey = "resource" },
                new() { ActionId = "move.target", Kind = WebGlRunActionKinds.MoveToObject, ObjectId = "actor", Target = new() { ObjectId = "target" }, DurationSeconds = 0.25 }
            ]
        }, context);

        Assert.True(plan.IsValid, string.Join(Environment.NewLine, plan.Errors));
        Assert.Single(plan.Motions);
        Assert.Equal(2, plan.Patches.Count);
        Assert.Equal("asset.person.working", plan.Patches[0].ObjectPatches[0].AssetId);
        Assert.Equal("resource", plan.Patches[1].ObjectPatches[0].Symbols![0].SemanticKind);
    }

    [Fact]
    public void Planner_returns_failed_diagnostic_for_unresolved_target_without_throwing()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "move.missing",
            Kind = WebGlRunActionKinds.MoveToObject,
            ObjectId = "actor",
            Target = new() { ObjectId = "missing" }
        }, CreatePlanningContext());

        Assert.False(plan.IsValid);
        Assert.Empty(plan.Motions);
        Assert.Contains(plan.Errors, error => error.Contains("missing", StringComparison.OrdinalIgnoreCase));
    }

    private static WebGlRunPlanningContext CreatePlanningContext()
        => new()
        {
            Scene = new()
            {
                SceneId = "planner-scene",
                Objects =
                [
                    new()
                    {
                        Id = "actor",
                        Position = new WebGlVector3(-2, 0, 0),
                        Anchors =
                        [
                            new() { Key = WebGlRunAnchorKeys.Home, Position = new WebGlVector3(-3, 0, 0) }
                        ]
                    },
                    new()
                    {
                        Id = "target",
                        Position = new WebGlVector3(4, 0, 0),
                        Size = new WebGlVector3(2, 2, 2),
                        Anchors =
                        [
                            new() { Key = WebGlRunAnchorKeys.Use, Position = new WebGlVector3(4, 0, 1) }
                        ]
                    }
                ]
            },
            VisualStates = new()
            {
                Poses =
                [
                    new() { PoseKey = "working", AssetId = "asset.person.working" }
                ],
                Symbols =
                [
                    new() { SymbolKey = "resource", SemanticKind = "resource", Color = "#22c55e" }
                ]
            }
        };
}
