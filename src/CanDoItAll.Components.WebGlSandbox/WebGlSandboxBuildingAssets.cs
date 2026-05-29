using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private static IEnumerable<WebGlAssetDefinition> CreateBuildingAssets()
    {
        yield return HouseAsset();
        yield return ServiceBuildingAsset();
    }

    private static WebGlAssetDefinition HouseAsset()
        => new()
        {
            Id = "asset.building.house.default",
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = External("glb/buildings/House_1.glb"),
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = WebGlPrimitiveKinds.House,
            DisplayName = "Generic house",
            Color = "#60a5fa",
            BoundsHint = new(2.4, 2.1, 2.2),
            SupportsTint = true,
            DefaultScale = 1,
            QualityTier = WebGlAssetQualityTiers.ModelLow,
            ImportOptions = DefaultModelImportOptions(),
            PerformanceHint = Hint(391_772, WebGlAssetQualityTiers.ModelLow, 18),
            Tags = ["building", "house", "variant"],
            License = "Repository asset",
            Source = "3DModels/glb/buildings",
            Variants =
            [
                PrimitiveVariant("variant.house.primitive", "Primitive house", WebGlPrimitiveKinds.House, "#60a5fa"),
                ModelVariant("variant.house.house-1.low", "House 1 low model", External("glb/buildings/House_1.glb"), WebGlAssetQualityTiers.ModelLow, WebGlPrimitiveKinds.House, 391_772, 18),
                ModelVariant("variant.house.house-2.high", "House 2 high model", External("glb/buildings/House_2.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.House, 483_576, 8)
            ]
        };

    private static WebGlAssetDefinition ServiceBuildingAsset()
        => new()
        {
            Id = "asset.building.service.default",
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = Lib("assets/model/gears.glb"),
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = WebGlPrimitiveKinds.Gear,
            DisplayName = "Generic service building",
            Color = "#f59e0b",
            BoundsHint = new(2.4, 2.4, 2.4),
            SupportsTint = true,
            DefaultScale = 1,
            QualityTier = WebGlAssetQualityTiers.ModelLow,
            ImportOptions = DefaultModelImportOptions(),
            PerformanceHint = Hint(480_552, WebGlAssetQualityTiers.ModelLow, 12),
            Tags = ["building", "service", "variant"],
            License = "Repository asset",
            Source = "WebGlLib and 3DModels",
            Variants =
            [
                PrimitiveVariant("variant.service.primitive", "Primitive service marker", WebGlPrimitiveKinds.Gear, "#f59e0b"),
                ModelVariant("variant.service.gears.low", "Gears low model", Lib("assets/model/gears.glb"), WebGlAssetQualityTiers.ModelLow, WebGlPrimitiveKinds.Gear, 480_552, 12),
                ModelVariant("variant.service.blacksmith.high", "Blacksmith high model", External("glb/buildings/Blacksmith.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.House, 618_248, 4),
                ModelVariant("variant.service.inn.high", "Inn high model", External("glb/buildings/Inn.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.House, 466_280, 5),
                ModelVariant("variant.service.mill.high", "Mill high model", External("glb/buildings/Mill.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.House, 518_868, 4),
                ModelVariant("variant.service.stable.high", "Stable high model", External("glb/buildings/Stable.glb"), WebGlAssetQualityTiers.ModelHigh, WebGlPrimitiveKinds.House, 409_288, 6)
            ]
        };
}
