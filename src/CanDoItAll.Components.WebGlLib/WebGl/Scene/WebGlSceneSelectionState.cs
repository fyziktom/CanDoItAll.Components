namespace CanDoItAll.Components.WebGlLib;

public sealed partial class WebGlSceneSelectionState
{
    public List<string> SelectedObjectIds { get; set; } = [];

    public string PrimaryObjectId { get; set; } = string.Empty;

    public string ContextActionId { get; set; } = string.Empty;
}

