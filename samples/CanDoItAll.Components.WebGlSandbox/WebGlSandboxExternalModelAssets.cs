using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private static IEnumerable<WebGlAssetDefinition> CreateExternalModelAssets()
    {
        foreach (var model in WebGlSandboxExternalModels.All)
        {
            yield return new WebGlAssetDefinition
            {
                Id = model.Id,
                Kind = WebGlAssetKinds.Model,
                Format = WebGlAssetFormats.Glb,
                Uri = External(model.RelativeUri),
                FallbackAssetId = "asset.primitive.fallback",
                PrimitiveKind = model.PrimitiveKind,
                DisplayName = model.DisplayName,
                Color = model.Color,
                BoundsHint = new(1.4, 1.4, 1.4),
                SupportsTint = true,
                QualityTier = WebGlAssetQualityTiers.ModelHigh,
                ImportOptions = DefaultModelImportOptions(),
                PerformanceHint = Hint(model.ByteSize, WebGlAssetQualityTiers.ModelHigh, 4),
                Tags = ["external", model.Category, "model-high"],
                License = "Repository asset",
                Source = "3DModels"
            };
        }
    }
}
