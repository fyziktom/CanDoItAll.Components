using System.Reflection;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Mermaid;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class VisualizationHardeningTests
{
    [Fact]
    public void MermaidDefaultsToStrictRenderingWithoutHtmlLabels()
    {
        var options = new MermaidDiagramOptions();

        Assert.Equal("strict", options.SecurityLevel);
        Assert.False(options.HtmlLabels);
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
