namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetMaterialOverride
{
    public string MaterialName { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public double Opacity { get; set; } = 1.0;

    public bool IsEmissive { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

