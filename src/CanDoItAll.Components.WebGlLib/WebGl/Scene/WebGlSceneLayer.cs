namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneLayer
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Kind { get; set; } = "layer";

    public bool IsVisible { get; set; } = true;

    public int SortOrder { get; set; }

    public List<string> ObjectIds { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

