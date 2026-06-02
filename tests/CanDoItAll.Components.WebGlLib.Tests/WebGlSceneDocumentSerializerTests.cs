using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlSceneDocumentSerializerTests
{
    [Fact]
    public void Serialize_round_trips_generic_scene_document()
    {
        var document = new WebGlSceneDocument
        {
            DocumentId = "doc-1",
            SavedAtUtc = new DateTimeOffset(2026, 5, 29, 12, 0, 0, TimeSpan.Zero),
            Source = "unit-test",
            Scene = new WebGlSceneModel
            {
                SceneId = "scene",
                Title = "Scene",
                Objects =
                [
                    new WebGlSceneObject
                    {
                        Id = "object.a",
                        AssetId = "asset.primitive.fallback",
                        Metadata =
                        {
                            ["z"] = "last",
                            ["a"] = "first"
                        }
                    }
                ]
            },
            Metadata =
            {
                ["purpose"] = "roundtrip"
            }
        };

        var json = WebGlSceneDocumentSerializer.Serialize(document);
        var roundTripped = WebGlSceneDocumentSerializer.Deserialize(json);
        var validation = WebGlSceneDocumentSerializer.Validate(roundTripped);

        Assert.True(validation.IsValid);
        Assert.Equal("scene", roundTripped.Scene.SceneId);
        Assert.Equal("object.a", roundTripped.Scene.Objects[0].Id);
        Assert.False(string.IsNullOrWhiteSpace(roundTripped.ContentHash));
        Assert.False(string.IsNullOrWhiteSpace(roundTripped.SceneContentHash));
        Assert.False(string.IsNullOrWhiteSpace(roundTripped.DocumentHash));
    }

    [Fact]
    public void Validate_rejects_run_layer_metadata()
    {
        var document = new WebGlSceneDocument
        {
            Scene = new WebGlSceneModel { SceneId = "scene" },
            Metadata =
            {
                ["run.clock"] = "out-of-boundary"
            }
        };

        var validation = WebGlSceneDocumentSerializer.Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Run-layer", StringComparison.Ordinal));
    }

    [Fact]
    public void Scene_content_hash_ignores_saved_time()
    {
        var document = CreateHashDocument();
        var first = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        document.SavedAtUtc = document.SavedAtUtc.AddHours(6);
        var second = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        Assert.Equal(first.SceneContentHash, second.SceneContentHash);
        Assert.NotEqual(first.DocumentHash, second.DocumentHash);
    }

    [Fact]
    public void Scene_content_hash_changes_when_object_position_changes()
    {
        var document = CreateHashDocument();
        var first = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        document.Scene.Objects[0].Position = new WebGlVector3(2, 0, 0);
        var second = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        Assert.NotEqual(first.SceneContentHash, second.SceneContentHash);
    }

    [Fact]
    public void Scene_content_hash_ignores_hover_and_selection()
    {
        var document = CreateHashDocument();
        var first = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        document.Scene.UiState.HoveredObjectId = "object.a";
        document.Scene.UiState.Selection.SelectedObjectIds.Add("object.a");
        document.Scene.UiState.Selection.PrimaryObjectId = "object.a";
        var second = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        Assert.Equal(first.ContentHash, second.ContentHash);
        Assert.Equal(first.SceneContentHash, second.SceneContentHash);
        Assert.NotEqual(first.DocumentHash, second.DocumentHash);
    }

    [Fact]
    public void Scene_hashes_use_scene_revision_and_normalize_ui_revision_mirror()
    {
        var document = CreateHashDocument();
        document.Scene.Revision = 4;
        document.Scene.UiState.Revision = 20;
        var first = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        document.Scene.UiState.Revision = 21;
        var uiOnly = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        document.Scene.Revision = 5;
        var sceneRevisionChanged = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(document));

        Assert.Equal(first.SceneContentHash, uiOnly.SceneContentHash);
        Assert.Equal(first.DocumentHash, uiOnly.DocumentHash);
        Assert.NotEqual(first.SceneContentHash, sceneRevisionChanged.SceneContentHash);
    }

    [Fact]
    public void Normalize_mirrors_canonical_scene_revision_to_ui_state()
    {
        var document = CreateHashDocument();
        document.Scene.Revision = 7;
        document.Scene.UiState.Revision = 2;

        var normalized = WebGlSceneDocumentSerializer.Normalize(document);

        Assert.Equal(7, normalized.Scene.Revision);
        Assert.Equal(7, normalized.Scene.UiState.Revision);
    }

    [Fact]
    public void Serialize_without_ui_state_keeps_canonical_scene_revision_without_ui_mirror_conflict()
    {
        var document = CreateHashDocument();
        document.Scene.Revision = 7;
        document.Scene.UiState.Revision = 2;

        var serialized = WebGlSceneDocumentSerializer.Serialize(document, new WebGlSceneDocumentSerializerOptions
        {
            IncludeUiState = false
        });
        var roundTripped = WebGlSceneDocumentSerializer.Deserialize(serialized);

        Assert.Equal(7, roundTripped.Scene.Revision);
        Assert.Equal(0, roundTripped.Scene.UiState.Revision);
    }

    [Fact]
    public void Scene_content_hash_ignores_object_link_asset_and_layer_order()
    {
        var firstDocument = CreateHashDocument();
        firstDocument.Scene.Objects.Add(new WebGlSceneObject { Id = "object.b" });
        firstDocument.Scene.Links.Add(new WebGlSceneLink { Id = "link.b", SourceObjectId = "object.a", TargetObjectId = "object.b" });
        firstDocument.Scene.Layers.Add(new WebGlSceneLayer { Id = "layer.b", ObjectIds = ["object.b", "object.a"] });

        var secondDocument = CreateHashDocument();
        secondDocument.Scene.Objects.Insert(0, new WebGlSceneObject { Id = "object.b" });
        secondDocument.Scene.Links.Insert(0, new WebGlSceneLink { Id = "link.b", SourceObjectId = "object.a", TargetObjectId = "object.b" });
        secondDocument.Scene.Layers.Insert(0, new WebGlSceneLayer { Id = "layer.b", ObjectIds = ["object.a", "object.b"] });

        var first = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(firstDocument));
        var second = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(secondDocument));

        Assert.Equal(first.SceneContentHash, second.SceneContentHash);
    }

    [Fact]
    public void Serialize_can_exclude_ui_runtime_options_and_diagnostics()
    {
        var document = CreateHashDocument();
        document.RuntimeOptions.RenderMode = WebGlRenderModes.Continuous;
        document.Diagnostics.RenderCount = 42;
        document.Scene.UiState.ShowGrid = false;
        document.Scene.UiState.Selection.SelectedObjectIds.Add("object.a");

        var serialized = WebGlSceneDocumentSerializer.Serialize(document, new WebGlSceneDocumentSerializerOptions
        {
            IncludeUiState = false,
            IncludeRuntimeOptions = false,
            IncludeDiagnostics = false
        });
        var roundTripped = WebGlSceneDocumentSerializer.Deserialize(serialized);

        Assert.True(roundTripped.Scene.UiState.ShowGrid);
        Assert.Empty(roundTripped.Scene.UiState.Selection.SelectedObjectIds);
        Assert.Equal(WebGlRenderModes.Auto, roundTripped.RuntimeOptions.RenderMode);
        Assert.Equal(0, roundTripped.Diagnostics.RenderCount);
    }

    [Fact]
    public void Validate_rejects_duplicate_object_ids_duplicate_link_ids_and_invalid_vectors()
    {
        var document = CreateHashDocument();
        document.Scene.Objects.Add(new WebGlSceneObject { Id = "object.a" });
        document.Scene.Links.Add(new WebGlSceneLink
        {
            Id = "link.invalid",
            SourceObjectId = "object.a",
            TargetObjectId = "object.missing"
        });
        document.Scene.Links.Add(new WebGlSceneLink
        {
            Id = "link.invalid",
            SourceObjectId = "object.a",
            TargetObjectId = "object.a"
        });
        document.Scene.Objects[0].Position = new WebGlVector3(double.NaN, 0, 0);

        var validation = WebGlSceneDocumentSerializer.Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Duplicate", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("missing endpoint", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("Invalid vector", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_warns_for_missing_asset_references()
    {
        var document = CreateHashDocument();
        document.Scene.Objects[0].AssetId = "asset.missing";

        var validation = WebGlSceneDocumentSerializer.Validate(document);

        Assert.True(validation.IsValid);
        Assert.Contains(validation.Warnings, warning => warning.Contains("asset.missing", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_warns_for_duplicate_and_stale_layer_membership()
    {
        var document = CreateHashDocument();
        document.Scene.Layers.Add(new WebGlSceneLayer
        {
            Id = "layer.operators",
            ObjectIds = ["object.a", "object.a", "object.missing"]
        });

        var validation = WebGlSceneDocumentSerializer.Validate(document);

        Assert.True(validation.IsValid);
        Assert.Contains(validation.Warnings, warning => warning.Contains("duplicate object id 'object.a'", StringComparison.Ordinal));
        Assert.Contains(validation.Warnings, warning => warning.Contains("stale object id 'object.missing'", StringComparison.Ordinal));
    }

    [Fact]
    public void Scene_model_validator_checks_live_scene_layers_and_metadata()
    {
        var scene = CreateHashDocument().Scene;
        scene.Metadata["run.clock"] = "out-of-boundary";
        scene.Layers.Add(new WebGlSceneLayer
        {
            Id = "layer.live",
            ObjectIds = ["object.a", "object.a", "object.missing"]
        });

        var validation = new WebGlSceneModelValidator().Validate(scene);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Run-layer", StringComparison.Ordinal));
        Assert.Contains(validation.Warnings, warning => warning.Contains("duplicate object id 'object.a'", StringComparison.Ordinal));
        Assert.Contains(validation.Warnings, warning => warning.Contains("stale object id 'object.missing'", StringComparison.Ordinal));
    }

    private static WebGlSceneDocument CreateHashDocument()
        => new()
        {
            DocumentId = "doc-hash",
            SavedAtUtc = new DateTimeOffset(2026, 5, 29, 12, 0, 0, TimeSpan.Zero),
            Source = "unit-test",
            Scene = new WebGlSceneModel
            {
                SceneId = "scene",
                AssetCatalog =
                {
                    Assets =
                    [
                        new WebGlAssetDefinition
                        {
                            Id = "asset.primitive.fallback",
                            Format = WebGlAssetFormats.Primitive,
                            PrimitiveKind = WebGlPrimitiveKinds.Box
                        }
                    ]
                },
                Objects =
                [
                    new WebGlSceneObject
                    {
                        Id = "object.a",
                        AssetId = "asset.primitive.fallback",
                        Position = WebGlVector3.Zero
                    }
                ]
            }
        };
}
