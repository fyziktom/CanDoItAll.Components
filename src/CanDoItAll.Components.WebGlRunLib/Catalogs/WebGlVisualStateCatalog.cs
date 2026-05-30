namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlVisualStateCatalog
{
    public List<WebGlPoseDefinition> Poses { get; set; } = [];

    public List<WebGlSymbolDefinition> Symbols { get; set; } = [];

    public List<WebGlActionBinding> ActionBindings { get; set; } = [];
}

public sealed class WebGlVisualStateCatalogValidationResult
{
    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlVisualStateCatalogValidator
{
    public WebGlVisualStateCatalogValidationResult Validate(WebGlVisualStateCatalog catalog, CanDoItAll.Components.WebGlLib.WebGlAssetCatalog? assetCatalog = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        var result = new WebGlVisualStateCatalogValidationResult();
        HashSet<string> assetIds = assetCatalog?.Assets
            .Where(static asset => !string.IsNullOrWhiteSpace(asset.Id))
            .Select(static asset => asset.Id)
            .ToHashSet(StringComparer.Ordinal) ?? [];
        HashSet<string> variantIds = assetCatalog?.Assets
            .SelectMany(static asset => asset.Variants)
            .Where(static variant => !string.IsNullOrWhiteSpace(variant.Id))
            .Select(static variant => variant.Id)
            .ToHashSet(StringComparer.Ordinal) ?? [];

        AddDuplicateErrors(catalog.Poses.Select(static pose => pose.PoseKey), "pose", result);
        AddDuplicateErrors(catalog.Symbols.Select(static symbol => symbol.SymbolKey), "symbol", result);
        AddDuplicateErrors(catalog.ActionBindings.Select(static binding => binding.ActionKind), "action-binding", result);

        foreach (WebGlPoseDefinition pose in catalog.Poses)
        {
            if (pose.IsNoOpFallback)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(pose.AssetId) && assetCatalog is not null && !assetIds.Contains(pose.AssetId))
            {
                result.Errors.Add($"Pose '{pose.PoseKey}' references missing asset '{pose.AssetId}'.");
            }

            if (!string.IsNullOrWhiteSpace(pose.AssetVariantId) && assetCatalog is not null && !variantIds.Contains(pose.AssetVariantId))
            {
                result.Errors.Add($"Pose '{pose.PoseKey}' references missing asset variant '{pose.AssetVariantId}'.");
            }
        }

        foreach (WebGlSymbolDefinition symbol in catalog.Symbols)
        {
            if (!string.IsNullOrWhiteSpace(symbol.SymbolAssetId) && assetCatalog is not null && !assetIds.Contains(symbol.SymbolAssetId))
            {
                result.Errors.Add($"Symbol '{symbol.SymbolKey}' references missing asset '{symbol.SymbolAssetId}'.");
            }
        }

        HashSet<string> poseKeys = catalog.Poses.Select(static pose => pose.PoseKey).ToHashSet(StringComparer.Ordinal);
        HashSet<string> symbolKeys = catalog.Symbols.Select(static symbol => symbol.SymbolKey).ToHashSet(StringComparer.Ordinal);
        foreach (WebGlActionBinding binding in catalog.ActionBindings)
        {
            if (!string.IsNullOrWhiteSpace(binding.PoseKey) && !poseKeys.Contains(binding.PoseKey))
            {
                result.Errors.Add($"Action binding '{binding.ActionKind}' references missing pose '{binding.PoseKey}'.");
            }

            if (!string.IsNullOrWhiteSpace(binding.SymbolKey) && !symbolKeys.Contains(binding.SymbolKey))
            {
                result.Errors.Add($"Action binding '{binding.ActionKind}' references missing symbol '{binding.SymbolKey}'.");
            }
        }

        if (catalog.Poses.All(static pose => !pose.IsNoOpFallback))
        {
            result.Warnings.Add("Catalog has no no-op fallback pose; runtime resolver will use the built-in fallback.");
        }

        if (catalog.Symbols.All(static symbol => !symbol.IsNoOpFallback))
        {
            result.Warnings.Add("Catalog has no no-op fallback symbol; runtime resolver will use the built-in fallback.");
        }

        return result;
    }

    private static void AddDuplicateErrors(IEnumerable<string> values, string kind, WebGlVisualStateCatalogValidationResult result)
    {
        foreach (string key in values.Where(static value => !string.IsNullOrWhiteSpace(value)).GroupBy(value => value, StringComparer.Ordinal).Where(static group => group.Count() > 1).Select(static group => group.Key))
        {
            result.Errors.Add($"Duplicate {kind} key '{key}'.");
        }
    }
}

public static class WebGlVisualStateCatalogFallbacks
{
    public static WebGlPoseDefinition NoOpPose { get; } = new()
    {
        PoseKey = "noop",
        IsNoOpFallback = true,
        Metadata =
        {
            ["fallback"] = "true"
        }
    };

    public static WebGlSymbolDefinition NoOpSymbol { get; } = new()
    {
        SymbolKey = "noop",
        SemanticKind = "noop",
        Color = "#94a3b8",
        EffectKey = CanDoItAll.Components.WebGlLib.WebGlSymbolEffects.None,
        IsNoOpFallback = true,
        Metadata =
        {
            ["fallback"] = "true"
        }
    };
}
