using Bunit;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ChartRegistrationTests
{
    [Fact]
    public void RegisteredLineSeriesRerendersChartAfterInitialChildRegistration()
    {
        using var context = new BunitContext();
        var data = new[] { new ChartPoint(42), new ChartPoint(48), new ChartPoint(39) };
        RenderFragment childContent = builder =>
        {
            builder.OpenComponent<LineSeries>(0);
            builder.AddAttribute(1, nameof(LineSeries.Title), "API latency");
            builder.AddAttribute(2, nameof(LineSeries.Data), data);
            builder.AddAttribute(3, nameof(LineSeries.ValueProperty), nameof(ChartPoint.Value));
            builder.CloseComponent();
        };

        var cut = context.Render<Chart>(parameters => parameters
            .Add(component => component.ChildContent, childContent));

        cut.WaitForAssertion(() =>
        {
            Assert.Single(cut.FindAll("polyline"));
            Assert.Contains("API latency", cut.Markup, StringComparison.Ordinal);
            Assert.DoesNotContain("No chart data.", cut.Markup, StringComparison.Ordinal);
        });
    }

    private sealed record ChartPoint(double Value);
}
