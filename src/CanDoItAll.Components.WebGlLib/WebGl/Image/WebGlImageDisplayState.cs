namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// What a <see cref="WebGlFloatImageView"/> sends to the browser module: compared by value, so an unchanged display is
/// never sent again.
/// </summary>
internal sealed record WebGlImageDisplayState(
    double? Low, double? High, double Gamma, bool Invert, WebGlImageDisplayMode Mode, bool Smooth, WebGlImageRegion? Region, double? ProfileRow)
{
    public static WebGlImageDisplayState From(
        WebGlImageWindow? window, double gamma, bool invert, WebGlImageDisplayMode mode, bool smooth, WebGlImageRegion? region, double? profileRow) =>
        new(window is { IsValid: true } w ? w.Low : null, window is { IsValid: true } v ? v.High : null,
            double.IsFinite(gamma) && gamma > 0 ? gamma : 1, invert, mode, smooth,
            region is { IsEmpty: false } r ? r : null, profileRow is { } p && double.IsFinite(p) && p >= 0 ? p : null);

    public object ToJs() => new
    {
        low = Low,
        high = High,
        gamma = Gamma,
        invert = Invert,
        mode = ModeName(Mode),
        smooth = Smooth,
        region = Region is { } r ? new[] { r.X, r.Y, r.Width, r.Height } : null,
        profileRow = ProfileRow ?? -1,
    };

    public static object SourceToJs(WebGlImageSource source) => new
    {
        key = source.Key,
        url = source.Url,
        width = source.Width,
        height = source.Height,
        elementType = ElementTypeName(source.ElementType),
        maskUrl = source.MaskUrl,
        referenceUrl = source.ReferenceUrl,
        referenceElementType = source.ReferenceElementType is { } t ? ElementTypeName(t) : null,
        label = source.Label,
    };

    public static string ModeName(WebGlImageDisplayMode mode) =>
        mode == WebGlImageDisplayMode.NegativeLogRatio ? "negative-log-ratio" : "linear";

    public static string ElementTypeName(WebGlImageElementType type) => type switch
    {
        WebGlImageElementType.Float32 => "float32",
        WebGlImageElementType.Float64 => "float64",
        WebGlImageElementType.UInt8 => "uint8",
        WebGlImageElementType.Int8 => "int8",
        WebGlImageElementType.UInt16 => "uint16",
        WebGlImageElementType.Int16 => "int16",
        WebGlImageElementType.UInt32 => "uint32",
        WebGlImageElementType.Int32 => "int32",
        WebGlImageElementType.UInt64 => "uint64",
        WebGlImageElementType.Int64 => "int64",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "unknown element type"),
    };

    /// <summary>
    /// The element type named in a data header or descriptor ("float32", "uint16", ...), or null when it is not one the
    /// view can display.
    /// </summary>
    public static WebGlImageElementType? ParseElementType(string? name) => name?.Trim().ToLowerInvariant() switch
    {
        "float32" or "f32" => WebGlImageElementType.Float32,
        "float64" or "f64" => WebGlImageElementType.Float64,
        "uint8" or "u8" => WebGlImageElementType.UInt8,
        "int8" or "i8" => WebGlImageElementType.Int8,
        "uint16" or "u16" => WebGlImageElementType.UInt16,
        "int16" or "i16" => WebGlImageElementType.Int16,
        "uint32" or "u32" => WebGlImageElementType.UInt32,
        "int32" or "i32" => WebGlImageElementType.Int32,
        "uint64" or "u64" => WebGlImageElementType.UInt64,
        "int64" or "i64" => WebGlImageElementType.Int64,
        _ => null,
    };
}
