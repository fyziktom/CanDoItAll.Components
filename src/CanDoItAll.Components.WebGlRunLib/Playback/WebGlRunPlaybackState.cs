namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackState
{
    public WebGlRunId RunId { get; set; } = new(string.Empty);

    public long CurrentFrameIndex { get; set; }

    public bool IsPlaying { get; set; }

    public double PlaybackSpeed { get; set; } = 1.0;
}
