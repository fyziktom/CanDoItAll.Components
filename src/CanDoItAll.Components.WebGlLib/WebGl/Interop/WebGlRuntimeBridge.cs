namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeBridge
{
    public string Namespace { get; set; } = "CanDoItAll.webglScene";

    public WebGlRuntimeOptions Options { get; set; } = new();

    public WebGlRuntimeDiagnostics Diagnostics { get; set; } = new();
}

