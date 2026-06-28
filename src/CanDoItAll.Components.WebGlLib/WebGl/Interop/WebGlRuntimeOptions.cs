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

    public string LabelVisibilityMode { get; set; } = WebGlLabelVisibilityModes.Always;

    public int LabelHoverHideDelayMilliseconds { get; set; } = 2_200;

    public bool ShowSymbols { get; set; } = true;

    public bool ShowLinks { get; set; } = true;

    public bool AutoFitOnCreate { get; set; } = true;

    public int MaxCommandResultHistory { get; set; } = 100;

    public int MaxCommandBatchChildResults { get; set; } = 5;

    public int MaxCommandBatchMessages { get; set; } = 5;

    public int MaxCommandBatchProofSnapshotPositions { get; set; } = 10;

    public bool NotifyStateChanged { get; set; } = true;

    public bool NotifyMotionCompleted { get; set; } = true;

    public bool NotifyCommandCompleted { get; set; } = true;

    public bool NotifyCommandFailed { get; set; } = true;

    public WebGlRuntimeBudgetOptions RuntimeBudget { get; set; } = new();

    public string RuntimeKey { get; set; } = string.Empty;
}

public static class WebGlLabelVisibilityModes
{
    public const string Always = "always";

    public const string Hover = "hover";
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
