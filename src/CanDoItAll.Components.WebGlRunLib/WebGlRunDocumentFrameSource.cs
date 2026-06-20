namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocumentFrameSource(WebGlRunDocument document) : IWebGlRunFrameSource
{
    private readonly WebGlRunFrameResolver resolver = new();

    public WebGlRunDocument Document { get; } = document ?? throw new ArgumentNullException(nameof(document));

    public ValueTask<WebGlRunFrame?> GetFrameAsync(WebGlRunId runId, long frameIndex, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.IsNullOrWhiteSpace(runId.Value) &&
            !string.Equals(runId.Value, Document.RunId.Value, StringComparison.Ordinal))
        {
            return ValueTask.FromResult<WebGlRunFrame?>(null);
        }

        return ValueTask.FromResult(resolver.ResolveFrame(Document.Timeline, frameIndex));
    }
}
