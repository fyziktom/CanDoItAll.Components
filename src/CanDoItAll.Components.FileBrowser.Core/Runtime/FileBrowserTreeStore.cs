namespace CanDoItAll.Components.FileBrowser.Core;

/// <summary>Configures bounded retention for loaded browser state.</summary>
public sealed record FileBrowserTreeStoreOptions
{
    public FileBrowserTreeStoreOptions(int maximumContainers = 128, int maximumItems = 10_000)
    {
        if (maximumContainers < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumContainers));
        }

        if (maximumItems < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumItems));
        }

        MaximumContainers = maximumContainers;
        MaximumItems = maximumItems;
    }

    public int MaximumContainers { get; }

    public int MaximumItems { get; }
}

/// <summary>Determines whether an incoming page replaces or extends a query result.</summary>
public enum FileBrowserPageApplyMode
{
    Replace,
    Append
}

/// <summary>Identifies a container and one sort/filter/metadata query over it.</summary>
public readonly record struct FileBrowserContainerQueryKey(
    FileBrowserItemKey ParentKey,
    FileBrowserQueryFingerprint Fingerprint);

/// <summary>Immutable projection of one loaded container query.</summary>
public sealed record FileBrowserContainerSnapshot(
    FileBrowserContainerQueryKey QueryKey,
    IReadOnlyList<FileBrowserItem> Items,
    string? NextContinuationToken,
    long? TotalCount,
    string? ConsistencyToken,
    FileBrowserCompleteness Completeness,
    IReadOnlyList<FileBrowserPageWarning> Warnings,
    FileBrowserError? Error,
    int LoadedPageCount,
    DateTimeOffset LastAccessedAt)
{
    public bool IsComplete => Completeness == FileBrowserCompleteness.Complete
        && NextContinuationToken is null
        && Error is null;

    public bool HasMore => NextContinuationToken is not null;
}

/// <summary>Cache metrics exposed for telemetry and sandbox proof.</summary>
public sealed record FileBrowserTreeDiagnostics(
    int CachedItemCount,
    int CachedContainerQueryCount,
    int ProtectedItemCount,
    int EvictedContainerQueryCount);

/// <summary>Read-only loaded hierarchy access used by search strategies.</summary>
public interface IFileBrowserLoadedTree
{
    bool TryGetItem(FileBrowserItemKey key, out FileBrowserItem? item);

    IReadOnlyList<FileBrowserItem> GetLoadedChildren(FileBrowserItemKey parentKey);

    IReadOnlyList<FileBrowserItem> GetLoadedDescendants(FileBrowserItemKey parentKey);
}

/// <summary>
/// Stores shallow provider pages, deduplicates overlapping cursors, and bounds retained
/// container queries without performing provider I/O.
/// </summary>
public sealed class FileBrowserTreeStore : IFileBrowserLoadedTree
{
    private readonly object sync = new();
    private readonly FileBrowserTreeStoreOptions options;
    private readonly Dictionary<FileBrowserItemKey, FileBrowserItem> items = [];
    private readonly Dictionary<FileBrowserItemKey, long> itemAccess = [];
    private readonly Dictionary<FileBrowserContainerQueryKey, ContainerEntry> containers = [];
    private readonly HashSet<FileBrowserItemKey> protectedPath = [];
    private long accessSequence;
    private int evictedContainers;

    public FileBrowserTreeStore(FileBrowserTreeStoreOptions? options = null)
    {
        this.options = options ?? new FileBrowserTreeStoreOptions();
    }

    public void Upsert(FileBrowserItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        lock (sync)
        {
            UpsertCore(item);
            TrimCore();
        }
    }

    public void UpsertPath(IReadOnlyList<FileBrowserItem> path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.Count == 0)
        {
            throw new ArgumentException("A path must contain at least one item.", nameof(path));
        }

        var sourceId = path[0].Key.SourceId;
        if (path.Any(item => item.Key.SourceId != sourceId))
        {
            throw new ArgumentException("Every path item must belong to the same source.", nameof(path));
        }

