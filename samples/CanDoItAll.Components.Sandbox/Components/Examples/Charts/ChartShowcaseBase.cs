using Microsoft.AspNetCore.Components;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Sandbox;

namespace CanDoItAll.Components.Sandbox.Components.Examples.Charts;

public abstract class ChartShowcaseBase : ComponentBase
{
    [CascadingParameter]
    public SandboxToolbarState? ToolbarState { get; set; }

    protected SandboxFramePreset Frame => ToolbarState?.Frame ?? SandboxFramePreset.LiveViewport;

    protected static readonly DateTime Start = new(2026, 4, 30, 3, 0, 0, DateTimeKind.Utc);
    protected static readonly IReadOnlyList<CdaChartSeries> EmptySeries = [];

    protected bool IsDense => true;

    protected object PrimaryChartHeight => Frame == SandboxFramePreset.Mobile ? 300 : 340;

    protected object WideChartHeight => Frame == SandboxFramePreset.Mobile ? 340 : 390;

    protected int TotalSeriesCount => ConsumptionAreaSeries.Count + ShareSeries.Count + MultiLineSeries.Count + PriceSeries.Count + ColorTunedSeries.Count;

    protected int TotalPointCount => AllSeries.Sum(series => series.Points.Count);

    protected IEnumerable<CdaChartSeries> AllSeries => ConsumptionAreaSeries
        .Concat(ShareSeries)
        .Concat(MultiLineSeries)
        .Concat(PriceSeries)
        .Concat(ColorTunedSeries);

    protected IReadOnlyList<CdaChartSeries> ConsumptionAreaSeries =>
    [
        new()
        {
            Name = "Main meter",
            Type = CdaChartType.Area,
            Color = "#14b8a6",
            Points = BuildDailyProfile(IsDense ? 32 : 24, 0.18m, 0.54m, 0.11m)
        }
    ];

    protected IReadOnlyList<CdaChartSeries> ShareSeries =>
    [
        new()
        {
            Name = "Source share",
            Type = CdaChartType.Pie,
            Points =
            [
                new("Solar", 38m, "#16a34a"),
                new("Grid import", 34m, "#2563eb"),
                new("Battery", 18m, "#7c3aed"),
                new("Export", 10m, "#0891b2")
            ]
        }
    ];

    protected IReadOnlyList<CdaChartSeries> MultiLineSeries =>
    [
        new()
        {
            Name = "House kWh",
            Type = CdaChartType.Area,
            Color = "#2563eb",
            Points = BuildDailyProfile(IsDense ? 30 : 24, 1.1m, 6.4m, 0.8m)
        },
        new()
        {
            Name = "Solar kWh",
            Type = CdaChartType.Area,
            Color = "#16a34a",
            Points = BuildSolarProfile(IsDense ? 30 : 24)
        },
        new()
        {
            Name = "Battery kWh",
            Type = CdaChartType.Line,
            Color = "#7c3aed",
            StrokeDash = 5,
            Points = BuildPulseProfile(IsDense ? 30 : 24, 0.2m, 3.8m, 8)
        },
        new()
        {
            Name = "Export kWh",
            Type = CdaChartType.Line,
            Color = "#0891b2",
            Points = BuildPulseProfile(IsDense ? 30 : 24, 0.0m, 1.4m, 5)
        }
    ];

    protected IReadOnlyList<CdaChartSeries> PriceSeries =>
    [
        new()
        {
            Name = "Spot CZK",
            Type = CdaChartType.Line,
            Color = "#0f766e",
            ShowDataLabels = true,
            Points = BuildPriceProfile(IsDense ? 14 : 8)
        }
    ];

    protected IReadOnlyList<CdaChartSeries> ColorTunedSeries =>
    [
        new()
        {
            Name = "Import / export",
            Type = CdaChartType.Bar,
            Points = BuildImportExportProfile(IsDense ? 18 : 12)
        }
    ];

    protected static readonly CdaChartOptions AreaOptions = new()
    {
        Type = CdaChartType.Area,
        Unit = "kWh",
        XAxisType = CdaChartAxisType.DateTime,
        YAxisTitle = "Consumption (kWh)",
        FillOpacity = 0.36,
        Curve = CdaChartCurve.Step,
        LegendPosition = CdaChartLegendPosition.Bottom,
        Palette = ["#14b8a6"]
    };

    protected static readonly CdaChartOptions PieOptions = new()
    {
        Type = CdaChartType.Pie,
        XAxisType = CdaChartAxisType.Category,
        ShowDataLabels = true,
        ShowToolbar = false,
        EnableZoom = false,
        LegendPosition = CdaChartLegendPosition.Bottom,
        Palette = ["#16a34a", "#2563eb", "#7c3aed", "#0891b2"]
    };

