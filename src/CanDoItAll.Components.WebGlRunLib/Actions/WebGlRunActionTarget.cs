using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionTarget
{
    public string ObjectId { get; set; } = string.Empty;

    public string AnchorKey { get; set; } = WebGlRunAnchorKeys.Center;

    public string FallbackAnchorKey { get; set; } = WebGlRunAnchorKeys.Center;

    public WebGlVector3 Offset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3? Position { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}
