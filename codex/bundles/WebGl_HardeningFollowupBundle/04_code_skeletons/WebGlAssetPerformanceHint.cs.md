# WebGlAssetPerformanceHint.cs skeleton

```csharp
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
    public const string Low = "low";
    public const string Medium = "medium";
    public const string High = "high";
}
```
