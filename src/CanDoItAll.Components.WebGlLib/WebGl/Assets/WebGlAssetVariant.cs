namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetVariant
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public WebGlVector3 Scale { get; set; } = WebGlVector3.One;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

