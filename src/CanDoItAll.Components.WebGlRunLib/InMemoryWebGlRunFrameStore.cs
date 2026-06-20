namespace CanDoItAll.Components.WebGlRunLib;

public sealed class InMemoryWebGlRunFrameStore(WebGlRunTimeline timeline) : IWebGlRunFrameStore
{
    private readonly IReadOnlyList<WebGlRunFrame> frames = WebGlRunFrameResolver.OrderFrames(timeline.Frames).ToList();

    public ValueTask<WebGlRunFrame?> GetFrameAsync(WebGlRunId runId, long frameIndex, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(frames.FirstOrDefault(frame => frame.Index == frameIndex));
    }

    public ValueTask<IReadOnlyList<WebGlRunFrame>> ListFramesAsync(WebGlRunId runId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(frames);
    }
}
