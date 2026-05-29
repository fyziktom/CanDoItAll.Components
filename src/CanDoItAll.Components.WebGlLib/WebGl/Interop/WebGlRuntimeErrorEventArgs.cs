namespace CanDoItAll.Components.WebGlLib;

public sealed record WebGlRuntimeErrorEventArgs(
    string SceneId,
    string Message,
    string Detail);

