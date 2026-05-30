namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunFrameResolver
{
    public WebGlRunFrame? ResolveFrame(WebGlRunTimeline timeline, long frameIndex)
        => OrderFrames(timeline.Frames).FirstOrDefault(frame => frame.Index == frameIndex);

    public WebGlRunFrame? ResolveNextFrame(WebGlRunTimeline timeline, long currentFrameIndex)
        => OrderFrames(timeline.Frames).FirstOrDefault(frame => frame.Index > currentFrameIndex)
           ?? OrderFrames(timeline.Frames).FirstOrDefault();

    public WebGlRunFrame? ResolvePreviousFrame(WebGlRunTimeline timeline, long currentFrameIndex)
        => OrderFrames(timeline.Frames).LastOrDefault(frame => frame.Index < currentFrameIndex)
           ?? OrderFrames(timeline.Frames).FirstOrDefault();

    public IReadOnlyList<WebGlRunFrame> ResolveReplayFrames(WebGlRunTimeline timeline, long targetFrameIndex)
        => OrderFrames(timeline.Frames)
            .Where(frame => frame.Index <= targetFrameIndex)
            .ToList();

    public static IReadOnlyList<WebGlRunFrame> OrderFrames(IEnumerable<WebGlRunFrame> frames)
        => frames.OrderBy(static frame => frame.Index)
            .ThenBy(static frame => frame.TimeSeconds)
            .ToList();
}
