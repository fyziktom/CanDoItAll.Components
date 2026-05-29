namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Kind { get; set; } = WebGlAssetKinds.Model;

    public string Format { get; set; } = WebGlAssetFormats.Glb;

    public string Uri { get; set; } = string.Empty;

    public string FallbackAssetId { get; set; } = string.Empty;

    public string PrimitiveKind { get; set; } = WebGlPrimitiveKinds.Box;

    public string DisplayName { get; set; } = string.Empty;

    public string ThumbnailUri { get; set; } = string.Empty;

    public string Color { get; set; } = "#ffffff";

    public double DefaultScale { get; set; } = 1.0;

    public bool SupportsTint { get; set; } = true;

    public bool SupportsBillboard { get; set; }

    public WebGlVector3 BoundsHint { get; set; } = new(1, 1, 1);

    public double LodDistanceHint { get; set; }

    public List<WebGlAssetVariant> Variants { get; set; } = [];

    public List<WebGlAssetAnimation> Animations { get; set; } = [];

    public List<WebGlAssetMaterialOverride> MaterialOverrides { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public string License { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

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

public static class WebGlPrimitiveKinds
{
    public const string Box = "box";
    public const string House = "house";
    public const string Sphere = "sphere";
    public const string Cylinder = "cylinder";
    public const string Cone = "cone";
    public const string Tree = "tree";
    public const string Person = "person";
    public const string Marker = "marker";
    public const string Gear = "gear";
}

