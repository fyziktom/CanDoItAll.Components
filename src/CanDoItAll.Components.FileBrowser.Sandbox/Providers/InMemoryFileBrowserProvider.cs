using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Sandbox.Providers;

/// <summary>
/// Deterministic shallow provider used by the sandbox to prove paging, recursive project
/// browsing, native search, and occurrence-versus-content identity without external services.
/// </summary>
internal sealed class InMemoryFileBrowserProvider :
    IFileBrowserProvider,
    IFileBrowserSearchProvider,
    IFileBrowserActionProvider
{
    private readonly IReadOnlyDictionary<FileBrowserItemKey, FileBrowserItem> items;
    private readonly IReadOnlyDictionary<FileBrowserItemKey, IReadOnlyList<FileBrowserItem>> children;
    private readonly FileBrowserItem root;
    private readonly TimeSpan simulatedLatency;
    private int requestCount;

    public InMemoryFileBrowserProvider(
        FileBrowserSourceDescriptor descriptor,
        IEnumerable<FileBrowserItem> items,
        TimeSpan? simulatedLatency = null)
    {
        Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        ArgumentNullException.ThrowIfNull(items);

        var materialized = items.ToArray();
        if (materialized.Length == 0 || materialized.Any(item => item.Key.SourceId != descriptor.Id))
        {
            throw new ArgumentException("Every demo item must belong to the provider source.", nameof(items));
        }

        this.items = materialized.ToDictionary(item => item.Key);
        var roots = materialized.Where(item => item.ParentKey is null).ToArray();
        if (roots.Length != 1 || !roots[0].IsContainer)
        {
            throw new ArgumentException("A demo provider requires exactly one container root.", nameof(items));
        }

        root = roots[0];
        children = materialized
            .Where(item => item.ParentKey.HasValue)
            .GroupBy(item => item.ParentKey!.Value)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<FileBrowserItem>)group.ToArray());
        this.simulatedLatency = simulatedLatency ?? TimeSpan.FromMilliseconds(120);
    }

    public FileBrowserSourceDescriptor Descriptor { get; }

    public int RequestCount => Volatile.Read(ref requestCount);

    public async ValueTask<FileBrowserItem> GetRootAsync(
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        await SimulateRequestAsync(cancellationToken);
        return root;
    }

    public async ValueTask<IReadOnlyList<FileBrowserItem>> GetPathAsync(
        FileBrowserItemKey itemKey,
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        await SimulateRequestAsync(cancellationToken);
        var current = RequireItem(itemKey);
        if (!current.IsContainer)
        {
            throw ProviderError(FileBrowserErrorCode.InvalidLocation, "Only folders can be used as browser locations.");
        }

        var path = new Stack<FileBrowserItem>();
        var visited = new HashSet<FileBrowserItemKey>();
        while (true)
        {
            if (!visited.Add(current.Key))
            {
                throw ProviderError(FileBrowserErrorCode.CorruptProviderResponse, "The demo hierarchy contains a parent cycle.");
            }

            path.Push(current);
            if (current.ParentKey is not { } parentKey)
            {
                break;
            }

            current = RequireItem(parentKey);
        }

        return path.ToArray();
    }

    public async ValueTask<FileBrowserPage> BrowseAsync(
        FileBrowserBrowseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateSource(request.ParentKey);
        await SimulateRequestAsync(cancellationToken);

        var parent = RequireItem(request.ParentKey);
        if (!parent.IsContainer)
        {
            throw ProviderError(FileBrowserErrorCode.InvalidLocation, "The requested browser location is not a folder.");
        }

        if (request.IncludeDescendants
            && !Descriptor.Supports(FileBrowserSourceCapabilities.RecursiveBrowse))
        {
            throw ProviderError(FileBrowserErrorCode.Unsupported, "This source does not expose recursive project listing.");
        }

        var candidates = request.IncludeDescendants
            ? EnumerateDescendants(parent.Key)
            : GetChildren(parent.Key);
        var ordered = FileBrowserItemOrdering.Apply(
            candidates.Where(request.Filter.Matches),
            request.Sort);

        var offset = ParseOffset(request.ContinuationToken);
        var pageItems = ordered.Skip(offset).Take(request.PageSize).ToArray();
        var nextOffset = offset + pageItems.Length;
        return new FileBrowserPage(
            pageItems,
            nextOffset < ordered.Count ? $"offset:{nextOffset}" : null,
            ordered.Count,
            GetConsistencyToken(),
            FileBrowserCompleteness.Complete);
    }

    public async ValueTask<FileBrowserSearchPage> SearchAsync(
        FileBrowserSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateSource(request.ContainerKey);
        await SimulateRequestAsync(cancellationToken);

        if (request.Scope != FileBrowserSearchScope.Provider)
        {
            throw ProviderError(FileBrowserErrorCode.Unsupported, "The demo index is only used for provider-wide search.");
        }

        RequireItem(request.ContainerKey);
        var descendants = EnumerateDescendants(request.ContainerKey);
        var matching = descendants
            .Where(request.Filter.Matches)
            .Where(item => item.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                || item.DisplayPath?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) == true
                || item.ContentIdentity?.Value.Contains(request.Query, StringComparison.OrdinalIgnoreCase) == true);
        var ordered = FileBrowserItemOrdering.Apply(matching, request.Sort);
        var offset = ParseOffset(request.ContinuationToken);
        var pageItems = ordered.Skip(offset).Take(request.PageSize).ToArray();
        var nextOffset = offset + pageItems.Length;
        return new FileBrowserSearchPage(
            pageItems,
            "demo-native-index",
            nextOffset < ordered.Count ? $"offset:{nextOffset}" : null,
            ordered.Count,
            isPartial: false,
            scannedContainers: descendants.Count(item => item.IsContainer),
            scannedItems: descendants.Count,
            consistencyToken: GetConsistencyToken());
    }

    public ValueTask<IReadOnlyList<FileBrowserActionDescriptor>> GetActionsAsync(
        FileBrowserItemKey itemKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RequireItem(itemKey);
        return ValueTask.FromResult<IReadOnlyList<FileBrowserActionDescriptor>>([]);
    }

    public ValueTask<FileBrowserActionResult> ExecuteAsync(
        FileBrowserActionRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var item = RequireItem(request.ItemKey);
        var result = request.ActionId switch
        {
            FileBrowserActionIds.OpenInNewTab => FileBrowserActionResult.Success(
                $"Sandbox host accepted an open-in-new-tab request for {item.Name}."),
            FileBrowserActionIds.Download => FileBrowserActionResult.Success(
                $"Sandbox host prepared a bounded demo download for {item.Name}."),
            _ => FileBrowserActionResult.Failure(new FileBrowserError(
                FileBrowserErrorCode.Unsupported,
                $"The demo provider does not execute action '{request.ActionId}'."))
        };
        return ValueTask.FromResult(result);
    }

    private IReadOnlyList<FileBrowserItem> GetChildren(FileBrowserItemKey parentKey)
        => children.TryGetValue(parentKey, out var values) ? values : [];

    private IReadOnlyList<FileBrowserItem> EnumerateDescendants(FileBrowserItemKey parentKey)
    {
        var result = new List<FileBrowserItem>();
        var queue = new Queue<FileBrowserItemKey>();
        var visited = new HashSet<FileBrowserItemKey>();
        queue.Enqueue(parentKey);

        while (queue.TryDequeue(out var current))
        {
            if (!visited.Add(current))
            {
                continue;
            }

            foreach (var child in GetChildren(current))
            {
                result.Add(child);
                if (child.IsContainer)
                {
                    queue.Enqueue(child.Key);
                }
            }
        }

        return result;
    }

    private FileBrowserItem RequireItem(FileBrowserItemKey key)
    {
        ValidateSource(key);
        return items.TryGetValue(key, out var item)
            ? item
            : throw ProviderError(FileBrowserErrorCode.NotFound, "The requested demo item no longer exists.");
    }

    private void ValidateSource(FileBrowserItemKey key)
    {
        if (key.SourceId != Descriptor.Id)
        {
            throw ProviderError(FileBrowserErrorCode.InvalidLocation, "The item belongs to another browser source.");
        }
    }

    private async ValueTask SimulateRequestAsync(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref requestCount);
        await Task.Delay(simulatedLatency, cancellationToken);
    }

    private static int ParseOffset(string? continuationToken)
    {
        if (continuationToken is null)
        {
            return 0;
        }

        if (!continuationToken.StartsWith("offset:", StringComparison.Ordinal)
            || !int.TryParse(continuationToken.AsSpan("offset:".Length), out var offset)
            || offset < 0)
        {
            throw ProviderError(FileBrowserErrorCode.StaleCursor, "The paging cursor is no longer valid.", isRetryable: true);
        }

        return offset;
    }

    private string GetConsistencyToken() => $"{Descriptor.Id.Value}:demo-v1";

    private static FileBrowserProviderException ProviderError(
        FileBrowserErrorCode code,
        string message,
        bool isRetryable = false)
        => new(new FileBrowserError(code, message, isRetryable));
}
