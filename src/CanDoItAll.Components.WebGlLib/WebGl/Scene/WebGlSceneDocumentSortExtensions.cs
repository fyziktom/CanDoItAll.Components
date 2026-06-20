namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlSceneDocumentSortExtensions
{
    public static void AssetsByIdSort(this WebGlSceneModel scene)
    {
        scene.AssetCatalog.Assets = scene.AssetCatalog.Assets
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Objects = scene.Objects
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Links = scene.Links
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
        scene.Layers = scene.Layers
            .OrderBy(static item => item.Id, StringComparer.Ordinal)
            .ToList();
    }
}
