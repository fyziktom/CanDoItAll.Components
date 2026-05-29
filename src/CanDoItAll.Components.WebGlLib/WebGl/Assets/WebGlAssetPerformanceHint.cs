namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetPerformanceHint
{
    public int TriangleCountHint { get; set; }

    public int VertexCountHint { get; set; }

    public long ByteSizeHint { get; set; }

    public string QualityTier { get; set; } = WebGlAssetQualityTiers.Unknown;

    public int RecommendedMaxInstanceCount { get; set; }

    public bool PreferInstancing { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlAssetQualityTiers
{
    public const string Unknown = "unknown";
    public const string Primitive = "primitive";
    public const string ModelLow = "model-low";
    public const string ModelMedium = "model-medium";
    public const string ModelHigh = "model-high";
}

public static class WebGlAssetQualityProfiles
{
    public const string Primitive = WebGlAssetQualityTiers.Primitive;
    public const string MixedGlb = WebGlAssetQualityTiers.ModelLow;
    public const string HighDetailGlb = WebGlAssetQualityTiers.ModelHigh;
}
