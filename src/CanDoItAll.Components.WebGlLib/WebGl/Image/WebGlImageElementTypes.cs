namespace CanDoItAll.Components.WebGlLib;

/// <summary>Names of <see cref="WebGlImageElementType"/> as data descriptors write them.</summary>
public static class WebGlImageElementTypes
{
    /// <summary>
    /// The element type a descriptor names ("float32", "uint16", "f64", ...), or null when a
    /// <see cref="WebGlFloatImageView"/> cannot display it.
    /// </summary>
    public static WebGlImageElementType? Parse(string? name) => WebGlImageDisplayState.ParseElementType(name);

    /// <summary>The descriptor name of an element type ("float32", "uint16", ...).</summary>
    public static string NameOf(WebGlImageElementType type) => WebGlImageDisplayState.ElementTypeName(type);
}
