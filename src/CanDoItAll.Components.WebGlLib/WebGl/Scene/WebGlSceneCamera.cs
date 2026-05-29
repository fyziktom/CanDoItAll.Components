namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneCamera
{
    public string ProjectionMode { get; set; } = WebGlSceneProjectionModes.Perspective;

    public string ViewMode { get; set; } = WebGlSceneViewModes.Isometric;

    public WebGlVector3 Target { get; set; } = WebGlVector3.Zero;

    public double Distance { get; set; } = 32;

    public double Zoom { get; set; } = 1;

    public double Azimuth { get; set; } = -0.78;

    public double Polar { get; set; } = 1.02;

    public double FieldOfView { get; set; } = 48;
}

public static class WebGlSceneProjectionModes
{
    public const string Perspective = "perspective";
    public const string Orthographic = "orthographic";
}

public static class WebGlSceneViewModes
{
    public const string Isometric = "isometric";
    public const string Perspective = "perspective";
    public const string Top = "top";
    public const string Front = "front";
}

