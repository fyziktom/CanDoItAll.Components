namespace CanDoItAll.Components.WebGlLib;

public sealed record WebGlSceneHoverChangedEventArgs(
    string? ObjectId,
    double ScreenX,
    double ScreenY);

