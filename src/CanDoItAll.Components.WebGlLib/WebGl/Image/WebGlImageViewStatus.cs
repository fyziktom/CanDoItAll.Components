namespace CanDoItAll.Components.WebGlLib;

/// <summary>What a <see cref="WebGlFloatImageView"/> can draw.</summary>
public enum WebGlImageViewStatus
{
    /// <summary>WebGL2 draws the image.</summary>
    Ready,

    /// <summary>The browser has no WebGL2: values and statistics are still available.</summary>
    Unavailable,

    /// <summary>The WebGL context was lost; the view draws again when the browser restores it.</summary>
    ContextLost,

    /// <summary>The image exceeds the GPU's texture size: values and statistics are still available.</summary>
    TooLarge,

    /// <summary>The image could not be loaded.</summary>
    LoadFailed
}

/// <summary>A change of what a <see cref="WebGlFloatImageView"/> can draw, with a sentence for the user.</summary>
public sealed record WebGlImageViewStatusEventArgs(WebGlImageViewStatus Status, string Message);
