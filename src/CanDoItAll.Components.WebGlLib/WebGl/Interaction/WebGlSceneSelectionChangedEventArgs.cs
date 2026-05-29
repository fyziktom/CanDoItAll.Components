namespace CanDoItAll.Components.WebGlLib;

public sealed record WebGlSceneSelectionChangedEventArgs(
    string? PrimaryObjectId,
    IReadOnlyList<string> SelectedObjectIds);

