using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlSceneDocumentHasher
{
    public static string ComputeSceneContentHash(WebGlSceneDocument document)
    {
        var normalized = WebGlSceneDocumentNormalizer.Normalize(document);
        normalized.DocumentId = string.Empty;
        normalized.SavedAtUtc = DateTimeOffset.UnixEpoch;
        normalized.Source = string.Empty;
        normalized.SceneContentHash = string.Empty;
        normalized.DocumentHash = string.Empty;
        normalized.ContentHash = string.Empty;
        normalized.Metadata = WebGlSceneDocumentMetadataPolicy.FilterSceneContentMetadata(normalized.Metadata);
        normalized.Scene.UiState.Selection = new WebGlSceneSelectionState();
        normalized.Scene.UiState.HoveredObjectId = string.Empty;
        normalized.Scene.UiState.Metadata = WebGlSceneDocumentMetadataPolicy.FilterSceneContentMetadata(normalized.Scene.UiState.Metadata);
        return ComputeHash(JsonSerializer.Serialize(normalized.Scene, WebGlSceneDocumentSerializer.JsonOptions));
    }

    public static string ComputeHash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
