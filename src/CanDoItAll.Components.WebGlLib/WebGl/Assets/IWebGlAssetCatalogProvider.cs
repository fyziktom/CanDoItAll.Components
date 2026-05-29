namespace CanDoItAll.Components.WebGlLib;

public interface IWebGlAssetCatalogProvider
{
    ValueTask<WebGlAssetCatalog> GetCatalogAsync(CancellationToken cancellationToken = default);
}

