using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static class WebGlSandboxAssetCatalogFactory
{
    private const string WebGlLibContentRoot = "_content/CanDoItAll.Components.WebGlLib";

    public static WebGlAssetCatalog Create()
    {
        return new WebGlAssetCatalog
        {
            CatalogId = "webgl-sandbox-generic-assets",
            Version = "1.0",
            DefaultFallbackAssetId = "asset.primitive.fallback",
            Assets =
            [
                Primitive("asset.primitive.fallback", "Fallback box", WebGlPrimitiveKinds.Box, "#94a3b8", new(1, 1, 1), ["fallback", "primitive"]),
                Primitive("asset.building.house.default", "Generic house", WebGlPrimitiveKinds.House, "#60a5fa", new(2.4, 2.1, 2.2), ["building", "house", "fallback"]),
                Model("asset.building.service.default", "Generic service building", "assets/model/gears.glb", WebGlPrimitiveKinds.Gear, "#f59e0b", new(2.4, 2.4, 2.4), ["building", "service", "glb"]),
                Primitive("asset.prop.tree.default", "Generic tree", WebGlPrimitiveKinds.Tree, "#22c55e", new(1.2, 2.4, 1.2), ["prop", "tree", "fallback"]),
                Model("asset.agent.person.default", "Generic person", "assets/model/lowpoly_person_boxing.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", new(0.8, 1.8, 0.8), ["agent", "person", "glb"]),
                Model("asset.symbol.marker.default", "Question marker", "assets/model/question_box.glb", WebGlPrimitiveKinds.Marker, "#facc15", new(0.75, 0.75, 0.75), ["symbol", "marker", "glb"]),
                Model("asset.symbol.info.default", "Info marker", "assets/model/1gears.glb", WebGlPrimitiveKinds.Gear, "#38bdf8", new(0.72, 0.72, 0.72), ["symbol", "info", "glb"]),
                Primitive("asset.symbol.warning.default", "Warning marker", WebGlPrimitiveKinds.Marker, "#f97316", new(0.72, 0.72, 0.72), ["symbol", "warning", "fallback"]),
                Primitive("asset.symbol.ready.default", "Ready marker", WebGlPrimitiveKinds.Marker, "#22c55e", new(0.72, 0.72, 0.72), ["symbol", "ready", "fallback"])
            ],
            Metadata =
            {
                ["source"] = "Repository WebGlLib GLB inventory plus primitive fallbacks",
                ["glbInventory"] = "1gears.glb; gears.glb; lowpoly_person_boxing.glb; question_box.glb"
            }
        };
    }

    private static WebGlAssetDefinition Model(
        string id,
        string displayName,
        string relativeUri,
        string fallbackPrimitive,
        string color,
        WebGlVector3 bounds,
        IEnumerable<string> tags)
    {
        return new WebGlAssetDefinition
        {
            Id = id,
            Kind = WebGlAssetKinds.Model,
            Format = WebGlAssetFormats.Glb,
            Uri = $"{WebGlLibContentRoot}/{relativeUri}",
            FallbackAssetId = "asset.primitive.fallback",
            PrimitiveKind = fallbackPrimitive,
            DisplayName = displayName,
            Color = color,
            BoundsHint = bounds,
            SupportsTint = true,
            DefaultScale = 1,
            Tags = [.. tags],
            License = "Repository asset",
            Source = "CanDoItAll.Components.WebGlLib/wwwroot/assets/model"
        };
    }

    private static WebGlAssetDefinition Primitive(
        string id,
        string displayName,
        string primitiveKind,
        string color,
        WebGlVector3 bounds,
        IEnumerable<string> tags)
    {
        return new WebGlAssetDefinition
        {
            Id = id,
            Kind = WebGlAssetKinds.Primitive,
            Format = WebGlAssetFormats.Primitive,
            PrimitiveKind = primitiveKind,
            DisplayName = displayName,
            Color = color,
            BoundsHint = bounds,
            SupportsTint = true,
            Tags = [.. tags],
            License = "Generated primitive",
            Source = "WebGlLib runtime primitive fallback"
        };
    }
}

