namespace CanDoItAll.Components.WebGlLib;

/// <summary>The pixel under the pointer of a <see cref="WebGlFloatImageView"/>.</summary>
/// <param name="Column">Column in image pixels.</param>
/// <param name="Row">Row in image pixels (row zero at the top).</param>
/// <param name="Value">The sample (as displayed: a 32-bit float), or null when it is not finite.</param>
/// <param name="Reference">The reference plane's sample, when the source has one.</param>
/// <param name="Invalid">The mask marks the pixel, or its sample is not finite.</param>
public sealed record WebGlImageProbe(int Column, int Row, double? Value, double? Reference, bool Invalid);
