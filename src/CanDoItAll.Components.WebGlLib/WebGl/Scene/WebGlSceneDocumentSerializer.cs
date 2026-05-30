using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanDoItAll.Components.WebGlLib;

public static class WebGlSceneDocumentSerializer
{
    public const string CurrentSchemaVersion = "webgl-scene-document/v1";

    internal static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static string Serialize(WebGlSceneDocument document, WebGlSceneDocumentSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        var normalized = Normalize(document, options);
        normalized.SceneContentHash = ComputeSceneContentHash(normalized);
        normalized.ContentHash = string.Empty;
        normalized.DocumentHash = string.Empty;
        normalized.ContentHash = normalized.SceneContentHash;
        normalized.DocumentHash = WebGlSceneDocumentHasher.ComputeHash(JsonSerializer.Serialize(normalized, JsonOptions));
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
        => WebGlSceneDocumentValidator.Validate(document);

    public static WebGlSceneDocument Normalize(WebGlSceneDocument document, WebGlSceneDocumentSerializerOptions? options = null)
        => WebGlSceneDocumentNormalizer.Normalize(document, options);

    public static string ComputeSceneContentHash(WebGlSceneDocument document)
        => WebGlSceneDocumentHasher.ComputeSceneContentHash(document);
}
