namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlCameraCommand
{
    public string CommandId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string ObjectId { get; set; } = string.Empty;

    public WebGlSceneCamera Camera { get; set; } = new();
}

