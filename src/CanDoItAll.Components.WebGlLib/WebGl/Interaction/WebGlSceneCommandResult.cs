namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneCommandResult
{
    public string CommandId { get; set; } = string.Empty;

    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public WebGlSceneProofSnapshot Snapshot { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}

