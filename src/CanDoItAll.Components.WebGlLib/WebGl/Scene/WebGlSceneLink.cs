namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneLink
{
    public string Id { get; set; } = string.Empty;

    public string SourceObjectId { get; set; } = string.Empty;

    public string TargetObjectId { get; set; } = string.Empty;

    public string Kind { get; set; } = "link";

    public string Label { get; set; } = string.Empty;

    public string Color { get; set; } = "#94a3b8";

    public double Width { get; set; } = 1.0;

    public double Opacity { get; set; } = 0.75;

    public bool IsDirectional { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

