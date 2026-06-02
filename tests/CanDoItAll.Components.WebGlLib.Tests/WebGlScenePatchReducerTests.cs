using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlScenePatchReducerTests
{
    [Fact]
    public void Apply_uses_scene_revision_as_canonical_and_mirrors_ui_revision()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Revision = 7,
            UiState = { Revision = 2 },
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
            BaseRevision = 7,
            ObjectPatches =
            [
                new WebGlSceneObjectPatch
                {
                    ObjectId = "object.a",
                    Position = new WebGlVector3(1, 0, 0)
                }
            ]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.True(result.Success);
        Assert.Equal(8, result.Revision);
        Assert.Equal(8, result.NextRevision);
        Assert.Equal(8, scene.Revision);
        Assert.Equal(8, scene.UiState.Revision);
    }

    [Fact]
    public void Apply_updates_object_transform_and_revision()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Revision = 4,
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
        Assert.True(result.Success);
        Assert.Equal("scene", result.SceneId);
        Assert.Equal("patch", result.CommandKind);
        Assert.Equal(5, result.Revision);
        Assert.Equal(new WebGlVector3(1, 0, 2), scene.Objects[0].Position);
        Assert.Equal("#38bdf8", scene.Objects[0].Color);
        Assert.Equal(5, scene.Revision);
        Assert.Equal(5, scene.UiState.Revision);
        Assert.Contains("object.a", result.PatchedObjectIds);
        Assert.Contains("object.a", result.AffectedObjectIds);
    }

    [Fact]
    public void Validate_rejects_wrong_scene_id()
    {
        var scene = new WebGlSceneModel { SceneId = "expected" };
        var patch = new WebGlScenePatch { SceneId = "actual" };

        var result = new WebGlScenePatchReducer().Validate(scene, patch);

        Assert.False(result.IsValid);
        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("does not match", StringComparison.Ordinal));
    }

    [Fact]
    public void Apply_rejects_added_object_without_id_without_throwing()
    {
        var scene = new WebGlSceneModel { SceneId = "scene" };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            AddObjects = [new WebGlSceneObject()]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("Added object id", StringComparison.Ordinal));
        Assert.Empty(scene.Objects);
    }

    [Fact]
    public void Validate_can_fail_strict_base_revision()
    {
        var scene = new WebGlSceneModel { SceneId = "scene", Revision = 4, UiState = { Revision = 1 } };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            BaseRevision = 3,
            Metadata = { ["strictBaseRevision"] = "true" }
        };

        var result = new WebGlScenePatchReducer().Validate(scene, patch);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("base revision", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_rejects_added_link_missing_endpoint_by_default()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Objects = [new WebGlSceneObject { Id = "object.a" }]
        };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            AddLinks =
            [
                new WebGlSceneLink
                {
                    Id = "link.missing",
                    SourceObjectId = "object.a",
                    TargetObjectId = "object.missing"
                }
            ]
        };

        var result = new WebGlScenePatchReducer().Validate(scene, patch);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("missing endpoint", StringComparison.Ordinal));
    }

    [Fact]
    public void Apply_strict_invalid_link_is_all_or_none_and_reports_transaction_metadata()
    {
        var scene = CreatePatchTransactionScene();
        var originalPosition = scene.Objects.Single(item => item.Id == "object.a").Position;
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            Metadata = { ["patchTransactionMode"] = "strict" },
            ObjectPatches =
            [
                new WebGlSceneObjectPatch
                {
                    ObjectId = "object.a",
                    Position = new WebGlVector3(3, 0, 0)
                }
            ],
            AddLinks =
            [
                new WebGlSceneLink
                {
                    Id = "link.bad",
                    SourceObjectId = "object.a",
                    TargetObjectId = "object.missing"
                }
            ]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.False(result.Success);
        Assert.Equal(originalPosition, scene.Objects.Single(item => item.Id == "object.a").Position);
        Assert.Empty(result.AffectedObjectIds);
        Assert.Empty(result.AffectedLinkIds);
        var metadata = ReadMetadata(result);
        Assert.Equal("strict", metadata["patchTransactionMode"]);
        Assert.Equal("fail", metadata["missingLinkEndpointMode"]);
        Assert.Equal("mixed-incremental", metadata["patchClassification"]);
    }

    [Fact]
    public void Apply_permissive_invalid_link_skips_bad_link_and_reports_affected_and_skipped_ids()
    {
        var scene = CreatePatchTransactionScene();
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            Metadata = { ["patchTransactionMode"] = "permissive-invalid-links" },
            ObjectPatches =
            [
                new WebGlSceneObjectPatch
                {
                    ObjectId = "object.a",
                    Position = new WebGlVector3(2, 0, 0)
                }
            ],
            AddLinks =
            [
                new WebGlSceneLink
                {
                    Id = "link.good",
                    SourceObjectId = "object.a",
                    TargetObjectId = "object.b"
                },
                new WebGlSceneLink
                {
                    Id = "link.bad",
                    SourceObjectId = "object.a",
                    TargetObjectId = "object.missing"
                }
            ]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.True(result.Success);
        Assert.Equal(new WebGlVector3(2, 0, 0), scene.Objects.Single(item => item.Id == "object.a").Position);
        Assert.Contains(scene.Links, link => link.Id == "link.good");
        Assert.DoesNotContain(scene.Links, link => link.Id == "link.bad");
        Assert.Equal(["object.a"], result.AffectedObjectIds);
        Assert.Equal(["link.good"], result.AffectedLinkIds);
        Assert.Contains(result.Warnings, warning => warning.Contains("link.bad", StringComparison.Ordinal));
        var metadata = ReadMetadata(result);
        Assert.Equal("permissive-invalid-links", metadata["patchTransactionMode"]);
        Assert.Equal("warn", metadata["missingLinkEndpointMode"]);
        Assert.Equal("link.bad", metadata["skippedLinkIds"]);
        Assert.Equal("mixed-incremental", metadata["patchClassification"]);
    }

    [Fact]
    public void Apply_deduplicates_affected_ids_and_increments_revision_once()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Revision = 4,
            UiState = { Revision = 4 },
            Objects =
            [
                new WebGlSceneObject { Id = "object.a" }
            ]
        };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            ObjectPatches =
            [
                new WebGlSceneObjectPatch { ObjectId = "object.a", Position = new WebGlVector3(1, 0, 0) },
                new WebGlSceneObjectPatch { ObjectId = "object.a", Color = "#f00" }
            ]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.True(result.Success);
        Assert.Equal(5, result.Revision);
        Assert.Equal(["object.a"], result.AffectedObjectIds);
    }

    [Fact]
    public void Apply_rejects_object_patch_missing_after_removal_without_partial_commit()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Revision = 4,
            UiState = { Revision = 4 },
            Objects =
            [
                new WebGlSceneObject { Id = "object.a" },
                new WebGlSceneObject { Id = "object.b" }
            ],
            Links =
            [
                new WebGlSceneLink { Id = "link.ab", SourceObjectId = "object.a", TargetObjectId = "object.b" }
            ],
            Layers =
            [
                new WebGlSceneLayer { Id = "layer.main", ObjectIds = ["object.a", "object.b"] }
            ]
        };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            RemoveObjectIds = ["object.a"],
            ObjectPatches = [new() { ObjectId = "object.a", Color = "#f00" }]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.False(result.Success);
        Assert.Contains(result.Errors, error => error.Contains("Object patch target", StringComparison.Ordinal));
        Assert.Equal(4, scene.Revision);
        Assert.Equal(4, scene.UiState.Revision);
        Assert.Equal(["object.a", "object.b"], scene.Objects.Select(static item => item.Id).ToArray());
        Assert.Equal(["link.ab"], scene.Links.Select(static item => item.Id).ToArray());
        Assert.Equal(["object.a", "object.b"], scene.Layers.Single().ObjectIds);
    }

    [Fact]
    public void Apply_remove_object_cleans_links_layers_and_increments_revision_once()
    {
        var scene = new WebGlSceneModel
        {
            SceneId = "scene",
            Revision = 4,
            UiState = { Revision = 4 },
            Objects =
            [
                new WebGlSceneObject { Id = "object.a" },
                new WebGlSceneObject { Id = "object.b" }
            ],
            Links =
            [
                new WebGlSceneLink { Id = "link.ab", SourceObjectId = "object.a", TargetObjectId = "object.b" }
            ],
            Layers =
            [
                new WebGlSceneLayer { Id = "layer.main", ObjectIds = ["object.a", "object.b"] }
            ]
        };
        var patch = new WebGlScenePatch
        {
            SceneId = "scene",
            BaseRevision = 4,
            RemoveObjectIds = ["object.a"]
        };

        var result = new WebGlScenePatchReducer().Apply(scene, patch);

        Assert.True(result.Success);
        Assert.Equal(5, result.Revision);
        Assert.Equal(5, scene.Revision);
        Assert.Equal(5, scene.UiState.Revision);
        Assert.Equal(["object.b"], scene.Objects.Select(static item => item.Id).ToArray());
        Assert.Empty(scene.Links);
        Assert.Equal(["object.b"], scene.Layers.Single().ObjectIds);
        Assert.Equal(["object.a"], result.RemovedObjectIds);
        Assert.Equal(["link.ab"], result.RemovedLinkIds);
    }

    private static WebGlSceneModel CreatePatchTransactionScene()
        => new()
        {
            SceneId = "scene",
            Revision = 4,
            UiState = { Revision = 4 },
            Objects =
            [
                new WebGlSceneObject { Id = "object.a", Position = WebGlVector3.Zero },
                new WebGlSceneObject { Id = "object.b", Position = new WebGlVector3(1, 0, 0) }
            ]
        };

    private static IReadOnlyDictionary<string, string> ReadMetadata(WebGlScenePatchResult result)
    {
        var metadataProperty = typeof(WebGlScenePatchResult).GetProperty("Metadata");
        Assert.NotNull(metadataProperty);
        var value = metadataProperty!.GetValue(result);
        return Assert.IsAssignableFrom<IReadOnlyDictionary<string, string>>(value);
    }
}
