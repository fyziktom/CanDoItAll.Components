namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlScenePatchPolicy
{
    public const string MetadataPatchTransactionMode = "patchTransactionMode";
    public const string MetadataMissingLinkEndpointMode = "missingLinkEndpointMode";
    public const string MetadataPatchClassification = "patchClassification";
    public const string MetadataSkippedLinkIds = "skippedLinkIds";
    public const string MissingLinkEndpointModeFail = "fail";
    public const string MissingLinkEndpointModeWarn = "warn";

    public static void PopulateResultMetadata(WebGlScenePatchResult result, WebGlScenePatch patch)
    {
        string transactionMode = ResolvePatchTransactionMode(patch);
        result.Metadata[MetadataPatchTransactionMode] = transactionMode;
        result.Metadata[MetadataMissingLinkEndpointMode] = ResolveMissingLinkEndpointMode(patch, transactionMode);
        result.Metadata[MetadataPatchClassification] = ClassifyPatch(patch);
    }

    public static bool IsStrictBaseRevision(WebGlScenePatch patch)
        => string.Equals(ResolveMetadata(patch, "strictBaseRevision"), "true", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(ResolveMetadata(patch, "baseRevisionMode"), "fail", StringComparison.OrdinalIgnoreCase);

    public static string ResolveMissingLinkEndpointMode(WebGlScenePatchResult result)
        => result.Metadata.TryGetValue(MetadataMissingLinkEndpointMode, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : MissingLinkEndpointModeFail;

    public static void AddWarning(WebGlScenePatchResult result, string message)
    {
        if (!string.IsNullOrWhiteSpace(message) &&
            !result.Warnings.Contains(message, StringComparer.Ordinal))
        {
            result.Warnings.Add(message);
        }
    }

    public static void AddSkippedLinkId(WebGlScenePatchResult result, string linkId)
    {
        if (string.IsNullOrWhiteSpace(linkId))
        {
            return;
        }

        List<string> values = result.Metadata.TryGetValue(MetadataSkippedLinkIds, out var existing)
            ? [.. existing.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)]
            : [];
        if (!values.Contains(linkId, StringComparer.Ordinal))
        {
            values.Add(linkId);
        }

        result.Metadata[MetadataSkippedLinkIds] = string.Join(",", values);
    }

    private static string ResolvePatchTransactionMode(WebGlScenePatch patch)
    {
        var explicitMode = ResolveMetadata(patch, MetadataPatchTransactionMode);
        if (IsPermissiveInvalidLinksMode(explicitMode) ||
            string.Equals(ResolveMetadata(patch, MetadataMissingLinkEndpointMode), MissingLinkEndpointModeWarn, StringComparison.OrdinalIgnoreCase))
        {
            return WebGlScenePatchTransactionModes.PermissiveInvalidLinks;
        }

        return WebGlScenePatchTransactionModes.Strict;
    }

    private static string ResolveMissingLinkEndpointMode(WebGlScenePatch patch, string transactionMode)
        => string.Equals(transactionMode, WebGlScenePatchTransactionModes.PermissiveInvalidLinks, StringComparison.Ordinal)
            ? MissingLinkEndpointModeWarn
            : string.Equals(ResolveMetadata(patch, MetadataMissingLinkEndpointMode), MissingLinkEndpointModeWarn, StringComparison.OrdinalIgnoreCase)
                ? MissingLinkEndpointModeWarn
                : MissingLinkEndpointModeFail;

    private static bool IsPermissiveInvalidLinksMode(string mode)
        => string.Equals(mode, WebGlScenePatchTransactionModes.PermissiveInvalidLinks, StringComparison.OrdinalIgnoreCase) ||
           string.Equals(mode, "warn-invalid-links", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(mode, "warning-invalid-links", StringComparison.OrdinalIgnoreCase);

    private static string ResolveMetadata(WebGlScenePatch patch, string key)
        => patch.Metadata is not null && patch.Metadata.TryGetValue(key, out var value) ? value : string.Empty;

    private static string ClassifyPatch(WebGlScenePatch patch)
    {
        bool hasObjectStructure = patch.AddObjects.Count > 0 || patch.RemoveObjectIds.Count > 0;
        bool hasLinkStructure = patch.AddLinks.Count > 0 || patch.RemoveLinkIds.Count > 0;
        if (hasObjectStructure)
        {
            return "graph-structure";
        }

        if (patch.ObjectPatches.Count == 0)
        {
            return hasLinkStructure ? "link-only" : "no-op";
        }

        var activeKinds = patch.ObjectPatches
            .Select(ClassifyObjectPatch)
            .Where(static kind => !string.Equals(kind, "no-op", StringComparison.Ordinal))
            .ToArray();
        if (activeKinds.Length == 0)
        {
            return hasLinkStructure ? "link-only" : "no-op";
        }

        if (!hasLinkStructure && activeKinds.All(static kind => string.Equals(kind, "transform-only", StringComparison.Ordinal)))
        {
            return "transform-only";
        }

        if (!hasLinkStructure && activeKinds.All(static kind => string.Equals(kind, "symbol-only", StringComparison.Ordinal)))
        {
            return "symbol-only";
        }

        if (!hasLinkStructure && activeKinds.All(static kind => string.Equals(kind, "visual-replace", StringComparison.Ordinal)))
        {
            return "visual-replace";
        }

        return activeKinds.All(IsIncrementalObjectPatchKind) ? "mixed-incremental" : "scene-rebuild";
    }

    private static string ClassifyObjectPatch(WebGlSceneObjectPatch patch)
    {
        bool hasTransform = patch.Position.HasValue || patch.Rotation.HasValue || patch.Scale.HasValue;
        bool hasSize = patch.Size.HasValue;
        bool hasSymbols = patch.Symbols is not null;
        bool hasVisual = patch.AssetId is not null || patch.Color is not null || patch.Metadata is not null;
        if (hasTransform && !hasSize && !hasSymbols && !hasVisual)
        {
            return "transform-only";
        }

        if (hasSymbols && !hasTransform && !hasSize && !hasVisual)
        {
            return "symbol-only";
        }

        return hasSize || hasSymbols || hasVisual ? "visual-replace" : "no-op";
    }

    private static bool IsIncrementalObjectPatchKind(string kind)
        => string.Equals(kind, "transform-only", StringComparison.Ordinal) ||
           string.Equals(kind, "symbol-only", StringComparison.Ordinal) ||
           string.Equals(kind, "visual-replace", StringComparison.Ordinal);
}
