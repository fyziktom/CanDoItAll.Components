using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlAssetVariantResolverTests
{
    [Fact]
    public void Resolve_prefers_explicit_variant_from_object_metadata()
    {
        var resolver = new WebGlAssetVariantResolver();
        var asset = AssetWithVariants();
        var sceneObject = new WebGlSceneObject
        {
            Metadata = { ["assetVariantId"] = "variant.high" }
        };

        var resolved = resolver.Resolve(asset, sceneObject, WebGlAssetQualityProfiles.MixedGlb);

        Assert.Equal("variant.high", resolved.VariantId);
        Assert.Equal(WebGlAssetQualityTiers.ModelHigh, resolved.QualityTier);
        Assert.Equal("/models/high.glb", resolved.Uri);
    }

    [Fact]
    public void Resolve_uses_profile_variant_when_no_explicit_variant_exists()
    {
        var resolved = new WebGlAssetVariantResolver().Resolve(
            AssetWithVariants(),
            new WebGlSceneObject(),
            WebGlAssetQualityProfiles.Primitive);

        Assert.Equal("variant.primitive", resolved.VariantId);
        Assert.Equal(WebGlAssetFormats.Primitive, resolved.Format);
    }

    private static WebGlAssetDefinition AssetWithVariants()
        => new()
        {
            Id = "asset.test",
            Format = WebGlAssetFormats.Glb,
            Uri = "/models/default.glb",
            PrimitiveKind = WebGlPrimitiveKinds.Box,
            Variants =
            [
                new WebGlAssetVariant
                {
                    Id = "variant.primitive",
                    QualityTier = WebGlAssetQualityTiers.Primitive,
                    Format = WebGlAssetFormats.Primitive,
                    PrimitiveKind = WebGlPrimitiveKinds.Box
                },
                new WebGlAssetVariant
                {
                    Id = "variant.low",
                    QualityTier = WebGlAssetQualityTiers.ModelLow,
                    Format = WebGlAssetFormats.Glb,
                    Uri = "/models/low.glb"
                },
                new WebGlAssetVariant
                {
                    Id = "variant.high",
                    QualityTier = WebGlAssetQualityTiers.ModelHigh,
                    Format = WebGlAssetFormats.Glb,
                    Uri = "/models/high.glb"
                }
            ]
        };
}
