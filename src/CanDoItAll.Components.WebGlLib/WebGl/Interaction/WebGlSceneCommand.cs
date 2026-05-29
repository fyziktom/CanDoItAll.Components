namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneCommand
{
    public string CommandId { get; set; } = string.Empty;

    public string CommandType { get; set; } = string.Empty;

    public string ObjectId { get; set; } = string.Empty;

    public WebGlVector3 Position { get; set; } = WebGlVector3.Zero;

    public Dictionary<string, string> Parameters { get; set; } = [];
}

