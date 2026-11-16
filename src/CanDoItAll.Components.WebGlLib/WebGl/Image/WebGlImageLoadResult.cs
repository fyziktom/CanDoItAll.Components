namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// The outcome of loading a <see cref="WebGlImageSource"/>: the image's size, the statistics of its valid samples and a
/// suggested window (the 0.5 % and 99.5 % quantiles of the displayed values).
/// </summary>
public sealed record WebGlImageLoadResult
{
    /// <summary>The source's key.</summary>
    public string Key { get; init; } = "";

    /// <summary>The image was loaded (false: it failed or a newer source replaced it).</summary>
    public bool Ok { get; init; }

    /// <summary>A newer source replaced this one before it was loaded.</summary>
    public bool Superseded { get; init; }

    /// <summary>Why the image could not be loaded.</summary>
    public string? Message { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    /// <summary>Pixels that are neither masked nor non-finite.</summary>
    public long ValidCount { get; init; }

    public double? Mean { get; init; }

    public double? Minimum { get; init; }

    public double? Maximum { get; init; }

    public double? SuggestedLow { get; init; }

    public double? SuggestedHigh { get; init; }

    /// <summary>The image is drawn (false without WebGL2 or when it exceeds the GPU's texture size).</summary>
    public bool Drawable { get; init; }

    /// <summary>The suggested window, when the image has valid values.</summary>
    public WebGlImageWindow? SuggestedWindow =>
        SuggestedLow is { } low && SuggestedHigh is { } high ? new WebGlImageWindow(low, high) : null;
}
