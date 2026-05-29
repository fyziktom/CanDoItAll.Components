# WebGl asset models skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public static class WebGlAssetKinds
{
    public const string Model = "model";
    public const string Primitive = "primitive";
    public const string Symbol = "symbol";
    public const string Material = "material";
}

public static class WebGlAssetFormats
{
    public const string Glb = "glb";
    public const string Gltf = "gltf";
    public const string Primitive = "primitive";
}

public sealed class WebGlAssetCatalog
{
    public string CatalogId { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0";

    public List<WebGlAssetDefinition> Assets { get; set; } = [];

    public string DefaultFallbackAssetId { get; set; } = string.Empty;
}

public sealed class WebGlAssetDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Kind { get; set; } = WebGlAssetKinds.Model;

    public string Format { get; set; } = WebGlAssetFormats.Glb;

    public string Uri { get; set; } = string.Empty;

    public string FallbackAssetId { get; set; } = string.Empty;

    public string PrimitiveKind { get; set; } = "box";

    public string DisplayName { get; set; } = string.Empty;

    public string Color { get; set; } = "#ffffff";

    public double DefaultScale { get; set; } = 1.0;

    public bool SupportsTint { get; set; } = true;

    public bool SupportsBillboard { get; set; }

    public WebGlVector3 BoundsHint { get; set; } = new(1, 1, 1);

    public List<WebGlAssetVariant> Variants { get; set; } = [];

    public List<WebGlAssetAnimation> Animations { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public string License { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlAssetVariant
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}

public sealed class WebGlAssetAnimation
{
    public string ClipName { get; set; } = string.Empty;

    public string SemanticKey { get; set; } = string.Empty;

    public bool Loop { get; set; } = true;

    public double Speed { get; set; } = 1.0;
}

public interface IWebGlAssetCatalogProvider
{
    ValueTask<WebGlAssetCatalog> GetCatalogAsync(CancellationToken cancellationToken = default);
}
```
