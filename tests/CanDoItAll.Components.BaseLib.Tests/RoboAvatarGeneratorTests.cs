namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class RoboAvatarGeneratorTests
{
    [Theory]
    [InlineData("jan", 6, "213735")]
    [InlineData("jan", 12, "213735096181")]
    [InlineData("", 6, "243594")]
    [InlineData("robo-avatar", 6, "243594")]
    [InlineData("Hello, World!", 6, "726136")]
    public void TextToAvatarIdMatchesReferenceImplementation(string text, int length, string expected)
    {
        Assert.Equal(expected, RoboAvatarGenerator.TextToAvatarId(text, length));
    }

    [Fact]
    public void RenderSvgIsDeterministicForSameOptions()
    {
        var options = new RoboAvatarOptions { Id = "213735", GradientId = "fixed" };

        var first = RoboAvatarGenerator.RenderSvg(options);
        var second = RoboAvatarGenerator.RenderSvg(options);

        Assert.Equal(first, second);
    }

    [Fact]
    public void RenderSvgProducesExpectedPlainMarkupForKnownId()
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735" });

        Assert.Contains(
            "<g data-robo-part=\"body\" stroke=\"oklch(62.7% 0.265 303.9)\" fill=\"var(--cad-color-surface, #fff)\"><path d=\"m 5,3 h 14 l 2,2 -7.444444,16.475555 H 10.444444 L 7.4125009,14.765457 3,5 Z\"></path></g>",
            svg,
            StringComparison.Ordinal);
        Assert.Contains(
            "<g data-robo-part=\"mouth\" stroke=\"oklch(58.5% 0.233 277.117)\" fill=\"var(--cad-color-surface, #fff)\"><path d=\"m 18,16 -6,3 -6,-3 v 0 z\"></path></g>",
            svg,
            StringComparison.Ordinal);
        Assert.Contains(
            "<g data-robo-part=\"eyes\" stroke=\"oklch(64.5% 0.246 16.439)\" fill=\"var(--cad-color-surface, #fff)\"><path d=\"m 14,12.5 2,-3 2,3 z m -8,0 2,-3 2,3 z\"></path></g>",
            svg,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DifferentSeedsProduceDifferentSelections()
    {
        var svgOne = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Text = "jan" });
        var svgTwo = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Text = "someone-else" });

        Assert.NotEqual(svgOne, svgTwo);
    }

    [Theory]
    [InlineData(RoboAvatarVariant.Fill)]
    [InlineData(RoboAvatarVariant.Gradient)]
    public void OnlyBodyPartEverReceivesGradientFill(RoboAvatarVariant variant)
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735", Variant = variant, GradientId = "abc123" });

        Assert.Contains("<defs>", svg, StringComparison.Ordinal);
        Assert.Contains("<g data-robo-part=\"body\" fill=\"url(#cda-robo-abc123)\" stroke=\"", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("data-robo-part=\"mouth\" fill=\"url(", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("data-robo-part=\"eyes\" fill=\"url(", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void PlainStyleOmitsDefsBlock()
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735" });

        Assert.DoesNotContain("<defs>", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void TitleIsEscapedAndAddsAccessibleRole()
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735", Title = "A & B <robot>" });

        Assert.Contains("<title>A &amp; B &lt;robot&gt;</title>", svg, StringComparison.Ordinal);
        Assert.Contains("role=\"img\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void ShortOrEmptyIdDoesNotThrow()
    {
        var shortIdException = Record.Exception(() => RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "1" }));
        Assert.Null(shortIdException);

        var emptyIdException = Record.Exception(() => RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = string.Empty }));
        Assert.Null(emptyIdException);
    }

    [Fact]
    public void ChoicesOverrideResolvesToExpectedZeroBasedIndex()
    {
        // choices["body"] = 2 is 1-based, so it must resolve to BodyShapes[1] (JS: wrapIndex(2 - 1, count)).
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions
        {
            Id = "000000",
            Choices = new Dictionary<string, int> { ["body"] = 2 }
        });

        Assert.Contains(
            "<g data-robo-part=\"body\" stroke=\"oklch(64.5% 0.246 16.439)\" fill=\"var(--cad-color-surface, #fff)\"><path d=\"M 12,1.5325091 10,4 h 4 z M 12,5.5 V 4 M 7.5000007,5.5 H 16.5 c 2.77,0 5,2.1541613 5,4.829958 v 6.315639 c 0,2.675797 -2.23,4.829958 -5,4.829958 H 7.5000007 c -2.7700002,0 -5.0000005,-2.154161 -5.0000005,-4.829958 V 10.329958 C 2.5000002,7.6541613 4.7300005,5.5 7.5000007,5.5 Z\"></path></g>",
            svg,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ColorChoiceOverrideUsesTheColorSpecificKey()
    {
        // choices["body-color"] = 2 selects palette index 1 (JS: wrapIndex(2 - 1, 6)) instead of the id-derived color 0.
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions
        {
            Id = "000000",
            Choices = new Dictionary<string, int> { ["body-color"] = 2 }
        });

        Assert.Contains("data-robo-part=\"body\" stroke=\"oklch(62.7% 0.265 303.9)\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void DarkFillUsesEqualOpacityStops()
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735", Variant = RoboAvatarVariant.Fill, Dark = true });

        Assert.Contains("stop-opacity=\"0.3\" /><stop offset=\"100%\" stop-color=\"oklch(62.7% 0.265 303.9)\" stop-opacity=\"0.3\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void LightGradientUsesAsymmetricOpacityStops()
    {
        var svg = RoboAvatarGenerator.RenderSvg(new RoboAvatarOptions { Id = "213735", Variant = RoboAvatarVariant.Gradient, Dark = false });

        Assert.Contains("stop-opacity=\"0.6\" /><stop offset=\"100%\" stop-color=\"oklch(62.7% 0.265 303.9)\" stop-opacity=\"0.12\"", svg, StringComparison.Ordinal);
    }
}
