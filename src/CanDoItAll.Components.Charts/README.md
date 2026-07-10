# CanDoItAll.Components.Charts

Package version: `0.1.0`.

## Purpose

Charts provides a friendly, typed Blazor boundary around ApexCharts. Use it for product dashboards and reports when you want consistent chart containers, accessible summaries, application-owned data models, and palette/options that do not leak a third-party chart API throughout the codebase.

## Quick start

Register the services, add the package stylesheet once in the host, and render `CdaChart` with typed series and options:

```csharp
// Program.cs
builder.Services.AddCanDoItAllCharts();
```

```razor
@using CanDoItAll.Components.Charts

@* App.razor <head> *@
<ChartsHeadAssets />

<CdaChart Title="Weekly sign-ups"
          Description="New accounts by week."
          Series="@series"
          Options="@options"
          AccessibleSummary="Sign-ups increased from 14 to 31 across four weeks." />
```

Keep your reporting data in application models and map it to `CdaChartSeries`/`CdaChartPoint` at the UI boundary. `CdaChartOptions` holds the chart type, axes, legend, curve, labels, animation, and palette choices; `CdaChart` renders a useful empty state when there is no data.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj
```

## References

Project references:

- None

Framework references:

- None

Direct package references:

- `Blazor-ApexCharts (6.1.0)`
- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Architecture Notes

Use this library when product modules need charts instead of binding directly to ApexCharts. Keep `CdaChart`, `CdaChartOptions`, `CdaChartSeries`, and `CdaChartPoint` as the typed boundary so modules can share chart behavior without copying JavaScript or package-specific options. View the running variations at the Sandbox `/groups/charts` route.

## Related Docs

- [Repository overview](../../README.md)
- [Sandbox chart examples](../CanDoItAll.Components.Sandbox/README.md)
