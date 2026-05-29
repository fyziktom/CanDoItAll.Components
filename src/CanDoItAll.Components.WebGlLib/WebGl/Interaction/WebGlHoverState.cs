namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlHoverState
{
    public string ObjectId { get; set; } = string.Empty;

    public WebGlVector3 WorldPosition { get; set; } = WebGlVector3.Zero;

    public double ScreenX { get; set; }

    public double ScreenY { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