    protected static readonly CdaChartOptions MultiLineOptions = new()
    {
        Type = CdaChartType.Area,
        Unit = "kWh",
        XAxisType = CdaChartAxisType.DateTime,
        YAxisTitle = "Energy (kWh)",
        FillOpacity = 0.22,
        Curve = CdaChartCurve.Straight,
        LegendPosition = CdaChartLegendPosition.Top,
        Palette = ["#2563eb", "#16a34a", "#7c3aed", "#0891b2"]
    };

    protected static readonly CdaChartOptions PriceOptions = new()
    {
        Type = CdaChartType.Line,
        Unit = "CZK",
        XAxisType = CdaChartAxisType.DateTime,
        YAxisTitle = "Price (CZK)",
        Curve = CdaChartCurve.Smooth,
        ShowToolbar = false,
        ShowDataLabels = true,
        LegendPosition = CdaChartLegendPosition.Bottom,
        Palette = ["#0f766e"]
    };

    protected static readonly CdaChartOptions BarOptions = new()
    {
        Type = CdaChartType.Bar,
        Unit = "kWh",
        XAxisType = CdaChartAxisType.Category,
        XAxisTitle = "Interval",
        YAxisTitle = "Energy balance",
        ValuePrecision = 1,
        TooltipPrecision = 2,
        ShowToolbar = false,
        RotateXAxisLabels = false,
        LegendPosition = CdaChartLegendPosition.Bottom,
        Palette = ["#2563eb", "#dc2626"]
    };

    protected static IReadOnlyList<CdaChartPoint> BuildDailyProfile(int count, decimal floor, decimal peak, decimal eveningBump)
    {
        var points = new List<CdaChartPoint>(count);
        for (var index = 0; index < count; index++)
        {
            var hour = index * 24m / count;
            var morning = Math.Max(0, 1 - Math.Abs((double)((hour - 9m) / 5m)));
            var evening = Math.Max(0, 1 - Math.Abs((double)((hour - 19m) / 2.5m)));
            var value = floor + (peak - floor) * (decimal)(morning * 0.82) + eveningBump * (decimal)evening;
            points.Add(new CdaChartPoint(Start.AddMinutes(index * 60), Math.Round(value, 2)));
        }

        return points;
    }

    protected static IReadOnlyList<CdaChartPoint> BuildSolarProfile(int count)
    {
        var points = new List<CdaChartPoint>(count);
        for (var index = 0; index < count; index++)
        {
            var hour = index * 24d / count;
            var value = hour is < 6 or > 18 ? 0 : Math.Sin((hour - 6) / 12 * Math.PI) * 4.2;
            points.Add(new CdaChartPoint(Start.AddMinutes(index * 60), Math.Round((decimal)value, 2)));
        }

        return points;
    }

    protected static IReadOnlyList<CdaChartPoint> BuildPulseProfile(int count, decimal floor, decimal high, int spacing)
    {
        var points = new List<CdaChartPoint>(count);
        for (var index = 0; index < count; index++)
        {
            var isPulse = index % spacing is 0 or 1;
            var value = isPulse ? high - (index % 3) * 0.35m : floor + (index % 4) * 0.08m;
            points.Add(new CdaChartPoint(Start.AddMinutes(index * 60), Math.Round(value, 2)));
        }

        return points;
    }

    protected static IReadOnlyList<CdaChartPoint> BuildPriceProfile(int count)
    {
        var points = new List<CdaChartPoint>(count);
        for (var index = 0; index < count; index++)
        {
            var value = 3.8m + (decimal)Math.Sin(index * 0.9) * 1.1m + (index % 3 == 0 ? 0.4m : 0m);
            points.Add(new CdaChartPoint(Start.AddHours(index * 2), Math.Round(value, 2)));
        }

        return points;
    }

    protected static IReadOnlyList<CdaChartPoint> BuildImportExportProfile(int count)
    {
        var points = new List<CdaChartPoint>(count);
        for (var index = 0; index < count; index++)
        {
            var export = index is >= 4 and <= 8;
            var value = export ? 0.7m + (index % 3) * 0.22m : 1.1m + (index % 4) * 0.28m;
            var color = export ? "#16a34a" : "#2563eb";
            points.Add(new CdaChartPoint($"T{index + 1}", Math.Round(value, 2), color, export ? "Export" : "Import"));
        }

        return points;
    }
}
