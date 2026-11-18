namespace CanDoItAll.Components.WebGlLib;

/// <summary>What a <see cref="WebGlFloatImageView"/> displays of each sample.</summary>
public enum WebGlImageDisplayMode
{
    /// <summary>The sample itself.</summary>
    Linear,

    /// <summary>
    /// The negative log ratio -ln(I / Iref) of the sample and the source's reference plane; a pixel without a positive
    /// sample and reference is invalid.
    /// </summary>
    NegativeLogRatio
}
