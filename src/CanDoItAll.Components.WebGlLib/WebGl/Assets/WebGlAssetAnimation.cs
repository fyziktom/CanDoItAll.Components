namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetAnimation
{
    public string ClipName { get; set; } = string.Empty;

    public string SemanticKey { get; set; } = string.Empty;

    public bool Loop { get; set; } = true;

    public double Speed { get; set; } = 1.0;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

