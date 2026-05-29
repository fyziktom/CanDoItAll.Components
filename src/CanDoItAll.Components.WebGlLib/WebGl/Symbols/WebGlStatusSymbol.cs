namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlStatusSymbol
{
    public string Id { get; set; } = string.Empty;

    public string SymbolAssetId { get; set; } = string.Empty;

    public string SemanticKind { get; set; } = string.Empty;

    public double Intensity { get; set; }

    public string Color { get; set; } = "#ffffff";

    public double Scale { get; set; } = 1.0;

    public double HeightOffset { get; set; } = 1.2;

    public string Anchor { get; set; } = WebGlSymbolAnchors.Top;

    public bool BillboardToCamera { get; set; } = true;

    public string EffectKey { get; set; } = WebGlSymbolEffects.None;

    public string Tooltip { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

