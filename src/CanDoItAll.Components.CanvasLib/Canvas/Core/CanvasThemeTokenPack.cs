namespace CanDoItAll.Components.CanvasLib;

public sealed record CanvasThemeSwatch(string Label, string Value, string Description);

public sealed class CanvasThemeTokenPack
{
    public static CanvasThemeTokenPack Default { get; } = new();

    public string ThemeKey { get; init; } = "canvas-sunrise";

    public string ThemeName { get; init; } = "Canvas Sunrise";

    public string StageRadius { get; init; } = "28px";

    public string CardRadius { get; init; } = "24px";

    public string PanelRadius { get; init; } = "22px";

    public string BorderSoft { get; init; } = "var(--ui-canvas-border-soft, rgba(15, 23, 42, 0.1))";

    public string ShadowSoft { get; init; } = "var(--ui-canvas-shadow-soft, 0 18px 36px rgba(15, 23, 42, 0.1))";

    public string ShadowStrong { get; init; } = "var(--ui-canvas-shadow-strong, 0 24px 56px rgba(15, 23, 42, 0.16))";

    public string BackgroundStart { get; init; } = "var(--ui-canvas-bg-start, #fff4e5)";

    public string BackgroundMid { get; init; } = "var(--ui-canvas-bg-mid, #f6faf7)";

    public string BackgroundEnd { get; init; } = "var(--ui-canvas-bg-end, #eef1ff)";

    public string AccentStart { get; init; } = "var(--ui-canvas-accent-start, #8b5cf6)";

    public string AccentEnd { get; init; } = "var(--ui-canvas-accent-end, #6d28d9)";

    public string DarkCard { get; init; } = "var(--color-neutral-900, #111827)";

    public string DarkCardBorder { get; init; } = "var(--ui-canvas-dark-card-border, #14b8a6)";

    public string Panel { get; init; } = "var(--ui-canvas-panel, rgba(255, 255, 255, 0.92))";

    public string MutedText { get; init; } = "var(--ui-canvas-muted, #64748b)";

    public string ForegroundText { get; init; } = "var(--ui-canvas-text, #0f172a)";

    public IReadOnlyDictionary<string, string> ToCssVariables()
        => new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["--cw-stage-radius"] = StageRadius,
            ["--cw-card-radius"] = CardRadius,
            ["--cw-panel-radius"] = PanelRadius,
            ["--cw-border-soft"] = BorderSoft,
            ["--cw-shadow-soft"] = ShadowSoft,
            ["--cw-shadow-strong"] = ShadowStrong,
            ["--cw-bg-start"] = BackgroundStart,
            ["--cw-bg-mid"] = BackgroundMid,
            ["--cw-bg-end"] = BackgroundEnd,
            ["--cw-accent-purple-start"] = AccentStart,
            ["--cw-accent-purple-end"] = AccentEnd,
            ["--cw-dark-card"] = DarkCard,
            ["--cw-dark-card-border"] = DarkCardBorder,
            ["--cw-panel"] = Panel,
            ["--cw-muted"] = MutedText,
            ["--cw-text"] = ForegroundText
        };

    public string ToInlineStyle()
        => string.Join(string.Empty, ToCssVariables().Select(pair => $"{pair.Key}:{pair.Value};"));

    public IReadOnlyList<CanvasThemeSwatch> BuildPreviewSwatches()
        =>
        [
            new("Backdrop start", BackgroundStart, "Warm entry tone for the stage gradient."),
            new("Backdrop mid", BackgroundMid, "Neutral midpoint used behind active nodes."),
            new("Backdrop end", BackgroundEnd, "Cool tail tone that keeps depth readable."),
            new("Accent start", AccentStart, "Primary action color for menus and highlights."),
            new("Accent end", AccentEnd, "Secondary accent that completes the gradient."),
            new("Dark card", DarkCard, "High-contrast card tone for overlays and previews."),
            new("Panel", Panel, "Shared glass panel fill used by host chrome."),
            new("Text", ForegroundText, "Primary readable text color.")
        ];

    public IReadOnlyList<string> BuildMetrics()
        =>
        [
            $"Stage radius {StageRadius}",
            $"Card radius {CardRadius}",
            $"Panel radius {PanelRadius}",
            $"Border {BorderSoft}",
            $"Shadow {ShadowSoft}"
        ];
}


