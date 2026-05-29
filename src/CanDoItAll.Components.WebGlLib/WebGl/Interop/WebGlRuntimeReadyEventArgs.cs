namespace CanDoItAll.Components.WebGlLib;

public sealed record WebGlRuntimeReadyEventArgs(
    string SceneId,
    WebGlRuntimeDiagnostics Diagnostics);

