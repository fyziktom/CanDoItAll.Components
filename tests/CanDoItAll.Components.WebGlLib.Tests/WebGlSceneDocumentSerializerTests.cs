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
}
