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
        normalized.ContentHash = string.Empty;
        var hashSource = JsonSerializer.Serialize(normalized, JsonOptions);
        normalized.ContentHash = ComputeHash(hashSource);
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

        if (document.Metadata.Keys.Any(static key => key.StartsWith("run.", StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add("Run-layer metadata does not belong in a generic WebGL scene document.");
        }

        return result;
    }

    public static WebGlSceneDocument Normalize(WebGlSceneDocument document)
    {
        var scene = document.Scene ?? new WebGlSceneModel();
        SortSceneMetadata(scene);
        return new WebGlSceneDocument
        {
            SchemaVersion = string.IsNullOrWhiteSpace(document.SchemaVersion) ? CurrentSchemaVersion : document.SchemaVersion,
            DocumentId = document.DocumentId,
            Scene = scene,
            RuntimeOptions = document.RuntimeOptions ?? new WebGlRuntimeOptions(),
            SavedAtUtc = document.SavedAtUtc.ToUniversalTime(),
            Source = document.Source,
            ContentHash = document.ContentHash,
            Metadata = Sort(document.Metadata)
        };
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
}
