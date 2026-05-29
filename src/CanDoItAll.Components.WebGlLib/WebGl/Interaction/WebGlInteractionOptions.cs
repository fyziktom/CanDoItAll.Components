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

    public string DragAxisLock { get; set; } = string.Empty;

    public double DragSnapGridSize { get; set; }

    public WebGlDragBounds? DragBounds { get; set; }
}

public sealed class WebGlDragBounds
{
    public double MinX { get; set; } = double.NegativeInfinity;

    public double MaxX { get; set; } = double.PositiveInfinity;

    public double MinZ { get; set; } = double.NegativeInfinity;

    public double MaxZ { get; set; } = double.PositiveInfinity;
}

