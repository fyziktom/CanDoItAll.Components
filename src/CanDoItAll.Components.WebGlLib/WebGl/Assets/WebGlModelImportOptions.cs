namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlModelImportOptions
{
    public double UnitScale { get; set; } = 1;

    public string FitMode { get; set; } = WebGlModelFitModes.FitBounds;

    public string CenterMode { get; set; } = WebGlModelCenterModes.CenterBottom;

    public double FixedScale { get; set; } = 1;

    public WebGlVector3 RotationOffset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 PositionOffset { get; set; } = WebGlVector3.Zero;

    public bool ForceDoubleSidedMaterial { get; set; }

    public bool NormalizeMaterialVisibility { get; set; }

    public bool DebugBounds { get; set; }

    public bool DisableTint { get; set; }
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
