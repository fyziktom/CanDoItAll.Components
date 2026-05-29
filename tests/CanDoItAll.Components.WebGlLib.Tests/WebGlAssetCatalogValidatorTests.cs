using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlAssetCatalogValidatorTests
{
    [Fact]
    public void Validate_reports_duplicate_asset_ids_and_missing_primitive_kind()
    {
        var catalog = new WebGlAssetCatalog
        {
            DefaultFallbackAssetId = "asset.missing",
            Assets =
            [
                new WebGlAssetDefinition
                {
                    Id = "asset.primitive",
                    Format = WebGlAssetFormats.Primitive,
                    PrimitiveKind = string.Empty
                },
                new WebGlAssetDefinition
                {
                    Id = "asset.primitive",
                    Format = WebGlAssetFormats.Glb
                }
            ]
        };

        var result = new WebGlAssetCatalogValidator().Validate(catalog);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("Duplicate asset id", StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.Contains("must declare a primitive kind", StringComparison.Ordinal));
        Assert.Contains(result.Warnings, warning => warning.Contains("Default fallback asset", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_checks_variant_contracts()
    {
        var catalog = new WebGlAssetCatalog
        {
            Assets =
            [
                new WebGlAssetDefinition
                {
                    Id = "asset.model",
                    Format = WebGlAssetFormats.Glb,
                    Uri = "model.glb",
                    Variants =
                    [
                        new WebGlAssetVariant
                        {
                            Format = WebGlAssetFormats.Primitive,
                            PrimitiveKind = string.Empty
                        },
                        new WebGlAssetVariant
                        {
                            Id = "variant.model",
                            Format = WebGlAssetFormats.Glb,
                            Uri = string.Empty
                        }
                    ]
                }
            ]
        };

        var result = new WebGlAssetCatalogValidator().Validate(catalog);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("variant without an id", StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.Contains("Primitive variant", StringComparison.Ordinal));
        Assert.Contains(result.Warnings, warning => warning.Contains("Model variant", StringComparison.Ordinal));
    }
}
