using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static class WebGlSandboxAssetCatalogFactory
{
    private const string WebGlLibContentRoot = "_content/CanDoItAll.Components.WebGlLib";
    private const string ExternalModelRoot = "/assets/external-models";

    public static WebGlAssetCatalog Create()
    {
        var assets = new List<WebGlAssetDefinition>
        {
            Primitive("asset.primitive.fallback", "Fallback box", WebGlPrimitiveKinds.Box, "#94a3b8", new(1, 1, 1), ["fallback", "primitive"]),
            HouseAsset(),
            ServiceBuildingAsset(),
            Primitive("asset.prop.tree.default", "Generic tree", WebGlPrimitiveKinds.Tree, "#22c55e", new(1.2, 2.4, 1.2), ["prop", "tree", "fallback"]),
            PersonAsset(),
            MarkerAsset("asset.symbol.marker.default", "Question marker", "assets/model/question_box.glb", WebGlPrimitiveKinds.Marker, "#facc15", 405_612, ["symbol", "marker", "glb"]),
            MarkerAsset("asset.symbol.info.default", "Info marker", "assets/model/1gears.glb", WebGlPrimitiveKinds.Gear, "#38bdf8", 2_747_328, ["symbol", "info", "glb"]),
            Primitive("asset.symbol.warning.default", "Warning marker", WebGlPrimitiveKinds.Marker, "#f97316", new(0.72, 0.72, 0.72), ["symbol", "warning", "fallback"]),
            Primitive("asset.symbol.ready.default", "Ready marker", WebGlPrimitiveKinds.Marker, "#22c55e", new(0.72, 0.72, 0.72), ["symbol", "ready", "fallback"])
        };
        assets.AddRange(CreateExternalModelAssets());

        return new WebGlAssetCatalog
        {
            CatalogId = "webgl-sandbox-generic-assets",
            Version = "1.1",
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

    private static WebGlAssetDefinition PersonAsset()
        => new()
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

    private static WebGlAssetDefinition MarkerAsset(
        string id,
        string displayName,
        string relativeUri,
        string fallbackPrimitive,
        string color,
        long byteSize,
        IEnumerable<string> tags)
        => new()
        {
            Id = id,
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = Lib(relativeUri),
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = fallbackPrimitive,
            DisplayName = displayName,
            Color = color,
            BoundsHint = new(0.75, 0.75, 0.75),
            SupportsTint = true,
            QualityTier = WebGlAssetQualityTiers.ModelLow,
            PerformanceHint = Hint(byteSize, WebGlAssetQualityTiers.ModelLow, 32),
            Tags = [.. tags],
            License = "Repository asset",
            Source = "CanDoItAll.Components.WebGlLib/wwwroot/assets/model",
            Variants =
            [
                PrimitiveVariant($"{id}.primitive", $"{displayName} primitive", fallbackPrimitive, color),
                ModelVariant($"{id}.model-low", $"{displayName} model", Lib(relativeUri), WebGlAssetQualityTiers.ModelLow, fallbackPrimitive, byteSize, 32)
            ]
        };

    private static IEnumerable<WebGlAssetDefinition> CreateExternalModelAssets()
    {
        foreach (var model in WebGlSandboxExternalModels.All)
        {
            yield return ExternalModel(model.Id, model.DisplayName, model.RelativeUri, model.PrimitiveKind, model.Color, model.ByteSize, model.Category);
        }
    }

    private static WebGlAssetDefinition ExternalModel(
        string id,
        string displayName,
        string relativeUri,
        string primitiveKind,
        string color,
        long byteSize,
        string category)
        => new()
        {
            Id = id,
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = External(relativeUri),
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = primitiveKind,
            DisplayName = displayName,
            Color = color,
            BoundsHint = new(1.4, 1.4, 1.4),
            SupportsTint = true,
            QualityTier = WebGlAssetQualityTiers.ModelHigh,
            PerformanceHint = Hint(byteSize, WebGlAssetQualityTiers.ModelHigh, 4),
            Tags = ["external", category, "model-high"],
            License = "Repository asset",
            Source = "3DModels"
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
