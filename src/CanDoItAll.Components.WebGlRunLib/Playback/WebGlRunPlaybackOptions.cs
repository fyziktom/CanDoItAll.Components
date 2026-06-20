namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackOptions
{
    public bool StopAtTimelineEnd { get; set; } = true;

    public Func<TimeSpan, CancellationToken, ValueTask>? DelayAsync { get; set; }
}
