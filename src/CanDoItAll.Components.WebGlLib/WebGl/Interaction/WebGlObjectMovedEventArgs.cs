namespace CanDoItAll.Components.WebGlLib;

public sealed record WebGlSceneObjectPositionChange(
    string ObjectId,
    WebGlVector3 Position);

public sealed record WebGlObjectMovedEventArgs(
    IReadOnlyList<WebGlSceneObjectPositionChange> Positions);

