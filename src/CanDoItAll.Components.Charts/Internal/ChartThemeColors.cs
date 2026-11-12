namespace CanDoItAll.Components.Charts;

/// <summary>
/// Colors resolved from Tailwind/theme.css's <c>--ui-charts-*</c> tokens via charts-theme.js.
/// Passed through <see cref="CdaApexChartOptionsFactory.Build"/> to replace the hardcoded
/// literal fallbacks it otherwise uses when this is <see langword="null"/> (no <c>ChartsBodyAssets</c>
/// present, or theme-tokens.js hasn't resolved yet — see CLAUDE.md rule 8's degrade requirement).
/// </summary>
internal sealed record ChartThemeColors(
    IReadOnlyList<string> Palette,
    string GridStripe,
    string LegendText,
    string StrokeDefault,
    bool IsDark);
