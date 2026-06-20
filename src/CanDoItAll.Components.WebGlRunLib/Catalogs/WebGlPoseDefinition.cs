using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlPoseDefinition
{
    public string PoseKey { get; set; } = string.Empty;

    public string AssetVariantId { get; set; } = string.Empty;

    public string AssetId { get; set; } = string.Empty;

    public WebGlVector3? Rotation { get; set; }

    public WebGlVector3? Scale { get; set; }

    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;

    public string SymbolKey { get; set; } = string.Empty;

    public bool IsNoOpFallback { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}
