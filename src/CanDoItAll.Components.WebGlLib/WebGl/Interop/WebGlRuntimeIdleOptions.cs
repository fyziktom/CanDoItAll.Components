namespace CanDoItAll.Components.WebGlLib;

public static class WebGlRuntimeIdlePolicyModes
{
    public const string SemanticOnly = "semanticOnly";

    public const string VisualStrict = "visualStrict";

    public const string AllowFinalRenderDrain = "allowFinalRenderDrain";
}

public sealed class WebGlRuntimeIdleOptions
{
    public int TimeoutMs { get; set; } = 2_000;

    public int PollIntervalMs { get; set; } = 16;

    public string Reason { get; set; } = "runtime-idle";

    public string PolicyMode { get; set; } = WebGlRuntimeIdlePolicyModes.AllowFinalRenderDrain;
}
