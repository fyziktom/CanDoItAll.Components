using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class DefaultWebGlSymbolPolicyTests
{
    [Fact]
    public void Normalize_clamps_intensity_scale_and_fills_defaults()
    {
        var policy = new DefaultWebGlSymbolPolicy();
        var symbol = new WebGlStatusSymbol
        {
            SemanticKind = "ready",
            Intensity = 3.4,
            Scale = -2,
            HeightOffset = -1,
            EffectKey = "unexpected"
        };

        var normalized = policy.Normalize(symbol);

        Assert.Equal(1, normalized.Intensity);
        Assert.Equal(1, normalized.Scale);
        Assert.Equal(1.2, normalized.HeightOffset);
        Assert.Equal(WebGlSymbolAnchors.Top, normalized.Anchor);
        Assert.Equal(WebGlSymbolEffects.None, normalized.EffectKey);
        Assert.False(string.IsNullOrWhiteSpace(normalized.Color));
    }
}