        lock (sync)
        {
            foreach (var item in path)
            {
                UpsertCore(item);
            }

            TrimCore();
        }
    }

    public FileBrowserContainerSnapshot ApplyPage(
        FileBrowserBrowseRequest request,
        FileBrowserPage page,
        FileBrowserPageApplyMode mode)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(page);
        ValidatePageResponse(request, page);

        lock (sync)
        {
            var queryKey = CreateQueryKey(request);
            var hasLoadedQuery = containers.TryGetValue(queryKey, out var entry);
            if (mode == FileBrowserPageApplyMode.Replace)
            {
                entry = new ContainerEntry(queryKey);
                containers[queryKey] = entry;
            }
            else if (!hasLoadedQuery || entry is null)
            {
                throw new FileBrowserProviderException(new FileBrowserError(
                    FileBrowserErrorCode.StaleCursor,
                    "The first folder page is no longer cached; refresh before loading more.",
                    isRetryable: true));
            }
            else
            {
                ValidateAppendCursor(request, page, entry);
                FileBrowserProviderResponseValidator.ValidateCursorNotPreviouslyObserved(
                    page.NextContinuationToken,
                    entry.ObservedContinuationTokens);
            }

            long? acceptedTotalCount = mode == FileBrowserPageApplyMode.Append
                ? MergeTotalCount(
                    entry.TotalCount,
                    page.TotalCount,
                    entry.ItemKeys.Concat(page.Items.Select(item => item.Key)).Distinct().Count())
                : page.TotalCount;

            if (mode == FileBrowserPageApplyMode.Replace)
            {
                entry.ItemKeys.Clear();
                entry.LoadedPageCount = 0;
            }

            foreach (var item in page.Items)
            {
                UpsertCore(item);
                if (!entry.ItemKeys.Contains(item.Key))
                {
                    entry.ItemKeys.Add(item.Key);
                }
            }

            entry.NextContinuationToken = page.NextContinuationToken;
            entry.TotalCount = acceptedTotalCount;
            entry.ConsistencyToken = page.ConsistencyToken ?? request.ConsistencyToken ?? entry.ConsistencyToken;
            entry.Completeness = mode == FileBrowserPageApplyMode.Append
                ? FileBrowserProviderResponseValidator.MergeCompleteness(
                    entry.Completeness,
                    page.Completeness)
                : page.Completeness;
            entry.Warnings = mode == FileBrowserPageApplyMode.Append
                ? FileBrowserProviderResponseValidator.MergeWarnings(entry.Warnings, page.Warnings)
                : page.Warnings.ToArray();
            if (mode == FileBrowserPageApplyMode.Replace)
            {
                entry.ObservedContinuationTokens.Clear();
            }

            if (page.NextContinuationToken is not null)
            {
                entry.ObservedContinuationTokens.Add(page.NextContinuationToken);
            }

            entry.Error = null;
            entry.LoadedPageCount++;
            entry.LastAccess = NextAccess();
            entry.LastAccessedAt = DateTimeOffset.UtcNow;
            TrimCore();
            return SnapshotCore(entry);
        }
    }

    public FileBrowserContainerSnapshot RecordError(
        FileBrowserBrowseRequest request,
        FileBrowserError error)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(error);
        lock (sync)
        {
            var queryKey = CreateQueryKey(request);
            if (!containers.TryGetValue(queryKey, out var entry))
            {
                entry = new ContainerEntry(queryKey);
                containers.Add(queryKey, entry);
            }

            entry.Error = error;
            entry.LastAccess = NextAccess();
            entry.LastAccessedAt = DateTimeOffset.UtcNow;
            TrimCore();
            return SnapshotCore(entry);
        }
    }

    public bool TryGetContainer(
        FileBrowserBrowseRequest request,
        out FileBrowserContainerSnapshot? snapshot)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (sync)
        {
            if (!containers.TryGetValue(CreateQueryKey(request), out var entry))
            {
                snapshot = null;
                return false;
            }

            entry.LastAccess = NextAccess();
            entry.LastAccessedAt = DateTimeOffset.UtcNow;
            snapshot = SnapshotCore(entry);
            return true;
        }
    }

    public bool TryGetItem(FileBrowserItemKey key, out FileBrowserItem? item)
    {
        lock (sync)
        {
            if (!items.TryGetValue(key, out item))
            {
                return false;
            }

            itemAccess[key] = NextAccess();
            return true;
        }
    }

    public IReadOnlyList<FileBrowserItem> GetLoadedChildren(FileBrowserItemKey parentKey)
    {
        lock (sync)
        {
            var result = containers.Values
                .Where(entry => entry.QueryKey.ParentKey == parentKey)
                .SelectMany(entry => entry.ItemKeys)
                .Distinct()
                .Select(key => items.GetValueOrDefault(key))
                .Where(static item => item is not null)
                .Cast<FileBrowserItem>()
                .ToArray();
            TouchItems(result);
            return result;
        }
    }

    public IReadOnlyList<FileBrowserItem> GetLoadedDescendants(FileBrowserItemKey parentKey)
    {
        lock (sync)
        {
            var visited = new HashSet<FileBrowserItemKey> { parentKey };
            var queue = new Queue<FileBrowserItemKey>();
            var result = new List<FileBrowserItem>();
            queue.Enqueue(parentKey);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var childKey in containers.Values
                             .Where(entry => entry.QueryKey.ParentKey == current)
                             .SelectMany(entry => entry.ItemKeys)
                             .Distinct())
                {
                    if (!visited.Add(childKey) || !items.TryGetValue(childKey, out var child))
                    {
                        continue;
                    }

                    result.Add(child);
                    if (child.IsContainer)
                    {
                        queue.Enqueue(childKey);
                    }
                }
            }

            TouchItems(result);
            return result;
        }
    }

    public void SetProtectedPath(IEnumerable<FileBrowserItemKey> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        lock (sync)
        {
            protectedPath.Clear();
            protectedPath.UnionWith(keys);
            TrimCore();
        }
    }

    public void Invalidate(FileBrowserItemKey parentKey)
    {
        lock (sync)
        {
            foreach (var key in containers.Keys.Where(key => key.ParentKey == parentKey).ToArray())
            {
                containers.Remove(key);
            }

            PruneItemsCore();
        }
    }

    public void ClearSource(FileBrowserSourceId sourceId)
    {
        lock (sync)
        {
            foreach (var key in containers.Keys.Where(key => key.ParentKey.SourceId == sourceId).ToArray())
            {
                containers.Remove(key);
            }

            foreach (var key in items.Keys.Where(key => key.SourceId == sourceId).ToArray())
            {
                items.Remove(key);
                itemAccess.Remove(key);
            }

            protectedPath.RemoveWhere(key => key.SourceId == sourceId);
        }
    }

    public FileBrowserTreeDiagnostics GetDiagnostics()
    {
        lock (sync)
        {
            return new FileBrowserTreeDiagnostics(items.Count, containers.Count, protectedPath.Count, evictedContainers);
        }
    }

    private static FileBrowserContainerQueryKey CreateQueryKey(FileBrowserBrowseRequest request)
        => new(request.ParentKey, FileBrowserQueryFingerprint.From(request));

    private void UpsertCore(FileBrowserItem item)
    {
        items[item.Key] = item;
        itemAccess[item.Key] = NextAccess();
    }

    internal static void ValidatePageResponse(FileBrowserBrowseRequest request, FileBrowserPage page)
        => FileBrowserProviderResponseValidator.ValidateBrowsePage(request, page);

    private static void ValidateAppendCursor(
        FileBrowserBrowseRequest request,
        FileBrowserPage page,
        ContainerEntry entry)
    {
        if (request.ContinuationToken is null)
        {
            throw new InvalidOperationException("Appending requires a continuation token.");
        }

        if (!string.Equals(entry.NextContinuationToken, request.ContinuationToken, StringComparison.Ordinal))
        {
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.StaleCursor,
                "The continuation token no longer matches the loaded folder state.",
                isRetryable: true));
        }

        if (entry.ConsistencyToken is not null
            && page.ConsistencyToken is not null
            && !string.Equals(entry.ConsistencyToken, page.ConsistencyToken, StringComparison.Ordinal))
        {
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.StaleCursor,
                "The source revision changed while loading this folder.",
                isRetryable: true));
        }
    }

    private static long? MergeTotalCount(long? existing, long? incoming, int accumulatedItemCount)
    {
        if (incoming.HasValue && incoming.Value < accumulatedItemCount)
        {
            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                "The provider returned a total count smaller than the accumulated result.");
        }

        if (existing.HasValue && incoming.HasValue && existing.Value != incoming.Value)
        {
            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                "The provider changed the total count while paging a stable result.");
        }

        return incoming ?? existing;
    }

    private FileBrowserContainerSnapshot SnapshotCore(ContainerEntry entry)
    {
        var loadedItems = entry.ItemKeys
            .Select(key => items.GetValueOrDefault(key))
            .Where(static item => item is not null)
            .Cast<FileBrowserItem>()
            .ToArray();
        TouchItems(loadedItems);
        return new FileBrowserContainerSnapshot(
            entry.QueryKey,
            loadedItems,
            entry.NextContinuationToken,
            entry.TotalCount,
            entry.ConsistencyToken,
            entry.Completeness,
            entry.Warnings,
            entry.Error,
            entry.LoadedPageCount,
            entry.LastAccessedAt);
    }

    private void TouchItems(IEnumerable<FileBrowserItem> loadedItems)
    {
        foreach (var item in loadedItems)
        {
            itemAccess[item.Key] = NextAccess();
        }
    }

    private long NextAccess() => ++accessSequence;

    private void TrimCore()
    {
        while (containers.Count > options.MaximumContainers)
        {
            if (!TryEvictLeastRecentlyUsedContainerCore())
            {
                break;
            }
        }

        PruneItemsCore();
        while (items.Count > options.MaximumItems)
        {
            if (!TryEvictLeastRecentlyUsedContainerCore())
            {
                // A protected navigation path may legitimately exceed a very small budget.
                break;
            }

            PruneItemsCore();
        }
    }

    private bool TryEvictLeastRecentlyUsedContainerCore()
    {
        var candidate = containers.Values
            .Where(entry => !protectedPath.Contains(entry.QueryKey.ParentKey))
            .OrderBy(entry => entry.LastAccess)
            .FirstOrDefault();
        if (candidate is null)
        {
            return false;
        }

        containers.Remove(candidate.QueryKey);
        evictedContainers++;
        return true;
    }

    private void PruneItemsCore()
    {
        if (items.Count <= options.MaximumItems)
        {
            return;
        }

        var referenced = new HashSet<FileBrowserItemKey>(protectedPath);
        foreach (var entry in containers.Values)
        {
            referenced.Add(entry.QueryKey.ParentKey);
            referenced.UnionWith(entry.ItemKeys);
        }

        foreach (var key in items.Keys
                     .Where(key => !referenced.Contains(key))
                     .OrderBy(key => itemAccess.GetValueOrDefault(key))
                     .Take(items.Count - options.MaximumItems)
                     .ToArray())
        {
            items.Remove(key);
            itemAccess.Remove(key);
        }
    }

    private sealed class ContainerEntry(FileBrowserContainerQueryKey queryKey)
    {
        public FileBrowserContainerQueryKey QueryKey { get; } = queryKey;
        public List<FileBrowserItemKey> ItemKeys { get; } = [];
        public string? NextContinuationToken { get; set; }
        public long? TotalCount { get; set; }
        public string? ConsistencyToken { get; set; }
        public FileBrowserCompleteness Completeness { get; set; } = FileBrowserCompleteness.Unknown;
        public IReadOnlyList<FileBrowserPageWarning> Warnings { get; set; } = [];
        public HashSet<string> ObservedContinuationTokens { get; } = new(StringComparer.Ordinal);
        public FileBrowserError? Error { get; set; }
        public int LoadedPageCount { get; set; }
        public long LastAccess { get; set; }
        public DateTimeOffset LastAccessedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
