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

    public IReadOnlyList<WebGlRunFrame> ResolveSnapshotAnchorReplayFrames(WebGlRunTimeline timeline, long targetFrameIndex)
    {
        IReadOnlyList<WebGlRunFrame> ordered = OrderFrames(timeline.Frames);
        WebGlRunFrame? anchor = ordered
            .Where(frame => frame.Index <= targetFrameIndex && IsSnapshotAnchor(frame))
            .LastOrDefault();
        if (anchor is null)
        {
            return [];
        }

        return ordered
            .Where(frame => frame.Index >= anchor.Index && frame.Index <= targetFrameIndex)
            .ToList();
    }

    public static IReadOnlyList<WebGlRunFrame> OrderFrames(IEnumerable<WebGlRunFrame> frames)
        => frames.OrderBy(static frame => frame.Index)
            .ThenBy(static frame => frame.TimeSeconds)
            .ToList();

    private static bool IsSnapshotAnchor(WebGlRunFrame frame)
        => IsTruthy(frame.Metadata.GetValueOrDefault("snapshotAnchor")) ||
           string.Equals(frame.Metadata.GetValueOrDefault("replayMode"), WebGlRunBrowserReplayModes.SnapshotAnchorReplay, StringComparison.OrdinalIgnoreCase);

    private static bool IsTruthy(string? value)
        => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
}
