namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlActionBinding
{
    public string ActionKind { get; set; } = string.Empty;

    public string PoseKey { get; set; } = string.Empty;

    public string SymbolKey { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}
