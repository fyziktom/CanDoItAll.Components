namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// The element type of raw little-endian image samples as a <see cref="WebGlImageSource"/> delivers them. Samples are
/// converted to 32-bit floats for display only.
/// </summary>
public enum WebGlImageElementType
{
    Float32,
    Float64,
    UInt8,
    Int8,
    UInt16,
    Int16,
    UInt32,
    Int32,
    UInt64,
    Int64
}
