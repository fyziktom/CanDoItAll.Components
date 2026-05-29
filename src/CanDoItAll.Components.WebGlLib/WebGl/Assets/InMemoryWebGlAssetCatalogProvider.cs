namespace CanDoItAll.Components.WebGlLib;

public sealed class InMemoryWebGlAssetCatalogProvider(WebGlAssetCatalog catalog) : IWebGlAssetCatalogProvider
{
    private readonly WebGlAssetCatalog catalog = catalog ?? new WebGlAssetCatalog();

    public ValueTask<WebGlAssetCatalog> GetCatalogAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(catalog);
}

