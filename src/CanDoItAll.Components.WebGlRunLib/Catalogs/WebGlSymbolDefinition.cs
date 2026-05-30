using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlSymbolDefinition
{
    public string SymbolKey { get; set; } = string.Empty;

    public string SemanticKind { get; set; } = string.Empty;

    public string SymbolAssetId { get; set; } = string.Empty;

    public string Color { get; set; } = "#facc15";

    public string EffectKey { get; set; } = WebGlSymbolEffects.Pulse;

    public string Tooltip { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}
