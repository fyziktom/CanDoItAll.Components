namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlInteractionOptions
{
    public string SelectionMode { get; set; } = WebGlSelectionModes.Single;

    public bool AllowHover { get; set; } = true;

    public bool AllowClickSelection { get; set; } = true;

    public bool AllowMultiSelect { get; set; }

    public bool AllowDragOnGroundPlane { get; set; }

    public bool AllowCameraPan { get; set; } = true;

    public bool AllowCameraZoom { get; set; } = true;

    public bool AllowCameraOrbit { get; set; } = true;

    public bool FitViewOnCreate { get; set; } = true;

    public bool FocusOnDoubleClick { get; set; } = true;
}

