namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// A display window: values at or below <see cref="Low"/> are drawn black, at or above <see cref="High"/> white, and
/// values between them in grey levels (shaped by gamma).
/// </summary>
public readonly record struct WebGlImageWindow(double Low, double High)
{
    /// <summary>Both edges are finite and <see cref="High"/> exceeds <see cref="Low"/>.</summary>
    public bool IsValid => double.IsFinite(Low) && double.IsFinite(High) && High > Low;

    /// <summary>The window's width.</summary>
    public double Width => High - Low;

    /// <summary>The smallest window covering both windows.</summary>
    public WebGlImageWindow Union(WebGlImageWindow other) => new(Math.Min(Low, other.Low), Math.Max(High, other.High));
}
