namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeOptions
{
    public bool DeterministicMode { get; set; } = true;

    public bool PreserveDrawingBuffer { get; set; } = true;

    public bool EnableAntialiasing { get; set; } = true;

    public double MaximumDevicePixelRatio { get; set; } = 2;

    public string RenderMode { get; set; } = WebGlRenderModes.Auto;

    public string AssetQualityProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public bool ShowDiagnosticsPanel { get; set; } = true;

    public bool ShowLabels { get; set; } = true;

    public bool ShowSymbols { get; set; } = true;

    public bool AutoFitOnCreate { get; set; } = true;

    public string RuntimeKey { get; set; } = string.Empty;
}
