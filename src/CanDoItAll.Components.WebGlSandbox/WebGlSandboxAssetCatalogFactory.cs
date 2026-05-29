using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private const string WebGlLibContentRoot = "_content/CanDoItAll.Components.WebGlLib";
    private const string ExternalModelRoot = "/assets/external-models";

    public static WebGlAssetCatalog Create()
    {
        var assets = new List<WebGlAssetDefinition>();
        assets.AddRange(CreateCoreAssets());
        assets.AddRange(CreateBuildingAssets());
        assets.AddRange(CreatePersonAssets());
        assets.AddRange(CreatePropAssets());
        assets.AddRange(CreateExternalModelAssets());

        return new WebGlAssetCatalog
        {
            CatalogId = "webgl-sandbox-generic-assets",
            Version = "1.2",
            DefaultFallbackAssetId = "asset.primitive.fallback",
            Assets = assets,
            Metadata =
            {
                ["source"] = "Repository WebGlLib GLB inventory, external 3DModels inventory, and primitive fallbacks",
                ["profiles"] = "primitive; model-low; model-high",
                ["largeScreenOnly"] = "true"
            }
        };
    }

    private static WebGlAssetDefinition Primitive(
        string id,
        string displayName,
        string primitiveKind,
        string color,
        WebGlVector3 bounds,
        IEnumerable<string> tags)
        => new()
        {
            Id = id,
            Kind = WebGlAssetKinds.Primitive,
            Format = WebGlAssetFormats.Primitive,
            PrimitiveKind = primitiveKind,
            DisplayName = displayName,
            Color = color,
            BoundsHint = bounds,
            QualityTier = WebGlAssetQualityTiers.Primitive,
            PerformanceHint = Hint(0, WebGlAssetQualityTiers.Primitive, 100),
            SupportsTint = true,
            Tags = [.. tags],
            License = "Generated primitive",
            Source = "WebGlLib runtime primitive fallback"
        };

    private static WebGlAssetVariant ModelVariant(
        string id,
        string displayName,
        string uri,
        string qualityTier,
        string fallbackPrimitive,
        long byteSize,
        int maxInstances)
        => new()
        {
            Id = id,
            DisplayName = displayName,
            Uri = uri,
            QualityTier = qualityTier,
            Format = WebGlAssetFormats.Glb,
            PrimitiveKind = fallbackPrimitive,
            FallbackAssetId = "asset.primitive.fallback",
            ImportOptions = DefaultModelImportOptions(),
            PerformanceHint = Hint(byteSize, qualityTier, maxInstances)
        };

    private static WebGlAssetVariant PrimitiveVariant(string id, string displayName, string primitiveKind, string color)
        => new()
        {
            Id = id,
            DisplayName = displayName,
            QualityTier = WebGlAssetQualityTiers.Primitive,
            Format = WebGlAssetFormats.Primitive,
            PrimitiveKind = primitiveKind,
            Color = color,
            FallbackAssetId = "asset.primitive.fallback",
            PerformanceHint = Hint(0, WebGlAssetQualityTiers.Primitive, 100)
        };

    private static WebGlModelImportOptions DefaultModelImportOptions()
        => new()
        {
            FitMode = WebGlModelFitModes.FitBounds,
            CenterMode = WebGlModelCenterModes.CenterBottom,
            ForceDoubleSidedMaterial = true,
            NormalizeMaterialVisibility = true
        };

    private static WebGlAssetPerformanceHint Hint(long byteSize, string qualityTier, int recommendedMaxInstanceCount)
        => new()
        {
            ByteSizeHint = byteSize,
            QualityTier = qualityTier,
            RecommendedMaxInstanceCount = recommendedMaxInstanceCount
        };

    private static string Lib(string relativeUri) => $"{WebGlLibContentRoot}/{relativeUri}";

    private static string External(string relativeUri) => $"{ExternalModelRoot}/{relativeUri}";
}
