# WebGl symbol models skeleton

```csharp
namespace CanDoItAll.Components.WebGlLib;

public static class WebGlSymbolEffects
{
    public const string None = "none";
    public const string Pulse = "pulse";
    public const string Blink = "blink";
    public const string Float = "float";
    public const string Spin = "spin";
    public const string Glow = "glow";
    public const string Shake = "shake";
    public const string ScaleByIntensity = "scale-by-intensity";
}

public sealed class WebGlStatusSymbol
{
    public string Id { get; set; } = string.Empty;

    public string SymbolAssetId { get; set; } = string.Empty;

    public string SemanticKind { get; set; } = string.Empty;

    public double Intensity { get; set; }

    public string Color { get; set; } = "#ffffff";

    public double Scale { get; set; } = 1.0;

    public double HeightOffset { get; set; } = 1.2;

    public bool BillboardToCamera { get; set; } = true;

    public string EffectKey { get; set; } = WebGlSymbolEffects.None;

    public string Tooltip { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSymbolIntensityPolicy
{
    public double MinimumScale { get; set; } = 0.75;

    public double MaximumScale { get; set; } = 1.65;

    public double MinimumOpacity { get; set; } = 0.45;

    public double MaximumOpacity { get; set; } = 1.0;

    public string LowColor { get; set; } = "#e2e8f0";

    public string MediumColor { get; set; } = "#facc15";

    public string HighColor { get; set; } = "#ef4444";
}

public interface IWebGlSymbolPolicy
{
    WebGlStatusSymbol Normalize(WebGlStatusSymbol symbol);
}
```
