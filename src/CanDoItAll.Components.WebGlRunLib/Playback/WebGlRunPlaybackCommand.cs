namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackCommand
{
    public string Kind { get; set; } = string.Empty;

    public long? TargetFrameIndex { get; set; }

    public string Reason { get; set; } = string.Empty;
}
