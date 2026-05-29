namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSymbolIntensityPolicy
{
    public double MinimumScale { get; set; } = 0.75;

    public double MaximumScale { get; set; } = 1.65;

    public double MinimumOpacity { get; set; } = 0.45;

    public double MaximumOpacity { get; set; } = 1.0;

    public string LowColor { get; set; } = "#38bdf8";

    public string MediumColor { get; set; } = "#facc15";

    public string HighColor { get; set; } = "#f97316";
}

