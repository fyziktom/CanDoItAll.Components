using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanDoItAll.Components.WebGlLib;

public static class WebGlSceneDocumentSerializer
{
    public const string CurrentSchemaVersion = "webgl-scene-document/v1";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static string Serialize(WebGlSceneDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var normalized = Normalize(document);
        normalized.SceneContentHash = ComputeSceneContentHash(normalized);
        normalized.ContentHash = string.Empty;
        normalized.DocumentHash = string.Empty;
        normalized.ContentHash = normalized.SceneContentHash;
        var documentHashSource = JsonSerializer.Serialize(normalized, JsonOptions);
        normalized.DocumentHash = ComputeHash(documentHashSource);
        return JsonSerializer.Serialize(normalized, JsonOptions);
    }

    public static WebGlSceneDocument Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Scene document JSON is required.", nameof(json));
        }

        return JsonSerializer.Deserialize<WebGlSceneDocument>(json, JsonOptions)
            ?? throw new JsonException("Scene document JSON did not produce a document.");
    }

    public static WebGlSceneDocumentValidationResult Validate(WebGlSceneDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var result = new WebGlSceneDocumentValidationResult();
        if (!string.Equals(document.SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal))
        {
            result.Errors.Add($"Unsupported WebGL scene document schema '{document.SchemaVersion}'.");
        }

        if (string.IsNullOrWhiteSpace(document.Scene.SceneId))
        {
            result.Errors.Add("Scene id is required.");
        }

        ValidateForbiddenMetadata(document, result);
        ValidateObjectIds(document.Scene, result);
        ValidateLinks(document.Scene, result);
        ValidateAssets(document.Scene, result);

        return result;
    }

    public static WebGlSceneDocument Normalize(WebGlSceneDocument document)
    {
        var scene = Clone(document.Scene ?? new WebGlSceneModel());
        SortSceneMetadata(scene);
        scene.AssetsByIdSort();
        return new WebGlSceneDocument
        {
            SchemaVersion = string.IsNullOrWhiteSpace(document.SchemaVersion) ? CurrentSchemaVersion : document.SchemaVersion,
            DocumentId = document.DocumentId,
            Scene = scene,
            RuntimeOptions = document.RuntimeOptions ?? new WebGlRuntimeOptions(),
            SavedAtUtc = document.SavedAtUtc.ToUniversalTime(),
            Source = document.Source,
            SceneContentHash = document.SceneContentHash,
            DocumentHash = document.DocumentHash,
            ContentHash = document.ContentHash,
            Metadata = Sort(document.Metadata)
        };
    }

    public static string ComputeSceneContentHash(WebGlSceneDocument document)
    {
        var normalized = Normalize(document);
        normalized.DocumentId = string.Empty;
        normalized.SavedAtUtc = DateTimeOffset.UnixEpoch;
        normalized.Source = string.Empty;
        normalized.SceneContentHash = string.Empty;
        normalized.DocumentHash = string.Empty;
        normalized.ContentHash = string.Empty;
        normalized.Metadata = FilterSceneContentMetadata(normalized.Metadata);
        normalized.Scene.UiState.Selection = new WebGlSceneSelectionState();
        normalized.Scene.UiState.HoveredObjectId = string.Empty;
        normalized.Scene.UiState.Metadata = FilterSceneContentMetadata(normalized.Scene.UiState.Metadata);
        return ComputeHash(JsonSerializer.Serialize(normalized.Scene, JsonOptions));
    }

    private static void SortSceneMetadata(WebGlSceneModel scene)
    {
        scene.Metadata = Sort(scene.Metadata);
        scene.UiState.Metadata = Sort(scene.UiState.Metadata);
        foreach (var sceneObject in scene.Objects)
        {
            sceneObject.Metadata = Sort(sceneObject.Metadata);
        }

        foreach (var link in scene.Links)
        {
            link.Metadata = Sort(link.Metadata);
        }

        foreach (var layer in scene.Layers)
        {
            layer.Metadata = Sort(layer.Metadata);
            layer.ObjectIds = layer.ObjectIds
                .Where(static id => !string.IsNullOrWhiteSpace(id))
                .OrderBy(static id => id, StringComparer.Ordinal)
                .ToList();
        }

        scene.AssetCatalog.Metadata = Sort(scene.AssetCatalog.Metadata);
        foreach (var asset in scene.AssetCatalog.Assets)
        {
            asset.Metadata = Sort(asset.Metadata);
            asset.Tags = asset.Tags.OrderBy(static item => item, StringComparer.Ordinal).ToList();
            foreach (var variant in asset.Variants)
            {
                variant.Metadata = Sort(variant.Metadata);
            }
        }
    }

    private static Dictionary<string, string> Sort(Dictionary<string, string>? values)
        => values is null
            ? []
            : values.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);

    private static string ComputeHash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static void ValidateForbiddenMetadata(WebGlSceneDocument document, WebGlSceneDocumentValidationResult result)
    {
        foreach (var (scope, metadata) in EnumerateMetadataScopes(document))
        {
            foreach (var key in metadata.Keys)
            {
                if (key.StartsWith("source.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (IsForbiddenMetadataKey(key))
                {
                    result.Errors.Add($"Run-layer or domain metadata key '{key}' does not belong in generic WebGL scene document scope '{scope}'.");
                }
            }
        }
    }

    private static void ValidateObjectIds(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var sceneObject in scene.Objects)
        {
            if (string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                result.Errors.Add("Scene object id is required.");
                continue;
            }

            if (!ids.Add(sceneObject.Id))
            {
                result.Errors.Add($"Duplicate scene object id '{sceneObject.Id}'.");
            }
        }
    }

    private static void ValidateLinks(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var ids = scene.Objects
            .Select(static item => item.Id)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var link in scene.Links)
        {
            if (string.IsNullOrWhiteSpace(link.Id))
            {
                result.Errors.Add("Scene link id is required.");
            }

            if (!ids.Contains(link.SourceObjectId) || !ids.Contains(link.TargetObjectId))
            {
                result.Errors.Add($"Scene link '{link.Id}' references missing endpoint(s): '{link.SourceObjectId}' -> '{link.TargetObjectId}'.");
            }
        }
    }

    private static void ValidateAssets(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var assetIds = scene.AssetCatalog.Assets
            .Select(static item => item.Id)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sceneObject in scene.Objects.Where(item => !string.IsNullOrWhiteSpace(item.AssetId) && !assetIds.Contains(item.AssetId)))
        {
            result.Warnings.Add($"Scene object '{sceneObject.Id}' references asset '{sceneObject.AssetId}' that is not present in the catalog.");
        }

        foreach (var asset in scene.AssetCatalog.Assets)
        {
            if (!string.IsNullOrWhiteSpace(asset.FallbackAssetId) && !assetIds.Contains(asset.FallbackAssetId))
            {
                result.Warnings.Add($"Asset '{asset.Id}' references fallback asset '{asset.FallbackAssetId}' that is not present in the catalog.");
            }

            foreach (var variant in asset.Variants.Where(variant => !string.IsNullOrWhiteSpace(variant.FallbackAssetId) && !assetIds.Contains(variant.FallbackAssetId)))
            {
                result.Warnings.Add($"Asset '{asset.Id}' variant '{variant.Id}' references fallback asset '{variant.FallbackAssetId}' that is not present in the catalog.");
            }
        }
    }

    private static IEnumerable<(string Scope, Dictionary<string, string> Metadata)> EnumerateMetadataScopes(WebGlSceneDocument document)
    {
        yield return ("document", document.Metadata);
        yield return ("scene", document.Scene.Metadata);
        yield return ("ui-state", document.Scene.UiState.Metadata);
        foreach (var sceneObject in document.Scene.Objects)
        {
            yield return ($"object:{sceneObject.Id}", sceneObject.Metadata);
        }

        foreach (var link in document.Scene.Links)
        {
            yield return ($"link:{link.Id}", link.Metadata);
        }

        foreach (var layer in document.Scene.Layers)
        {
            yield return ($"layer:{layer.Id}", layer.Metadata);
        }
    }

    private static bool IsForbiddenMetadataKey(string key)
    {
        var normalized = key.Trim().ToLowerInvariant();
        return normalized.StartsWith("run.", StringComparison.Ordinal) ||
               normalized.StartsWith("economy.", StringComparison.Ordinal) ||
               normalized.StartsWith("ledger.", StringComparison.Ordinal) ||
               normalized.StartsWith("account.", StringComparison.Ordinal) ||
               normalized.StartsWith("market.", StringComparison.Ordinal);
    }

    private static Dictionary<string, string> FilterSceneContentMetadata(Dictionary<string, string>? metadata)
        => Sort(metadata?.Where(static pair => !pair.Key.StartsWith("runtime.", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(static pair => pair.Key, static pair => pair.Value, StringComparer.Ordinal));

    private static T Clone<T>(T value)
        => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, JsonOptions), JsonOptions)
           ?? throw new JsonException($"Unable to clone {typeof(T).Name}.");
}

file static class WebGlSceneDocumentSortExtensions
{
    public static void AssetsByIdSort(this WebGlSceneModel scene)
    {
        scene.AssetCatalog.Assets = scene.AssetCatalog.Assets
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Objects = scene.Objects
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Links = scene.Links
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Layers = scene.Layers
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
    }
}
