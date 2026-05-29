namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetCatalogValidator
{
    public WebGlAssetCatalogValidationResult Validate(WebGlAssetCatalog? catalog)
    {
        var result = new WebGlAssetCatalogValidationResult();
        if (catalog is null)
        {
            result.Errors.Add("Catalog is missing.");
            return result;
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var asset in catalog.Assets)
        {
            if (string.IsNullOrWhiteSpace(asset.Id))
            {
                result.Errors.Add("Asset id is required.");
                continue;
            }

            if (!ids.Add(asset.Id))
            {
                result.Errors.Add($"Duplicate asset id '{asset.Id}'.");
            }

            if (string.Equals(asset.Format, WebGlAssetFormats.Primitive, StringComparison.Ordinal) &&
                string.IsNullOrWhiteSpace(asset.PrimitiveKind))
            {
                result.Errors.Add($"Primitive asset '{asset.Id}' must declare a primitive kind.");
            }

            if ((string.Equals(asset.Format, WebGlAssetFormats.Glb, StringComparison.Ordinal) ||
                 string.Equals(asset.Format, WebGlAssetFormats.Gltf, StringComparison.Ordinal)) &&
                string.IsNullOrWhiteSpace(asset.Uri))
            {
                result.Warnings.Add($"Model asset '{asset.Id}' has no URI and will require a fallback.");
            }

            foreach (var variant in asset.Variants)
            {
                if (string.IsNullOrWhiteSpace(variant.Id))
                {
                    result.Errors.Add($"Asset '{asset.Id}' contains a variant without an id.");
                }

                if (string.Equals(variant.Format, WebGlAssetFormats.Primitive, StringComparison.Ordinal) &&
                    string.IsNullOrWhiteSpace(variant.PrimitiveKind))
                {
                    result.Errors.Add($"Primitive variant on '{asset.Id}' must declare a primitive kind.");
                }

                if ((string.Equals(variant.Format, WebGlAssetFormats.Glb, StringComparison.Ordinal) ||
                     string.Equals(variant.Format, WebGlAssetFormats.Gltf, StringComparison.Ordinal)) &&
                    string.IsNullOrWhiteSpace(variant.Uri))
                {
                    result.Warnings.Add($"Model variant '{variant.Id}' on '{asset.Id}' has no URI and will require a fallback.");
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(catalog.DefaultFallbackAssetId) && !ids.Contains(catalog.DefaultFallbackAssetId))
        {
            result.Warnings.Add($"Default fallback asset '{catalog.DefaultFallbackAssetId}' is not present in the catalog.");
        }

        foreach (var asset in catalog.Assets)
        {
            if (!string.IsNullOrWhiteSpace(asset.FallbackAssetId) && !ids.Contains(asset.FallbackAssetId))
            {
                result.Warnings.Add($"Fallback asset '{asset.FallbackAssetId}' for '{asset.Id}' is not present in the catalog.");
            }

            foreach (var variant in asset.Variants.Where(variant => !string.IsNullOrWhiteSpace(variant.FallbackAssetId) && !ids.Contains(variant.FallbackAssetId)))
            {
                result.Warnings.Add($"Fallback asset '{variant.FallbackAssetId}' for variant '{variant.Id}' on '{asset.Id}' is not present in the catalog.");
            }
        }

        return result;
    }
}

public sealed class WebGlAssetCatalogValidationResult
{
    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}
