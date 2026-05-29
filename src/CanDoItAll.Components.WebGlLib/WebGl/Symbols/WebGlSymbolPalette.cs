namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSymbolPalette
{
    public string Neutral { get; set; } = "#e2e8f0";

    public string Info { get; set; } = "#38bdf8";

    public string Ready { get; set; } = "#22c55e";

    public string Busy { get; set; } = "#f97316";

    public string Warning { get; set; } = "#facc15";

    public string Alert { get; set; } = "#ef4444";

    public Dictionary<string, string> CustomColors { get; set; } = [];

    public string Resolve(string semanticKind)
    {
        if (CustomColors.TryGetValue(semanticKind, out var customColor) && !string.IsNullOrWhiteSpace(customColor))
        {
            return customColor;
        }

        return semanticKind switch
        {
            "info" => Info,
            "ready" or "available" => Ready,
            "busy" or "needs-input" => Busy,
            "warning" or "blocked" => Warning,
            "alert" => Alert,
            _ => Neutral
        };
    }
}

