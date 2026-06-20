namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetCatalog
{
    public string CatalogId { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0";

    public string DefaultFallbackAssetId { get; set; } = string.Empty;

    public List<WebGlAssetDefinition> Assets { get; set; } = [];

    public List<WebGlModelImportRecipe> ModelImportRecipes { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

