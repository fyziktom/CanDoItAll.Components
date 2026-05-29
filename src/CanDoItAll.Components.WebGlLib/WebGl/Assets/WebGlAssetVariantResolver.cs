namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetVariantResolver
{
    private static readonly string[] ExplicitVariantKeys = ["assetVariantId", "preferredAssetVariantId"];

    public WebGlResolvedAssetVariant Resolve(
        WebGlAssetDefinition asset,
        WebGlSceneObject? sceneObject,
        string? qualityProfile)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var profile = NormalizeProfile(qualityProfile);
        var explicitVariantId = ResolveExplicitVariantId(sceneObject);
        var variant = ResolveExplicitVariant(asset, explicitVariantId)
            ?? ResolveProfileVariant(asset, profile)
            ?? ResolveCompatibleVariant(asset, profile);

        if (variant is null)
        {
            return WebGlResolvedAssetVariant.FromBaseAsset(asset, profile);
        }

        return WebGlResolvedAssetVariant.FromVariant(asset, variant, profile);
    }

    public static string NormalizeProfile(string? profile)
    {
        if (string.IsNullOrWhiteSpace(profile))
        {
            return WebGlAssetQualityProfiles.Primitive;
        }

        return profile.Trim().ToLowerInvariant() switch
        {
            "mixed" => WebGlAssetQualityProfiles.MixedGlb,
            "glb-mixed" => WebGlAssetQualityProfiles.MixedGlb,
            "model" => WebGlAssetQualityProfiles.MixedGlb,
            "high" => WebGlAssetQualityProfiles.HighDetailGlb,
            "glb-high" => WebGlAssetQualityProfiles.HighDetailGlb,
            "model-high" => WebGlAssetQualityProfiles.HighDetailGlb,
            "model-medium" => WebGlAssetQualityTiers.ModelMedium,
            "model-low" => WebGlAssetQualityProfiles.MixedGlb,
            "primitive" => WebGlAssetQualityProfiles.Primitive,
            _ => profile.Trim().ToLowerInvariant()
        };
    }

    private static WebGlAssetVariant? ResolveExplicitVariant(WebGlAssetDefinition asset, string explicitVariantId)
    {
        if (string.IsNullOrWhiteSpace(explicitVariantId))
        {
            return null;
        }

        return asset.Variants.FirstOrDefault(
            variant => string.Equals(variant.Id, explicitVariantId, StringComparison.OrdinalIgnoreCase));
    }

    private static WebGlAssetVariant? ResolveProfileVariant(WebGlAssetDefinition asset, string profile)
        => asset.Variants.FirstOrDefault(
            variant => string.Equals(variant.QualityTier, profile, StringComparison.OrdinalIgnoreCase));

    private static WebGlAssetVariant? ResolveCompatibleVariant(WebGlAssetDefinition asset, string profile)
    {
        if (string.Equals(profile, WebGlAssetQualityProfiles.Primitive, StringComparison.OrdinalIgnoreCase))
        {
            return asset.Variants.FirstOrDefault(
                variant => string.Equals(variant.Format, WebGlAssetFormats.Primitive, StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(variant.QualityTier, WebGlAssetQualityTiers.Primitive, StringComparison.OrdinalIgnoreCase));
        }

        if (string.Equals(profile, WebGlAssetQualityProfiles.HighDetailGlb, StringComparison.OrdinalIgnoreCase))
        {
            return asset.Variants.FirstOrDefault(
                variant => string.Equals(variant.QualityTier, WebGlAssetQualityTiers.ModelMedium, StringComparison.OrdinalIgnoreCase))
                ?? asset.Variants.FirstOrDefault(
                    variant => string.Equals(variant.QualityTier, WebGlAssetQualityTiers.ModelLow, StringComparison.OrdinalIgnoreCase));
        }

        return asset.Variants.FirstOrDefault(
            variant => string.Equals(variant.QualityTier, WebGlAssetQualityTiers.ModelLow, StringComparison.OrdinalIgnoreCase))
            ?? asset.Variants.FirstOrDefault();
    }

    private static string ResolveExplicitVariantId(WebGlSceneObject? sceneObject)
    {
        if (sceneObject?.Metadata is null)
        {
            return string.Empty;
        }

        foreach (var key in ExplicitVariantKeys)
        {
            if (sceneObject.Metadata.TryGetValue(key, out var variantId) && !string.IsNullOrWhiteSpace(variantId))
            {
                return variantId;
            }
        }

        return string.Empty;
    }
}

public sealed class WebGlResolvedAssetVariant
{
    public string AssetId { get; init; } = string.Empty;

    public string VariantId { get; init; } = string.Empty;

    public string Uri { get; init; } = string.Empty;

    public string Format { get; init; } = WebGlAssetFormats.Primitive;

    public string PrimitiveKind { get; init; } = WebGlPrimitiveKinds.Box;

    public string FallbackAssetId { get; init; } = string.Empty;

    public string QualityTier { get; init; } = WebGlAssetQualityTiers.Unknown;

    public string Color { get; init; } = "#ffffff";

    public WebGlVector3 Scale { get; init; } = WebGlVector3.One;

    public WebGlAssetPerformanceHint PerformanceHint { get; init; } = new();

    public string RequestedProfile { get; init; } = WebGlAssetQualityProfiles.Primitive;

    public bool UsesVariant => !string.IsNullOrWhiteSpace(VariantId);

    public static WebGlResolvedAssetVariant FromBaseAsset(WebGlAssetDefinition asset, string requestedProfile)
        => new()
        {
            AssetId = asset.Id,
            VariantId = string.Empty,
            Uri = asset.Uri,
            Format = asset.Format,
            PrimitiveKind = asset.PrimitiveKind,
            FallbackAssetId = asset.FallbackAssetId,
            QualityTier = string.IsNullOrWhiteSpace(asset.QualityTier) ? WebGlAssetQualityTiers.Unknown : asset.QualityTier,
            Color = asset.Color,
            Scale = WebGlVector3.One,
            PerformanceHint = asset.PerformanceHint,
            RequestedProfile = requestedProfile
        };

    public static WebGlResolvedAssetVariant FromVariant(
        WebGlAssetDefinition asset,
        WebGlAssetVariant variant,
        string requestedProfile)
        => new()
        {
            AssetId = asset.Id,
            VariantId = variant.Id,
            Uri = string.IsNullOrWhiteSpace(variant.Uri) ? asset.Uri : variant.Uri,
            Format = string.IsNullOrWhiteSpace(variant.Format) ? asset.Format : variant.Format,
            PrimitiveKind = string.IsNullOrWhiteSpace(variant.PrimitiveKind) ? asset.PrimitiveKind : variant.PrimitiveKind,
            FallbackAssetId = string.IsNullOrWhiteSpace(variant.FallbackAssetId) ? asset.FallbackAssetId : variant.FallbackAssetId,
            QualityTier = string.IsNullOrWhiteSpace(variant.QualityTier) ? asset.QualityTier : variant.QualityTier,
            Color = string.IsNullOrWhiteSpace(variant.Color) ? asset.Color : variant.Color,
            Scale = variant.Scale,
            PerformanceHint = variant.PerformanceHint,
            RequestedProfile = requestedProfile
        };
}
