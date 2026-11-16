namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlImageDisplayContractTests
{
    [Theory]
    [InlineData(0, 1, true)]
    [InlineData(-5, -1, true)]
    [InlineData(1, 1, false)]
    [InlineData(2, 1, false)]
    [InlineData(double.NaN, 1, false)]
    [InlineData(0, double.PositiveInfinity, false)]
    public void Window_is_valid_only_with_finite_edges_and_a_width(double low, double high, bool valid)
    {
        Assert.Equal(valid, new WebGlImageWindow(low, high).IsValid);
    }

    [Fact]
    public void Window_union_covers_both_windows()
    {
        var union = new WebGlImageWindow(2, 5).Union(new WebGlImageWindow(-1, 3));

        Assert.Equal(new WebGlImageWindow(-1, 5), union);
        Assert.Equal(6, union.Width);
    }

    [Fact]
    public void Display_state_drops_an_invalid_window_an_empty_region_and_a_negative_profile_row()
    {
        var state = WebGlImageDisplayState.From(new WebGlImageWindow(3, 3), gamma: 0, invert: false, WebGlImageDisplayMode.Linear,
            smooth: false, new WebGlImageRegion(1, 2, 0, 4), profileRow: -2);

        Assert.Null(state.Low);
        Assert.Null(state.High);
        Assert.Equal(1, state.Gamma);
        Assert.Null(state.Region);
        Assert.Null(state.ProfileRow);
    }

    [Fact]
    public void Display_state_compares_by_value_so_an_unchanged_display_is_not_sent_again()
    {
        WebGlImageDisplayState Make() => WebGlImageDisplayState.From(new WebGlImageWindow(0, 10), 1.5, true,
            WebGlImageDisplayMode.NegativeLogRatio, true, new WebGlImageRegion(1, 2, 3, 4), 7);

        Assert.Equal(Make(), Make());
        Assert.NotEqual(Make(), Make() with { Gamma = 2 });
    }

    [Theory]
    [InlineData(WebGlImageDisplayMode.Linear, "linear")]
    [InlineData(WebGlImageDisplayMode.NegativeLogRatio, "negative-log-ratio")]
    public void Display_modes_have_the_browser_module_names(WebGlImageDisplayMode mode, string name)
    {
        Assert.Equal(name, WebGlImageDisplayState.ModeName(mode));
    }

    [Fact]
    public void Every_element_type_round_trips_through_its_descriptor_name()
    {
        foreach (WebGlImageElementType type in Enum.GetValues<WebGlImageElementType>())
        {
            string name = WebGlImageElementTypes.NameOf(type);
            Assert.Equal(type, WebGlImageElementTypes.Parse(name));
            Assert.Equal(type, WebGlImageElementTypes.Parse(" " + name.ToUpperInvariant() + " "));
        }
    }

    [Theory]
    [InlineData("f32", WebGlImageElementType.Float32)]
    [InlineData("u16", WebGlImageElementType.UInt16)]
    [InlineData("i64", WebGlImageElementType.Int64)]
    public void Short_element_type_names_are_understood(string name, WebGlImageElementType expected)
    {
        Assert.Equal(expected, WebGlImageElementTypes.Parse(name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("complex64")]
    [InlineData("bool")]
    public void Unknown_element_types_are_not_displayable(string? name)
    {
        Assert.Null(WebGlImageElementTypes.Parse(name));
    }

    [Fact]
    public void Load_result_names_its_suggested_window_only_when_both_edges_are_known()
    {
        Assert.Equal(new WebGlImageWindow(1, 2), new WebGlImageLoadResult { SuggestedLow = 1, SuggestedHigh = 2 }.SuggestedWindow);
        Assert.Null(new WebGlImageLoadResult { SuggestedLow = 1 }.SuggestedWindow);
    }

    [Fact]
    public void Image_source_defaults_to_float32_samples_without_mask_or_reference()
    {
        var source = new WebGlImageSource("k", "/plane", 4, 3);

        Assert.Equal(WebGlImageElementType.Float32, source.ElementType);
        Assert.Null(source.MaskUrl);
        Assert.Null(source.ReferenceUrl);
        Assert.Null(source.ReferenceElementType);
    }

    [Fact]
    public void The_browser_module_exports_the_view_and_the_histogram_and_their_proof_hooks()
    {
        string module = File.ReadAllText(Path.Combine(FindRepoRoot(), "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "image", "webgl-image-display.js"));

        Assert.Contains("export function createImageView(", module, StringComparison.Ordinal);
        Assert.Contains("export function createWindowHistogram(", module, StringComparison.Ordinal);
        foreach (string hook in new[] { "proof:", "readPixel:", "displayedValues:", "histogram:" })
            Assert.Contains(hook, module, StringComparison.Ordinal);
        foreach (string callback in new[] { "NotifyProbe", "NotifyStatus", "NotifyWindowChanged", "NotifyWindowInput", "NotifyStats" })
            Assert.Contains($"'{callback}'", module, StringComparison.Ordinal);
        Assert.EndsWith(WebGlFloatImageView.ModulePath["./_content/CanDoItAll.Components.WebGlLib/".Length..], "js/image/webgl-image-display.js", StringComparison.Ordinal);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx")) && !Directory.Exists(Path.Combine(directory.FullName, "src", "CanDoItAll.Components.WebGlLib")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("repository root not found");
    }
}
