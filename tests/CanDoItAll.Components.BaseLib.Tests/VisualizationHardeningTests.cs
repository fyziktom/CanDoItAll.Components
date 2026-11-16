using System.Reflection;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Mermaid;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class VisualizationHardeningTests
{
    [Theory]
    [InlineData(false, "#0f172a", "#f8fafc")]
    [InlineData(true, "#f1f5f9", "#1e293b")]
    public void ChartKeepsAxisTextAndGridReadableInItsTheme(bool isDark, string text, string stripe) {
        var theme = new ChartThemeColors(["#2563eb"], stripe, text, "#64748b", isDark);

        var options = CdaApexChartOptionsFactory.Build(new CdaChartOptions(), [], null, theme);

        Assert.Equal(text, options.Chart.ForeColor);
        Assert.Equal("#64748b", options.Grid.BorderColor);
        Assert.Equal(text, options.Legend.Labels.Colors);
        Assert.Equal(isDark ? ApexCharts.Mode.Dark : ApexCharts.Mode.Light, options.Tooltip.Theme);
    }

    [Fact]
    public void MermaidDefaultsToStrictRenderingWithoutHtmlLabels()
    {
        var options = new MermaidDiagramOptions();

        Assert.Equal("strict", options.SecurityLevel);
        Assert.False(options.HtmlLabels);
    }

    [Fact]
    public void MermaidJavascriptAppliesHtmlLabelPolicyToRootAndFlowchartConfiguration()
    {
        var source = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "TestAssets",
            "mermaidDiagram.js"));

        Assert.Equal(
            2,
            source.Split("htmlLabels: options?.htmlLabels ?? false", StringSplitOptions.None).Length - 1);
    }

    [Fact]
    public void MermaidJavascriptKeepsNodeInteractionButDelegatesPanZoomToBaseLib()
    {
        var source = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "TestAssets",
            "mermaidDiagram.js"));

        Assert.Contains("data-cda-zoom-pan-interactive", source, StringComparison.Ordinal);
        Assert.DoesNotContain("function attachPanZoom", source, StringComparison.Ordinal);
        Assert.DoesNotContain("export function zoom", source, StringComparison.Ordinal);
        Assert.DoesNotContain("export function reset", source, StringComparison.Ordinal);
    }

    [Fact]
    public void MermaidJavascriptKeepsTheGlobalTooltipOutOfDocumentOverflow()
    {
        var source = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "TestAssets",
            "mermaidDiagram.js"));

        Assert.Contains("function stabilizeMermaidTooltip", source, StringComparison.Ordinal);
        Assert.Contains("tooltip.style.position = 'fixed'", source, StringComparison.Ordinal);
        Assert.Contains("stabilizeMermaidTooltip();", source, StringComparison.Ordinal);
    }

    [Fact]
    public void ChartBuildsAccessibleSeriesAndRangeSummary()
    {
        var chart = new CdaChart();
        SetParameter(
            chart,
            nameof(CdaChart.Series),
            new CdaChartSeries[]
            {
                new()
                {
                    Name = "Latency",
                    Points = [new("start", 10), new("finish", 42)]
                }
            });

        Invoke(chart, "OnParametersSet");

        var summary = (string)chart.GetType()
            .GetProperty("ResolvedAccessibleSummary", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(chart)!;

        Assert.Contains("Latency: 2 points", summary, StringComparison.Ordinal);
        Assert.Contains("minimum 10", summary, StringComparison.Ordinal);
        Assert.Contains("maximum 42", summary, StringComparison.Ordinal);
    }

    private static void SetParameter(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!.SetValue(target, value);

    private static void Invoke(object target, string methodName)
        => target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, null);
}
