namespace CanDoItAll.Components.WebGlLib;

public sealed class DefaultWebGlSymbolPolicy : IWebGlSymbolPolicy
{
    private readonly WebGlSymbolIntensityPolicy intensityPolicy;
    private readonly WebGlSymbolPalette palette;

    public DefaultWebGlSymbolPolicy()
        : this(new WebGlSymbolIntensityPolicy(), new WebGlSymbolPalette())
    {
    }

    public DefaultWebGlSymbolPolicy(WebGlSymbolIntensityPolicy intensityPolicy, WebGlSymbolPalette palette)
    {
        this.intensityPolicy = intensityPolicy;
        this.palette = palette;
    }

    public WebGlStatusSymbol Normalize(WebGlStatusSymbol symbol)
    {
        var intensity = Math.Clamp(symbol.Intensity, 0, 1);
        var scale = symbol.Scale <= 0
            ? 1
            : symbol.Scale;

        return new WebGlStatusSymbol
        {
            Id = symbol.Id,
            SymbolAssetId = symbol.SymbolAssetId,
            SemanticKind = symbol.SemanticKind,
            Intensity = intensity,
            Color = string.IsNullOrWhiteSpace(symbol.Color) || string.Equals(symbol.Color, "#ffffff", StringComparison.OrdinalIgnoreCase)
                ? ResolveColor(symbol.SemanticKind, intensity)
                : symbol.Color,
            Scale = Math.Clamp(scale, intensityPolicy.MinimumScale, intensityPolicy.MaximumScale),
            HeightOffset = symbol.HeightOffset <= 0 ? 1.2 : symbol.HeightOffset,
            Anchor = string.IsNullOrWhiteSpace(symbol.Anchor) ? WebGlSymbolAnchors.Top : symbol.Anchor,
            BillboardToCamera = symbol.BillboardToCamera,
            EffectKey = NormalizeEffect(symbol.EffectKey),
            Tooltip = symbol.Tooltip,
            SortOrder = symbol.SortOrder,
            IsVisible = symbol.IsVisible,
            Metadata = new Dictionary<string, string>(symbol.Metadata, StringComparer.Ordinal)
        };
    }

    private string ResolveColor(string semanticKind, double intensity)
    {
        if (intensity >= 0.78)
        {
            return intensityPolicy.HighColor;
        }

        if (intensity >= 0.45)
        {
            return intensityPolicy.MediumColor;
        }

        var paletteColor = palette.Resolve(semanticKind);
        return string.IsNullOrWhiteSpace(paletteColor)
            ? intensityPolicy.LowColor
            : paletteColor;
    }

    private static string NormalizeEffect(string? effectKey)
        => effectKey switch
        {
            WebGlSymbolEffects.Pulse => WebGlSymbolEffects.Pulse,
            WebGlSymbolEffects.Blink => WebGlSymbolEffects.Blink,
            WebGlSymbolEffects.Float => WebGlSymbolEffects.Float,
            WebGlSymbolEffects.Spin => WebGlSymbolEffects.Spin,
            WebGlSymbolEffects.Glow => WebGlSymbolEffects.Glow,
            WebGlSymbolEffects.Shake => WebGlSymbolEffects.Shake,
            WebGlSymbolEffects.ScaleByIntensity => WebGlSymbolEffects.ScaleByIntensity,
            _ => WebGlSymbolEffects.None
        };
}

