namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunTimeline
{
    public int FrameRate { get; set; } = 30;

    public List<WebGlRunFrame> Frames { get; set; } = [];
}
