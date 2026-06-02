using System.Text.Json;

namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlSceneDocumentNormalizer
{
    public static WebGlSceneDocument Normalize(WebGlSceneDocument document, WebGlSceneDocumentSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        options ??= new WebGlSceneDocumentSerializerOptions();

        var scene = Clone(document.Scene ?? new WebGlSceneModel());
        scene.AssetCatalog ??= new WebGlAssetCatalog();
        scene.UiState ??= new WebGlSceneUiState();
        WebGlSceneRevisionPolicy.Normalize(scene);
        if (!options.IncludeUiState)
        {
            scene.UiState = new WebGlSceneUiState();
        }

        SortSceneMetadata(scene);
        scene.AssetsByIdSort();

        return new WebGlSceneDocument
        {
            SchemaVersion = string.IsNullOrWhiteSpace(document.SchemaVersion)
                ? WebGlSceneDocumentSerializer.CurrentSchemaVersion
                : document.SchemaVersion,
            DocumentId = document.DocumentId,
            Scene = scene,
            RuntimeOptions = options.IncludeRuntimeOptions
                ? document.RuntimeOptions ?? new WebGlRuntimeOptions()
                : new WebGlRuntimeOptions(),
            Diagnostics = options.IncludeDiagnostics
                ? document.Diagnostics ?? new WebGlRuntimeDiagnostics()
                : new WebGlRuntimeDiagnostics(),
            SavedAtUtc = document.SavedAtUtc.ToUniversalTime(),
            Source = document.Source,
            SceneContentHash = document.SceneContentHash,
            DocumentHash = document.DocumentHash,
            ContentHash = document.ContentHash,
            Metadata = WebGlSceneDocumentMetadataPolicy.Sort(document.Metadata)
        };
    }

    private static void SortSceneMetadata(WebGlSceneModel scene)
    {
        scene.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(scene.Metadata);
        scene.UiState.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(scene.UiState.Metadata);
        foreach (var sceneObject in scene.Objects)
        {
            sceneObject.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(sceneObject.Metadata);
        }

        foreach (var link in scene.Links)
        {
            link.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(link.Metadata);
        }

        foreach (var layer in scene.Layers)
        {
            layer.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(layer.Metadata);
            layer.ObjectIds = layer.ObjectIds
                .Where(static id => !string.IsNullOrWhiteSpace(id))
                .OrderBy(static id => id, StringComparer.Ordinal)
                .ToList();
        }

        scene.AssetCatalog.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(scene.AssetCatalog.Metadata);
        foreach (var asset in scene.AssetCatalog.Assets)
        {
            asset.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(asset.Metadata);
            asset.Tags = asset.Tags.OrderBy(static item => item, StringComparer.Ordinal).ToList();
            foreach (var variant in asset.Variants)
            {
                variant.Metadata = WebGlSceneDocumentMetadataPolicy.Sort(variant.Metadata);
            }
        }
    }

    private static T Clone<T>(T value)
        => JsonSerializer.Deserialize<T>(
               JsonSerializer.Serialize(value, WebGlSceneDocumentSerializer.JsonOptions),
               WebGlSceneDocumentSerializer.JsonOptions)
           ?? throw new JsonException($"Unable to clone {typeof(T).Name}.");
}
