using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private static IEnumerable<WebGlAssetDefinition> CreateCoreAssets()
    {
        yield return Primitive(
            "asset.primitive.fallback",
            "Fallback box",
            WebGlPrimitiveKinds.Box,
            "#94a3b8",
            new(1, 1, 1),
            ["fallback", "primitive"]);

        yield return MarkerAsset(
            "asset.symbol.marker.default",
            "Question marker",
            "assets/model/question_box.glb",
            WebGlPrimitiveKinds.Marker,
            "#facc15",
            405_612,
            ["symbol", "marker", "glb"]);

        yield return MarkerAsset(
            "asset.symbol.info.default",
            "Info marker",
            "assets/model/1gears.glb",
            WebGlPrimitiveKinds.Gear,
            "#38bdf8",
            2_747_328,
            ["symbol", "info", "glb"]);

        yield return Primitive(
            "asset.symbol.warning.default",
            "Warning marker",
            WebGlPrimitiveKinds.Marker,
            "#f97316",
            new(0.72, 0.72, 0.72),
            ["symbol", "warning", "fallback"]);

        yield return Primitive(
            "asset.symbol.ready.default",
            "Ready marker",
            WebGlPrimitiveKinds.Marker,
            "#22c55e",
            new(0.72, 0.72, 0.72),
            ["symbol", "ready", "fallback"]);
    }

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
            ImportOptions = DefaultModelImportOptions(),
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
}
