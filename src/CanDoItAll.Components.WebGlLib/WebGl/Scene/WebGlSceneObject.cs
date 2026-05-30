namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneObject
{
    public string Id { get; set; } = string.Empty;

    public string Kind { get; set; } = "object";

    public string Family { get; set; } = "generic";

    public string Title { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string AssetId { get; set; } = string.Empty;

    public WebGlVector3 Position { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 Rotation { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 Scale { get; set; } = WebGlVector3.One;

    public WebGlVector3 Size { get; set; } = new(1, 1, 1);

    public string Tone { get; set; } = "neutral";

    public string Color { get; set; } = "#ffffff";

    public bool IsSelectable { get; set; } = true;

    public bool IsDraggable { get; set; }

    public List<WebGlStatusSymbol> Symbols { get; set; } = [];

    public List<WebGlSceneObjectAnchor> Anchors { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneObjectAnchor
{
    public string Key { get; set; } = string.Empty;

    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3? Position { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

