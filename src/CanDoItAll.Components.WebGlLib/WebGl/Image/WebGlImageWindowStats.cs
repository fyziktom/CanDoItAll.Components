namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// The displayed values a <see cref="WebGlImageWindowHistogram"/> bins, against its window: their range, the histogram's
/// scale and the fractions drawn black (below the window) and white (above it).
/// </summary>
public sealed record WebGlImageWindowStats
{
    public long Count { get; init; }

    public double? Minimum { get; init; }

    public double? Maximum { get; init; }

    public double? Low { get; init; }

    public double? High { get; init; }

    public double ScaleLow { get; init; }

    public double ScaleHigh { get; init; }

    public double BelowFraction { get; init; }

    public double AboveFraction { get; init; }
}
