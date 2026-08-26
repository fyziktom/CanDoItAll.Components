namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class HomoAvatarGeneratorTests
{
    [Theory]
    [InlineData("jan", 6, "213735")]
    [InlineData("jan", 12, "213735096181")]
    [InlineData("", 6, "378920")]
    [InlineData("homo-avatar", 6, "378920")]
    [InlineData("Hello, World!", 6, "726136")]
    public void TextToAvatarIdMatchesReferenceImplementation(string text, int length, string expected)
    {
        Assert.Equal(expected, HomoAvatarGenerator.TextToAvatarId(text, length));
    }

    [Fact]
    public void RenderSvgIsDeterministicForSameOptions()
    {
        var options = new HomoAvatarOptions { Id = "213735", GradientId = "fixed" };

        var first = HomoAvatarGenerator.RenderSvg(options);
        var second = HomoAvatarGenerator.RenderSvg(options);

        Assert.Equal(first, second);
    }

    [Fact]
    public void RenderSvgProducesExpectedPlainMarkupForKnownId()
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735" });

        Assert.Contains(
            "<g data-homo-part=\"body\" stroke=\"oklch(79.5% 0.184 86.047)\" fill=\"var(--tone-surface, #fff)\"><path d=\"M 9,4 H 15 A 6,6 0 0 1 21,10 V 15 A 6,6 0 0 1 15,21 H 9 A 6,6 0 0 1 3,15 V 10 A 6,6 0 0 1 9,4 Z M 4,8.5 C 5,3 10.5,2.7 12.2,3.6 C 13,2.9 19,3.3 20.5,8.5 M 1.6,12.5 A 1,1 0 1 1 3.6,12.5 A 1,1 0 1 1 1.6,12.5 Z M 20.4,12.5 A 1,1 0 1 1 22.4,12.5 A 1,1 0 1 1 20.4,12.5 Z\"></path></g>",
            svg,
            StringComparison.Ordinal);
        Assert.Contains(
            "<g data-homo-part=\"mouth\" stroke=\"oklch(71.5% 0.143 215.221)\" fill=\"var(--tone-surface, #fff)\"><path d=\"M 8,17 H 16\"></path></g>",
            svg,
            StringComparison.Ordinal);
        Assert.Contains(
            "<g data-homo-part=\"eyes\" stroke=\"oklch(71.5% 0.143 215.221)\" fill=\"var(--tone-surface, #fff)\"><path d=\"M 6.4,10.5 A 2.1,2.1 0 1 1 10.6,10.5 A 2.1,2.1 0 1 1 6.4,10.5 Z M 8.1,10.5 A 0.4,0.4 0 1 1 8.9,10.5 A 0.4,0.4 0 1 1 8.1,10.5 Z M 13.4,10.5 A 2.1,2.1 0 1 1 17.6,10.5 A 2.1,2.1 0 1 1 13.4,10.5 Z M 15.1,10.5 A 0.4,0.4 0 1 1 15.9,10.5 A 0.4,0.4 0 1 1 15.1,10.5 Z\"></path></g>",
            svg,
            StringComparison.Ordinal);
    }

    [Fact]
    public void DifferentSeedsProduceDifferentSelections()
    {
        var svgOne = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Text = "jan" });
        var svgTwo = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Text = "someone-else" });

        Assert.NotEqual(svgOne, svgTwo);
    }

    [Theory]
    [InlineData(RoboAvatarVariant.Fill)]
    [InlineData(RoboAvatarVariant.Gradient)]
    public void OnlyBodyPartEverReceivesGradientFill(RoboAvatarVariant variant)
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735", Variant = variant, GradientId = "abc123" });

        Assert.Contains("<defs>", svg, StringComparison.Ordinal);
        Assert.Contains("<g data-homo-part=\"body\" fill=\"url(#cda-homo-abc123)\" stroke=\"", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("data-homo-part=\"mouth\" fill=\"url(", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("data-homo-part=\"eyes\" fill=\"url(", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void PlainStyleOmitsDefsBlock()
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735" });

        Assert.DoesNotContain("<defs>", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void TitleIsEscapedAndAddsAccessibleRole()
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735", Title = "A & B <robot>" });

        Assert.Contains("<title>A &amp; B &lt;robot&gt;</title>", svg, StringComparison.Ordinal);
        Assert.Contains("role=\"img\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void ShortOrEmptyIdDoesNotThrow()
    {
        var shortIdException = Record.Exception(() => HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "1" }));
        Assert.Null(shortIdException);

        var emptyIdException = Record.Exception(() => HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = string.Empty }));
        Assert.Null(emptyIdException);
    }

    [Fact]
    public void ChoicesOverrideResolvesToExpectedZeroBasedIndex()
    {
        // choices["body"] = 2 is 1-based, so it must resolve to BodyShapes[1] (wrapIndex(2 - 1, count)).
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions
        {
            Id = "000000",
            Choices = new Dictionary<string, int> { ["body"] = 2 }
        });

        Assert.Contains(
            "<g data-homo-part=\"body\" stroke=\"oklch(70.5% 0.213 47.604)\" fill=\"var(--tone-surface, #fff)\"><path d=\"M 5,12.5 A 7,9.5 0 1 1 19,12.5 A 7,9.5 0 1 1 5,12.5 Z M 6,5 C 9,3.3 14,2.8 18,7\"></path></g>",
            svg,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ColorChoiceOverrideUsesTheColorSpecificKey()
    {
        // choices["body-color"] = 2 selects palette index 1 (wrapIndex(2 - 1, 6)) instead of the id-derived color 0.
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions
        {
            Id = "000000",
            Choices = new Dictionary<string, int> { ["body-color"] = 2 }
        });

        Assert.Contains("data-homo-part=\"body\" stroke=\"oklch(79.5% 0.184 86.047)\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void DarkFillUsesEqualOpacityStops()
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735", Variant = RoboAvatarVariant.Fill, Dark = true });

        Assert.Contains("stop-opacity=\"0.3\" /><stop offset=\"100%\" stop-color=\"oklch(79.5% 0.184 86.047)\" stop-opacity=\"0.3\"", svg, StringComparison.Ordinal);
    }

    [Fact]
    public void LightGradientUsesAsymmetricOpacityStops()
    {
        var svg = HomoAvatarGenerator.RenderSvg(new HomoAvatarOptions { Id = "213735", Variant = RoboAvatarVariant.Gradient, Dark = false });

        Assert.Contains("stop-opacity=\"0.6\" /><stop offset=\"100%\" stop-color=\"oklch(79.5% 0.184 86.047)\" stop-opacity=\"0.12\"", svg, StringComparison.Ordinal);
    }
}
