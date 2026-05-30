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
