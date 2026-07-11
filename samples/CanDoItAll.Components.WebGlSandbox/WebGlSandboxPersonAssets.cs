using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private static IEnumerable<WebGlAssetDefinition> CreatePersonAssets()
    {
        yield return new WebGlAssetDefinition
        {
            Id = "asset.agent.person.default",
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = Lib("assets/model/lowpoly_person_boxing.glb"),
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = WebGlPrimitiveKinds.Person,
            DisplayName = "Generic person",
            Color = "#e2e8f0",
            BoundsHint = new(0.8, 1.8, 0.8),
            SupportsTint = true,
            QualityTier = WebGlAssetQualityTiers.ModelLow,
            ImportOptions = DefaultModelImportOptions(),
            PerformanceHint = Hint(363_328, WebGlAssetQualityTiers.ModelLow, 24),
            Tags = ["agent", "person", "variant"],
            License = "Repository asset",
            Source = "WebGlLib and 3DModels",
            Variants =
            [
                PrimitiveVariant("variant.person.primitive", "Primitive person", WebGlPrimitiveKinds.Person, "#e2e8f0"),
                ModelVariant("variant.person.boxer.low", "Low poly person", Lib("assets/model/lowpoly_person_boxing.glb"), WebGlAssetQualityTiers.ModelLow, WebGlPrimitiveKinds.Person, 363_328, 24),
                ModelVariant("variant.person.male-running.high", "Male running", External("glb/people/Male_Running.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.Person, 69_440, 18),
                ModelVariant("variant.person.female-running.high", "Female running", External("glb/people/Female_Running.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.Person, 99_932, 18),
                ModelVariant("variant.person.male-standing.high", "Male standing", External("glb/people/Male_Standing.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.Person, 69_452, 18),
                ModelVariant("variant.person.female-standing.high", "Female standing", External("glb/people/Female_Standing.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.Person, 101_508, 18)
            ]
        };
    }
}
