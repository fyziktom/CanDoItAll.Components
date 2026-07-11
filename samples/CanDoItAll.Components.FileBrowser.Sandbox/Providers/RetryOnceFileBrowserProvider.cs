using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Sandbox.Providers;

/// <summary>Sandbox decorator that proves normalized retryable-error and retry behavior.</summary>
internal sealed class RetryOnceFileBrowserProvider(IFileBrowserProvider inner) : IFileBrowserProvider
{
    private int remainingFailures = 1;

    public FileBrowserSourceDescriptor Descriptor => inner.Descriptor;

    public ValueTask<FileBrowserItem> GetRootAsync(
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default)
        => inner.GetRootAsync(metadata, cancellationToken);

    public ValueTask<IReadOnlyList<FileBrowserItem>> GetPathAsync(
        FileBrowserItemKey itemKey,
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default)
        => inner.GetPathAsync(itemKey, metadata, cancellationToken);

    public async ValueTask<FileBrowserPage> BrowseAsync(
        FileBrowserBrowseRequest request,
        CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref remainingFailures, 0) == 1)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(260), cancellationToken);
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.Unavailable,
                "The demo source is temporarily unavailable. Retry to prove recovery without recreating the browser.",
                isRetryable: true,
                correlationId: "sandbox-retry-once"));
        }

        return await inner.BrowseAsync(request, cancellationToken);
    }
}

