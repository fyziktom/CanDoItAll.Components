namespace CanDoItAll.Components.WebGlLib;

/// <summary>A rectangle in image pixels (column, row, width, height) a <see cref="WebGlFloatImageView"/> outlines.</summary>
public readonly record struct WebGlImageRegion(double X, double Y, double Width, double Height)
{
    /// <summary>The region has no area.</summary>
    public bool IsEmpty => !(Width > 0 && Height > 0);
}
