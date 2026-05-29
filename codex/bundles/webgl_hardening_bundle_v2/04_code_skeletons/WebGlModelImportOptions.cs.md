# WebGlModelImportOptions.cs skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlModelImportOptions
{
    public double UnitScale { get; set; } = 1.0;

    public string FitMode { get; set; } = WebGlModelFitModes.FitBounds;

    public string CenterMode { get; set; } = WebGlModelCenterModes.CenterBottom;

    public WebGlVector3 RotationOffset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 PositionOffset { get; set; } = WebGlVector3.Zero;

    public bool ForceDoubleSidedMaterials { get; set; }

    public bool NormalizeMaterialVisibility { get; set; }

    public bool DisableTint { get; set; }

    public bool ShowDebugBounds { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlModelFitModes
{
    public const string FitBounds = "fit-bounds";
    public const string OriginalScale = "original-scale";
    public const string FixedScale = "fixed-scale";
}

public static class WebGlModelCenterModes
{
    public const string CenterBottom = "center-bottom";
    public const string CenterBounds = "center-bounds";
    public const string PreserveOrigin = "preserve-origin";
}
```

Add `ImportOptions` to both `WebGlAssetDefinition` and `WebGlAssetVariant`. Variant options should override base asset options.
