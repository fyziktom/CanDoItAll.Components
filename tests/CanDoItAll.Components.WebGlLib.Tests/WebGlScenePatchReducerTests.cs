using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlScenePatchReducerTests
{
    [Fact]
    public void Apply_updates_object_transform_and_revision()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            UiState = { Revision = 4 },
            Objects =
            [
                new WebGlSceneObject
                {
                    Id = "object.a",
                    Position = WebGlVector3.Zero
                }
            ]
        };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            BaseRevision = 4,
            NextRevision = 5,
            ObjectPatches =
            [
                new WebGlSceneObjectPatch
                {
                    ObjectId = "object.a",
                    Position = new WebGlVector3(1, 0, 2),
                    Color = "#38bdf8"
                }
            ]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.True(result.IsValid);
        Assert.Equal(new WebGlVector3(1, 0, 2), scene.Objects[0].Position);
        Assert.Equal("#38bdf8", scene.Objects[0].Color);
        Assert.Equal(5, scene.UiState.Revision);
        Assert.Contains("object.a", result.PatchedObjectIds);
    }

    [Fact]
    public void Validate_rejects_wrong_scene_id()
    {
        var scene = new WebGlSceneModel { SceneId = "expected" };
        var patch = new WebGlScenePatch { SceneId = "actual" };

        var result = new WebGlScenePatchReducer().Validate(scene, patch);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("does not match", StringComparison.Ordinal));
    }
}
