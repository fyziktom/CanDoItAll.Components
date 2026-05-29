# WebGlModelDiagnostics.cs skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlModelDiagnostics
{
    public string AssetId { get; set; } = string.Empty;

    public string VariantId { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public bool Loaded { get; set; }

    public int MeshCount { get; set; }

    public int VisibleMeshCount { get; set; }

    public int TransparentMaterialCount { get; set; }

    public WebGlVector3 BoundsMin { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 BoundsMax { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 BoundsSize { get; set; } = WebGlVector3.Zero;

    public List<string> Warnings { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public bool HasVisibilityRisk => Errors.Count > 0 || Warnings.Count > 0;
}
```

The JS diagnostics object should have the same JSON shape.
