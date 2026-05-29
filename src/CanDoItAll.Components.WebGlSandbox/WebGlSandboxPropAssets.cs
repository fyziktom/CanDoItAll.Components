using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static partial class WebGlSandboxAssetCatalogFactory
{
    private static IEnumerable<WebGlAssetDefinition> CreatePropAssets()
    {
        yield return Primitive(
            "asset.prop.tree.default",
            "Generic tree",
            WebGlPrimitiveKinds.Tree,
            "#22c55e",
            new(1.2, 2.4, 1.2),
            ["prop", "tree", "fallback"]);
    }
}
