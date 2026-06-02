namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlSceneDocumentMetadataPolicy
{
    public static Dictionary<string, string> Sort(Dictionary<string, string>? values)
        => values is null
            ? []
            : values.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);

    public static Dictionary<string, string> FilterSceneContentMetadata(Dictionary<string, string>? metadata)
        => Sort(metadata?.Where(static pair => !pair.Key.StartsWith("runtime.", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(static pair => pair.Key, static pair => pair.Value, StringComparer.Ordinal));

    public static IEnumerable<(string Scope, Dictionary<string, string> Metadata)> EnumerateMetadataScopes(WebGlSceneDocument document)
    {
        yield return ("document", document.Metadata);
        foreach (var scope in EnumerateSceneMetadataScopes(document.Scene))
        {
            yield return scope;
        }
    }

    public static IEnumerable<(string Scope, Dictionary<string, string> Metadata)> EnumerateSceneMetadataScopes(WebGlSceneModel scene)
    {
        yield return ("scene", scene.Metadata);
        yield return ("ui-state", scene.UiState.Metadata);
        foreach (var sceneObject in scene.Objects)
        {
            yield return ($"object:{sceneObject.Id}", sceneObject.Metadata);
        }

        foreach (var link in scene.Links)
        {
            yield return ($"link:{link.Id}", link.Metadata);
        }

        foreach (var layer in scene.Layers)
        {
            yield return ($"layer:{layer.Id}", layer.Metadata);
        }
    }

    public static bool IsForbiddenMetadataKey(string key)
    {
        var normalized = key.Trim().ToLowerInvariant();
        return normalized.StartsWith("run.", StringComparison.Ordinal) ||
               normalized.StartsWith("runtime.", StringComparison.Ordinal) ||
               normalized.StartsWith("playback.", StringComparison.Ordinal);
    }
}
