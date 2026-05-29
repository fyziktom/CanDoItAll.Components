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
