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

    public int MaxCommandResultHistory { get; set; } = 100;

    public WebGlRuntimeBudgetOptions RuntimeBudget { get; set; } = new();

    public string RuntimeKey { get; set; } = string.Empty;
}

public sealed class WebGlRuntimeBudgetOptions
{
    public string Profile { get; set; } = "standard";

    public int MaxSceneObjects { get; set; } = 500;

    public int MaxLoadedAssets { get; set; } = 128;

    public int MaxAssetCacheEntries { get; set; } = 128;

    public int MaxActiveMotions { get; set; } = 256;

    public int MaxQueuedMotions { get; set; } = 512;

    public int MaxQueuedCommandStages { get; set; } = 128;

    public int MaxEstimatedTriangles { get; set; } = 250_000;

    public bool DegradeWhenExceeded { get; set; } = true;
}
