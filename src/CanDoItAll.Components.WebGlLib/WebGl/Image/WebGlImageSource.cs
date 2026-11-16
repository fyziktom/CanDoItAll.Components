namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// Where a <see cref="WebGlFloatImageView"/> reads an image: raw little-endian samples, row zero first (drawn at the
/// top), fetched from same-origin URLs with the page's credentials. A different <see cref="Key"/> loads again; the same
/// key never does.
/// </summary>
/// <param name="Key">Identity of the image (for example a content hash with the plane's name).</param>
/// <param name="Url">The samples: <see cref="Width"/> × <see cref="Height"/> elements of <see cref="ElementType"/>.</param>
/// <param name="Width">Pixels per row.</param>
/// <param name="Height">Rows.</param>
public sealed record WebGlImageSource(string Key, string Url, int Width, int Height)
{
    /// <summary>The element type of the samples.</summary>
    public WebGlImageElementType ElementType { get; init; } = WebGlImageElementType.Float32;

    /// <summary>Optional mask of one byte per pixel; a nonzero byte marks the pixel invalid.</summary>
    public string? MaskUrl { get; init; }

    /// <summary>Optional reference plane of the same size for <see cref="WebGlImageDisplayMode.NegativeLogRatio"/>.</summary>
    public string? ReferenceUrl { get; init; }

    /// <summary>The element type of the reference plane (the samples' type when not given).</summary>
    public WebGlImageElementType? ReferenceElementType { get; init; }

    /// <summary>What the values are, for example "signal [keV]"; a linked histogram uses it as its axis label.</summary>
    public string? Label { get; init; }
}
