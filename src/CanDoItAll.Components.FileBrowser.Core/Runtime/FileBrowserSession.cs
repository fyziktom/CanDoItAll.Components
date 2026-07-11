namespace CanDoItAll.Components.FileBrowser.Core;

/// <summary>
/// Coordinates providers, the lazy tree store, navigation, search strategies, and immutable
/// snapshots. Construction and provider registration remain outside this runtime behavior.
/// </summary>
public sealed class FileBrowserSession : IFileBrowserSession
{
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private readonly object lifecycleSync = new();
    private readonly object selectionSync = new();
    private readonly CancellationTokenSource lifetimeCancellation = new();
    private readonly IFileBrowserProviderCatalog providerCatalog;
    private readonly FileBrowserSearchStrategyCatalog searchStrategies;
    private readonly FileBrowserSessionOptions options;
    private readonly FileBrowserTreeStore treeStore;
    private readonly FileBrowserNavigationState navigation = new();
    private readonly HashSet<FileBrowserItemKey> selectedKeys = [];
    private readonly HashSet<string> searchContinuationTokens = new(StringComparer.Ordinal);
    private IFileBrowserProvider? currentProvider;
    private FileBrowserContainerSnapshot? currentContainer;
    private FileBrowserBrowseRequest? currentBrowseRequest;
    private FileBrowserSearchRequest? currentSearchRequest;
    private FileBrowserSearchPage? currentSearchPage;
    private IReadOnlyList<FileBrowserItem> visibleItems = [];
    private FileBrowserSortDescriptor sort;
    private FileBrowserFilter filter = FileBrowserFilter.None;
    private bool includeDescendants;
    private FileBrowserOperationKind operation;
    private FileBrowserError? error;
    private IReadOnlyList<FileBrowserPageWarning> warnings = [];
    private RetryCommand? retryCommand;
    private long revision;
    private int pendingExecutions;
    private bool disposalStarted;
    private TaskCompletionSource? executionsDrained;
    private Task? disposalTask;

    public FileBrowserSession(
        IFileBrowserProviderCatalog providerCatalog,
        FileBrowserSearchStrategyCatalog? searchStrategies = null,
        FileBrowserSessionOptions? options = null)
    {
        this.providerCatalog = providerCatalog ?? throw new ArgumentNullException(nameof(providerCatalog));
        this.searchStrategies = searchStrategies ?? FileBrowserSearchStrategyCatalog.CreateDefault();
        this.options = options ?? new FileBrowserSessionOptions();
        treeStore = new FileBrowserTreeStore(this.options.Cache);
        sort = this.options.DefaultSort;
        Snapshot = CreateSnapshot();
    }

    public FileBrowserSession(
        IEnumerable<IFileBrowserProvider> providers,
        FileBrowserSessionOptions? options = null)
        : this(new FileBrowserProviderCatalog(providers), options: options)
    {
    }

    public event EventHandler<FileBrowserSnapshotChangedEventArgs>? Changed;

    public FileBrowserSnapshot Snapshot { get; private set; }

    public ValueTask InitializeAsync(
        FileBrowserSourceId? sourceId = null,
        FileBrowserItemKey? startAt = null,
        CancellationToken cancellationToken = default)
    {
        var selectedSource = sourceId ?? providerCatalog.Sources[0].Id;
        if (startAt.HasValue && startAt.Value.SourceId != selectedSource)
        {
            throw new ArgumentException("The starting item must belong to the selected source.", nameof(startAt));
        }

        return ExecuteAsync(
            FileBrowserOperationKind.Initializing,
            token => InitializeCoreAsync(selectedSource, startAt, token),
            cancellationToken);
    }

    public ValueTask ChangeSourceAsync(
        FileBrowserSourceId sourceId,
        CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.Initializing,
            token => InitializeCoreAsync(sourceId, null, token),
            cancellationToken);

    public ValueTask NavigateAsync(
        FileBrowserItemKey containerKey,
        CancellationToken cancellationToken = default)
    {
        if (!providerCatalog.TryGet(containerKey.SourceId, out _))
        {
            throw new KeyNotFoundException($"File browser source '{containerKey.SourceId}' is not registered.");
        }

        return ExecuteAsync(
            FileBrowserOperationKind.LoadingFolder,
            token => NavigateCoreAsync(containerKey, token),
            cancellationToken);
    }

    public ValueTask GoBackAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.LoadingFolder,
            async token =>
            {
                var location = navigation.PeekBack();
                var prepared = await PrepareLocationLoadAsync(
                    RequireProvider(),
                    location,
                    force: false,
                    sort,
                    filter,
                    includeDescendants,
                    token);
                token.ThrowIfCancellationRequested();
                CommitPreparedBrowse(prepared);
                navigation.GoBack();
                ClearSearchCore();
                ClearSelectionCore();
            },
            cancellationToken);

    public ValueTask GoForwardAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.LoadingFolder,
            async token =>
            {
                var location = navigation.PeekForward();
                var prepared = await PrepareLocationLoadAsync(
                    RequireProvider(),
                    location,
                    force: false,
                    sort,
                    filter,
                    includeDescendants,
                    token);
                token.ThrowIfCancellationRequested();
                CommitPreparedBrowse(prepared);
                navigation.GoForward();
                ClearSearchCore();
                ClearSelectionCore();
            },
            cancellationToken);

    public ValueTask GoUpAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.LoadingFolder,
            async token =>
            {
                var location = navigation.PeekUp();
                var prepared = await PrepareLocationLoadAsync(
                    RequireProvider(),
                    location,
                    force: false,
                    sort,
                    filter,
                    includeDescendants,
                    token);
                token.ThrowIfCancellationRequested();
                CommitPreparedBrowse(prepared);
                navigation.GoUp();
                ClearSearchCore();
                ClearSelectionCore();
            },
            cancellationToken);

    public ValueTask LoadMoreAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.LoadingMore,
            LoadMoreCoreAsync,
            cancellationToken);

    public ValueTask RefreshAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.Refreshing,
            async token =>
            {
                if (currentSearchRequest is not null)
                {
                    await SearchCoreAsync(currentSearchRequest.Query, currentSearchRequest.Scope, token);
                    return;
                }

                var location = RequireLocation();
                await LoadLocationCoreAsync(location, force: true, token);
            },
            cancellationToken);

    public ValueTask SetSortAsync(
        FileBrowserSortDescriptor sort,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sort);
        return ExecuteAsync(
            FileBrowserOperationKind.Refreshing,
            async token =>
            {
                ValidateSort(sort);
                this.sort = sort;
                await ReloadCurrentModeAsync(token);
            },
            cancellationToken);
    }

    public ValueTask SetFilterAsync(
        FileBrowserFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return ExecuteAsync(
            FileBrowserOperationKind.Refreshing,
            async token =>
            {
                this.filter = filter;
                await ReloadCurrentModeAsync(token);
            },
            cancellationToken);
    }

    public ValueTask SetIncludeDescendantsAsync(
        bool includeDescendants,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(
            FileBrowserOperationKind.Refreshing,
            async token =>
            {
                if (includeDescendants
                    && !RequireProvider().Descriptor.Supports(
                        FileBrowserSourceCapabilities.RecursiveBrowse))
                {
                    throw new FileBrowserProviderException(new FileBrowserError(
                        FileBrowserErrorCode.Unsupported,
                        "This source does not support recursive folder listing."));
                }

                this.includeDescendants = includeDescendants;
                ClearSearchCore();
                await LoadLocationCoreAsync(RequireLocation(), force: false, token);
            },
            cancellationToken);
    }

    public ValueTask SearchAsync(
        string query,
        FileBrowserSearchScope scope,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return ClearSearchAsync(cancellationToken);
        }

        return ExecuteAsync(
            FileBrowserOperationKind.Searching,
            token => SearchCoreAsync(query, scope, token),
            cancellationToken);
    }

    public ValueTask ClearSearchAsync(CancellationToken cancellationToken = default)
        => ExecuteAsync(
            FileBrowserOperationKind.LoadingFolder,
            async token =>
            {
                ClearSearchCore();
                await RestoreBrowseResultAsync(token);
            },
            cancellationToken);

    public ValueTask RetryAsync(CancellationToken cancellationToken = default)
        => ExecuteCommandAsync(null, retryRequested: true, cancellationToken);

    public ValueTask<IReadOnlyList<FileBrowserActionDescriptor>> GetActionsAsync(
        FileBrowserItemKey itemKey,
        CancellationToken cancellationToken = default)
        => ExecuteSerializedAsync(
            async token =>
            {
                var (provider, item) = ResolveCachedItem(itemKey);
                var actions = new List<FileBrowserActionDescriptor>(
                    FileBrowserBuiltInActions.GetFor(item));
                if (provider is IFileBrowserActionProvider actionProvider
                    && provider.Descriptor.Supports(FileBrowserSourceCapabilities.CustomActions)
                    && item.Supports(FileBrowserItemCapabilities.CustomActions))
                {
                    var customActions = await actionProvider.GetActionsAsync(itemKey, token);
                    token.ThrowIfCancellationRequested();
                    if (customActions is null || customActions.Any(action => action is null))
                    {
                        throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                            "The provider returned an invalid custom action list.");
                    }

                    var actionIds = actions.Select(action => action.Id).ToHashSet(StringComparer.Ordinal);
                    foreach (var customAction in customActions)
                    {
                        if (FileBrowserActionIds.IsReserved(customAction.Id)
                            && !actionIds.Contains(customAction.Id))
                        {
                            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                                $"The provider returned reserved action '{customAction.Id}' for an item that does not support it.");
                        }

                        if (actionIds.Add(customAction.Id))
                        {
                            actions.Add(customAction);
                        }
                    }
                }

                return (IReadOnlyList<FileBrowserActionDescriptor>)actions.ToArray();
            },
            cancellationToken);

    public async ValueTask<FileBrowserActionResult> ExecuteActionAsync(
        FileBrowserActionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            var dispatch = await ExecuteSerializedAsync(
                token => DispatchActionAsync(request, token),
                cancellationToken);
            if (dispatch.NavigationKey is not { } navigationKey)
            {
                return dispatch.Result!;
            }

            await NavigateAsync(navigationKey, cancellationToken);
            return Snapshot.CurrentContainer?.Key == navigationKey && Snapshot.Error is null
                ? FileBrowserActionResult.Success()
                : FileBrowserActionResult.Failure(Snapshot.Error ?? new FileBrowserError(
                    FileBrowserErrorCode.InvalidOperation,
                    "The folder could not be opened."));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ObjectDisposedException)
        {
            throw;
        }
        catch (FileBrowserProviderException exception)
        {
            return FileBrowserActionResult.Failure(exception.Error);
        }
        catch (Exception exception)
        {
            return FileBrowserActionResult.Failure(new FileBrowserError(
                FileBrowserErrorCode.ProviderFailure,
                "The source could not complete the file browser action.",
                isRetryable: true,
                technicalDetail: exception.ToString()));
        }
    }

    public ValueTask<FileBrowserContentLease> OpenReadAsync(
        FileBrowserReadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteSerializedAsync(
            async token =>
            {
                var (provider, _) = ResolveCachedItem(request.ItemKey);
                if (!provider.Descriptor.Supports(FileBrowserSourceCapabilities.ContentRead)
                    || provider is not IFileBrowserContentProvider contentProvider)
                {
                    throw Unsupported("This source does not provide file content reads.");
                }

                if ((request.Offset > 0 || request.Length.HasValue)
                    && !provider.Descriptor.Supports(FileBrowserSourceCapabilities.RangeRead))
                {
                    throw Unsupported("This source does not provide ranged file content reads.");
                }

                var lease = await contentProvider.OpenReadAsync(request, token);
                if (lease is null)
                {
                    throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                        "The provider returned no content lease.");
                }

                if (!lease.Stream.CanRead)
                {
                    await lease.DisposeAsync();
                    throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                        "The provider returned a content stream that cannot be read.");
                }

                if (token.IsCancellationRequested)
                {
                    await lease.DisposeAsync();
                    token.ThrowIfCancellationRequested();
                }

                return lease;
            },
            cancellationToken);
    }

    public void Select(FileBrowserItemKey itemKey, bool toggle = false)
    {
        ThrowIfDisposed();
        if (!visibleItems.Any(item => item.Key == itemKey))
        {
            throw new ArgumentException("Only a currently visible item can be selected.", nameof(itemKey));
        }

        lock (selectionSync)
        {
            if (toggle)
            {
                if (!selectedKeys.Add(itemKey))
                {
                    selectedKeys.Remove(itemKey);
                }
            }
            else
            {
                selectedKeys.Clear();
                selectedKeys.Add(itemKey);
            }
        }

        Publish();
    }

    public void ClearSelection()
    {
        ThrowIfDisposed();
        lock (selectionSync)
        {
            if (selectedKeys.Count == 0)
            {
                return;
            }

            selectedKeys.Clear();
        }

        Publish();
    }

    private async ValueTask InitializeCoreAsync(
        FileBrowserSourceId sourceId,
        FileBrowserItemKey? startAt,
        CancellationToken cancellationToken)
    {
        var provider = providerCatalog.Get(sourceId);

        IReadOnlyList<FileBrowserItem> path;
        if (startAt.HasValue)
        {
            path = await provider.GetPathAsync(startAt.Value, options.Metadata, cancellationToken);
        }
        else
        {
            var root = await provider.GetRootAsync(options.Metadata, cancellationToken);
            path = [root];
        }

        ValidatePath(provider, path, startAt);
        var location = new FileBrowserLocation(path);
        var prepared = await PrepareLocationLoadAsync(
            provider,
            location,
            force: false,
            options.DefaultSort,
            FileBrowserFilter.None,
            requestedIncludeDescendants: false,
            cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        CommitPreparedBrowse(prepared);
        currentProvider = provider;
        sort = options.DefaultSort;
        filter = FileBrowserFilter.None;
        includeDescendants = false;
        ClearSearchCore();
        ClearSelectionCore();
        navigation.Reset(location);
    }

    private async ValueTask NavigateCoreAsync(
        FileBrowserItemKey containerKey,
        CancellationToken cancellationToken)
    {
        var provider = providerCatalog.Get(containerKey.SourceId);
        if (currentProvider?.Descriptor.Id != provider.Descriptor.Id)
        {
            await InitializeCoreAsync(containerKey.SourceId, containerKey, cancellationToken);
            return;
        }

        var path = await provider.GetPathAsync(containerKey, options.Metadata, cancellationToken);
        ValidatePath(provider, path, containerKey);
        var location = new FileBrowserLocation(path);
        var prepared = await PrepareLocationLoadAsync(
            provider,
            location,
            force: false,
            sort,
            filter,
            includeDescendants,
            cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        CommitPreparedBrowse(prepared);
        navigation.Navigate(location);
        ClearSearchCore();
        ClearSelectionCore();
    }

    private async ValueTask LoadLocationCoreAsync(
        FileBrowserLocation location,
        bool force,
        CancellationToken cancellationToken)
    {
        var provider = RequireProvider();
        var prepared = await PrepareLocationLoadAsync(
            provider,
            location,
            force,
            sort,
            filter,
            includeDescendants,
            cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        CommitPreparedBrowse(prepared);
    }

    private async ValueTask<PreparedBrowse> PrepareLocationLoadAsync(
        IFileBrowserProvider provider,
        FileBrowserLocation location,
        bool force,
        FileBrowserSortDescriptor requestedSort,
        FileBrowserFilter requestedFilter,
        bool requestedIncludeDescendants,
        CancellationToken cancellationToken)
    {
        var request = CreateBrowseRequest(
            provider,
            location.Key,
            requestedSort,
            requestedFilter,
            requestedIncludeDescendants);
        if (!force
            && treeStore.TryGetContainer(request, out var cached)
            && cached is not null
            && cached.LoadedPageCount > 0)
        {
            return new PreparedBrowse(location, request, cached, null);
        }

        var page = await provider.BrowseAsync(request, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        FileBrowserTreeStore.ValidatePageResponse(request, page);
        return new PreparedBrowse(location, request, null, page);
    }

    private void CommitPreparedBrowse(PreparedBrowse prepared)
    {
        treeStore.SetProtectedPath(prepared.Location.Path.Select(item => item.Key));
        treeStore.UpsertPath(prepared.Location.Path);
        currentBrowseRequest = prepared.Request.FirstPage();
        currentContainer = prepared.Cached
            ?? treeStore.ApplyPage(prepared.Request, prepared.Page!, FileBrowserPageApplyMode.Replace);
        SetBrowseResult(currentContainer);
    }

    private async ValueTask LoadMoreCoreAsync(CancellationToken cancellationToken)
    {
        if (currentSearchRequest is not null && currentSearchPage is not null)
        {
            if (currentSearchPage.NextContinuationToken is null)
            {
                return;
            }

            var provider = RequireProvider();
            var strategy = searchStrategies.Get(currentSearchRequest.Scope, provider);
            var searchNextRequest = currentSearchRequest.Next(
                currentSearchPage.NextContinuationToken,
                currentSearchPage.ConsistencyToken);
            var page = await strategy.SearchAsync(
                new FileBrowserSearchStrategyContext(provider, new SearchData(this), searchNextRequest),
                cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var mergedPage = MergeSearchPage(page);
            currentSearchRequest = searchNextRequest;
            currentSearchPage = mergedPage;
            visibleItems = mergedPage.Items;
            warnings = mergedPage.Warnings;
            RegisterSearchContinuation(page.NextContinuationToken);
            return;
        }

        if (currentBrowseRequest is null
            || currentContainer?.NextContinuationToken is not { } continuationToken)
        {
            return;
        }

        var browseNextRequest = currentBrowseRequest.Next(continuationToken, currentContainer.ConsistencyToken);
        var pageResult = await RequireProvider().BrowseAsync(browseNextRequest, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        currentContainer = treeStore.ApplyPage(browseNextRequest, pageResult, FileBrowserPageApplyMode.Append);
        currentBrowseRequest = currentBrowseRequest.FirstPage();
        SetBrowseResult(currentContainer, reconcileSelection: false);
    }

    private async ValueTask ReloadCurrentModeAsync(CancellationToken cancellationToken)
    {
        if (currentSearchRequest is not null)
        {
            await SearchCoreAsync(currentSearchRequest.Query, currentSearchRequest.Scope, cancellationToken);
            return;
        }

        await LoadLocationCoreAsync(RequireLocation(), force: false, cancellationToken);
    }

    private async ValueTask SearchCoreAsync(
        string query,
        FileBrowserSearchScope scope,
        CancellationToken cancellationToken)
    {
        var provider = RequireProvider();
        var strategy = searchStrategies.Get(scope, provider);
        var request = new FileBrowserSearchRequest(
            RequireLocation().Key,
            query,
            scope,
            Math.Min(options.PageSize, provider.Descriptor.MaximumPageSize),
            sort: sort,
            filter: filter,
            consistencyToken: currentContainer?.ConsistencyToken,
            metadata: options.Metadata);
        var page = await strategy.SearchAsync(
            new FileBrowserSearchStrategyContext(provider, new SearchData(this), request),
            cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        currentSearchRequest = request;
        currentSearchPage = page;
        visibleItems = page.Items;
        warnings = page.Warnings;
        searchContinuationTokens.Clear();
        RegisterSearchContinuation(page.NextContinuationToken);
        ReconcileSelection();
    }

    private async ValueTask RestoreBrowseResultAsync(CancellationToken cancellationToken)
    {
        if (currentBrowseRequest is not null
            && treeStore.TryGetContainer(currentBrowseRequest, out var cached)
            && cached is not null
            && cached.LoadedPageCount > 0)
        {
            SetBrowseResult(cached);
            return;
        }

        await LoadLocationCoreAsync(RequireLocation(), force: false, cancellationToken);
    }

    private FileBrowserBrowseRequest CreateBrowseRequest(
        IFileBrowserProvider provider,
        FileBrowserItemKey parentKey,
        FileBrowserSortDescriptor requestedSort,
        FileBrowserFilter requestedFilter,
        bool requestedIncludeDescendants)
    {
        return new FileBrowserBrowseRequest(
            parentKey,
            Math.Min(options.PageSize, provider.Descriptor.MaximumPageSize),
            sort: requestedSort,
            filter: requestedFilter,
            includeDescendants: requestedIncludeDescendants,
            consistencyToken: parentKey.Revision,
            metadata: options.Metadata);
    }

    private void SetBrowseResult(FileBrowserContainerSnapshot container, bool reconcileSelection = true)
    {
        currentContainer = container;
        visibleItems = container.Items;
        warnings = container.Warnings;
        error = container.Error;
        if (reconcileSelection)
        {
            ReconcileSelection();
        }
    }

    private void ClearSearchCore()
    {
        currentSearchRequest = null;
        currentSearchPage = null;
        searchContinuationTokens.Clear();
    }

    private FileBrowserSearchPage MergeSearchPage(FileBrowserSearchPage incoming)
    {
        var existing = currentSearchPage
            ?? throw new InvalidOperationException("A search page cannot be appended before its first page.");
        if (!string.Equals(existing.StrategyId, incoming.StrategyId, StringComparison.Ordinal))
        {
            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                "The provider changed search strategy identifiers while paging one result.");
        }

        FileBrowserProviderResponseValidator.ValidateCursorNotPreviouslyObserved(
            incoming.NextContinuationToken,
            searchContinuationTokens);
        FileBrowserProviderResponseValidator.ValidateNoConflictingOverlaps(
            visibleItems,
            incoming.Items);

        var mergedItems = visibleItems.ToList();
        var observedKeys = mergedItems.Select(item => item.Key).ToHashSet();
        mergedItems.AddRange(incoming.Items.Where(item => observedKeys.Add(item.Key)));

        if (existing.TotalCount.HasValue
            && incoming.TotalCount.HasValue
            && existing.TotalCount.Value != incoming.TotalCount.Value)
        {
            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                "The provider changed the total count while paging a stable search result.");
        }

        long? totalCount = incoming.TotalCount ?? existing.TotalCount;
        if (totalCount.HasValue && totalCount.Value < mergedItems.Count)
        {
            throw FileBrowserProviderResponseValidator.CorruptProviderResponse(
                "The provider returned a total count smaller than the accumulated search result.");
        }

        return new FileBrowserSearchPage(
            mergedItems,
            existing.StrategyId,
            incoming.NextContinuationToken,
            totalCount,
            existing.IsPartial || incoming.IsPartial,
            Math.Max(existing.ScannedContainers, incoming.ScannedContainers),
            Math.Max(existing.ScannedItems, incoming.ScannedItems),
            incoming.ConsistencyToken ?? existing.ConsistencyToken,
            FileBrowserProviderResponseValidator.MergeWarnings(
                existing.Warnings,
                incoming.Warnings));
    }

    private void RegisterSearchContinuation(string? continuationToken)
    {
        if (continuationToken is not null)
        {
            searchContinuationTokens.Add(continuationToken);
        }
    }

    private void ClearSelectionCore()
    {
        lock (selectionSync)
        {
            selectedKeys.Clear();
        }
    }

    private void ReconcileSelection()
    {
        var visible = visibleItems.Select(item => item.Key).ToHashSet();
        lock (selectionSync)
        {
            selectedKeys.RemoveWhere(key => !visible.Contains(key));
        }
    }

    private void ValidateSort(FileBrowserSortDescriptor descriptor)
    {
        if (currentProvider is not null
            && !currentProvider.Descriptor.SupportedSortFields.Contains(descriptor.Field))
        {
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.Unsupported,
                $"This source cannot sort by {descriptor.Field}."));
        }
    }

    private static void ValidatePath(
        IFileBrowserProvider provider,
        IReadOnlyList<FileBrowserItem> path,
        FileBrowserItemKey? requestedKey)
    {
        if (path.Count == 0)
        {
            throw InvalidProviderPath();
        }

        var visited = new HashSet<FileBrowserItemKey>();
        for (var index = 0; index < path.Count; index++)
        {
            var item = path[index];
            FileBrowserItemKey? expectedParent = index == 0 ? null : path[index - 1].Key;
            if (item.Key.SourceId != provider.Descriptor.Id
                || !item.IsContainer
                || item.ParentKey != expectedParent
                || !visited.Add(item.Key))
            {
                throw InvalidProviderPath();
            }
        }

        if (requestedKey.HasValue && path[^1].Key != requestedKey.Value)
        {
            throw InvalidProviderPath();
        }
    }

    private static FileBrowserProviderException InvalidProviderPath()
        => new(new FileBrowserError(
            FileBrowserErrorCode.CorruptProviderResponse,
            "The provider returned an invalid browser path."));

    private async ValueTask<ActionDispatch> DispatchActionAsync(
        FileBrowserActionRequest request,
        CancellationToken cancellationToken)
    {
        var (provider, item) = ResolveCachedItem(request.ItemKey);
        switch (request.ActionId)
        {
            case FileBrowserActionIds.Open
                when item.IsContainer
                     && item.Supports(FileBrowserItemCapabilities.Navigate):
                return ActionDispatch.Navigate(item.Key);
            case FileBrowserActionIds.Open
                when item.Supports(FileBrowserItemCapabilities.Open)
                     && item.OpenUri is not null:
            case FileBrowserActionIds.OpenInNewTab
                when item.Supports(FileBrowserItemCapabilities.OpenInNewTab)
                     && item.OpenUri is not null:
                return ActionDispatch.Complete(FileBrowserActionResult.Success(
                    navigationUri: item.OpenUri));
            case FileBrowserActionIds.CopyPath
                when item.Supports(FileBrowserItemCapabilities.CopyPath)
                     && item.DisplayPath is not null:
                return ActionDispatch.Complete(FileBrowserActionResult.Success(
                    value: item.DisplayPath));
            case FileBrowserActionIds.CopyContentIdentity
                when item.Supports(FileBrowserItemCapabilities.CopyContentIdentity)
                     && item.ContentIdentity is not null:
                return ActionDispatch.Complete(FileBrowserActionResult.Success(
                    value: item.ContentIdentity.Value));
            case FileBrowserActionIds.Download
                when (item.Supports(FileBrowserItemCapabilities.DownloadFile)
                      || item.Supports(FileBrowserItemCapabilities.DownloadDirectory))
                     && item.DownloadUri is not null:
                return ActionDispatch.Complete(FileBrowserActionResult.Success(
                    navigationUri: item.DownloadUri));
        }

        var advertisedBuiltIn = FileBrowserBuiltInActions.GetFor(item)
            .Any(action => string.Equals(action.Id, request.ActionId, StringComparison.Ordinal));
        var advertisedCustom = !FileBrowserActionIds.IsReserved(request.ActionId)
            && provider.Descriptor.Supports(FileBrowserSourceCapabilities.CustomActions)
            && item.Supports(FileBrowserItemCapabilities.CustomActions);
        if (provider is IFileBrowserActionProvider actionProvider
            && (advertisedBuiltIn || advertisedCustom))
        {
            var result = await actionProvider.ExecuteAsync(request, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return ActionDispatch.Complete(result);
        }

        return ActionDispatch.Complete(FileBrowserActionResult.Failure(new FileBrowserError(
            FileBrowserErrorCode.Unsupported,
            $"The action '{request.ActionId}' is not supported for this item.")));
    }

    private (IFileBrowserProvider Provider, FileBrowserItem Item) ResolveCachedItem(
        FileBrowserItemKey itemKey)
    {
        var provider = currentProvider ?? throw new FileBrowserProviderException(new FileBrowserError(
            FileBrowserErrorCode.InvalidOperation,
            "The file browser session is not initialized."));
        if (itemKey.SourceId != provider.Descriptor.Id)
        {
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.InvalidLocation,
                "The item does not belong to the active file browser source."));
        }

        var item = visibleItems.FirstOrDefault(candidate => candidate.Key == itemKey);
        if (item is null && !treeStore.TryGetItem(itemKey, out item))
        {
            throw new FileBrowserProviderException(new FileBrowserError(
                FileBrowserErrorCode.NotFound,
                "The item is not loaded in this file browser session."));
        }

        return (provider, item!);
    }

    private static FileBrowserProviderException Unsupported(string message)
        => new(new FileBrowserError(FileBrowserErrorCode.Unsupported, message));

    private ValueTask ExecuteAsync(
        FileBrowserOperationKind operationKind,
        Func<CancellationToken, ValueTask> action,
        CancellationToken cancellationToken)
        => ExecuteCommandAsync(
            new RetryCommand(operationKind, action),
            retryRequested: false,
            cancellationToken);

    private async ValueTask ExecuteCommandAsync(
        RetryCommand? requestedCommand,
        bool retryRequested,
        CancellationToken cancellationToken)
    {
        var lifetimeToken = BeginExecution();
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            lifetimeToken);
        var token = linkedCancellation.Token;
        var gateHeld = false;
        SessionCheckpoint? checkpoint = null;
        RetryCommand? command = null;
        try
        {
            await operationGate.WaitAsync(token);
            gateHeld = true;
            token.ThrowIfCancellationRequested();
            command = retryRequested ? retryCommand : requestedCommand;
            if (command is null)
            {
                return;
            }

            checkpoint = CaptureCheckpoint();
            operation = command.OperationKind;
            error = null;
            Publish();
            await command.Action(token);
            retryCommand = null;
            operation = FileBrowserOperationKind.Idle;
            Publish();
        }
        catch (Exception exception)
        {
            if (checkpoint is not null)
            {
                RestoreCheckpoint(checkpoint);
            }

            operation = FileBrowserOperationKind.Idle;
            if (token.IsCancellationRequested)
            {
                if (!IsDisposalStarted)
                {
                    Publish();
                }

                throw new OperationCanceledException("The file browser operation was canceled.", exception, token);
            }

            error = exception is FileBrowserProviderException providerException
                ? providerException.Error
                : new FileBrowserError(
                    FileBrowserErrorCode.ProviderFailure,
                    "The source could not complete the file browser request.",
                    isRetryable: true,
                    technicalDetail: exception.ToString());
            retryCommand = command;
            Publish();
        }
        finally
        {
            if (gateHeld)
            {
                operationGate.Release();
            }

            EndExecution();
        }
    }

    private async ValueTask<T> ExecuteSerializedAsync<T>(
        Func<CancellationToken, ValueTask<T>> action,
        CancellationToken cancellationToken)
    {
        var lifetimeToken = BeginExecution();
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            lifetimeToken);
        var token = linkedCancellation.Token;
        var gateHeld = false;
        try
        {
            await operationGate.WaitAsync(token);
            gateHeld = true;
            token.ThrowIfCancellationRequested();
            var result = await action(token);
            if (token.IsCancellationRequested && result is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }

            token.ThrowIfCancellationRequested();
            return result;
        }
        catch (Exception exception) when (token.IsCancellationRequested)
        {
            throw new OperationCanceledException(
                "The file browser operation was canceled.",
                exception,
                token);
        }
        finally
        {
            if (gateHeld)
            {
                operationGate.Release();
            }

            EndExecution();
        }
    }

    private void Publish()
    {
        if (IsDisposalStarted)
        {
            return;
        }

        Snapshot = CreateSnapshot();
        Changed?.Invoke(this, new FileBrowserSnapshotChangedEventArgs(Snapshot));
    }

    private FileBrowserSnapshot CreateSnapshot()
    {
        var selected = new HashSet<FileBrowserItemKey>();
        lock (selectionSync)
        {
            selected.UnionWith(selectedKeys);
        }

        var searchSnapshot = currentSearchRequest is null || currentSearchPage is null
            ? null
            : new FileBrowserSearchSnapshot(
                currentSearchRequest.Query,
                currentSearchRequest.Scope,
                currentSearchPage.StrategyId,
                currentSearchPage.IsPartial,
                currentSearchPage.ScannedContainers,
                currentSearchPage.ScannedItems,
                currentSearchPage.NextContinuationToken,
                currentSearchPage.TotalCount);
        return new FileBrowserSnapshot(
            providerCatalog.Sources,
            currentProvider?.Descriptor,
            navigation.Current,
            visibleItems,
            selected,
            sort,
            filter,
            includeDescendants,
            currentProvider is null ? [] : searchStrategies.GetAvailable(currentProvider),
            searchSnapshot,
            operation,
            error,
            warnings,
            currentSearchRequest is not null
                ? currentSearchPage?.NextContinuationToken
                : currentContainer?.NextContinuationToken,
            currentSearchRequest is not null
                ? currentSearchPage?.TotalCount
                : currentContainer?.TotalCount,
            navigation.CanGoBack,
            navigation.CanGoForward,
            navigation.CanGoUp,
            treeStore.GetDiagnostics(),
            ++revision,
            browseCompleteness: currentSearchRequest is null
                ? currentContainer?.Completeness ?? FileBrowserCompleteness.Unknown
                : FileBrowserCompleteness.Unknown,
            consistencyToken: currentSearchRequest is null
                ? currentContainer?.ConsistencyToken
                : currentSearchPage?.ConsistencyToken);
    }

    private IFileBrowserProvider RequireProvider()
        => currentProvider ?? throw new InvalidOperationException("The file browser session is not initialized.");

    private FileBrowserLocation RequireLocation()
        => navigation.Current ?? throw new InvalidOperationException("The file browser session is not initialized.");

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(IsDisposalStarted, this);

    public ValueTask DisposeAsync()
    {
        lock (lifecycleSync)
        {
            if (disposalTask is not null)
            {
                return new ValueTask(disposalTask);
            }

            disposalStarted = true;
            Changed = null;
            var drainTask = pendingExecutions == 0
                ? Task.CompletedTask
                : (executionsDrained ??= new TaskCompletionSource(
                    TaskCreationOptions.RunContinuationsAsynchronously)).Task;
            disposalTask = DisposeCoreAsync(drainTask);
            return new ValueTask(disposalTask);
        }
    }

    private async Task DisposeCoreAsync(Task drainTask)
    {
        Exception? cancellationError = null;
        try
        {
            await lifetimeCancellation.CancelAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            cancellationError = exception;
        }

        try
        {
            await drainTask.ConfigureAwait(false);
        }
        finally
        {
            operationGate.Dispose();
            lifetimeCancellation.Dispose();
        }

        if (cancellationError is not null)
        {
            throw cancellationError;
        }
    }

    private CancellationToken BeginExecution()
    {
        lock (lifecycleSync)
        {
            ObjectDisposedException.ThrowIf(disposalStarted, this);
            pendingExecutions++;
            return lifetimeCancellation.Token;
        }
    }

    private void EndExecution()
    {
        lock (lifecycleSync)
        {
            pendingExecutions--;
            if (disposalStarted && pendingExecutions == 0)
            {
                executionsDrained?.TrySetResult();
            }
        }
    }

    private bool IsDisposalStarted
    {
        get
        {
            lock (lifecycleSync)
            {
                return disposalStarted;
            }
        }
    }

    private SessionCheckpoint CaptureCheckpoint()
    {
        HashSet<FileBrowserItemKey> selection;
        lock (selectionSync)
        {
            selection = [.. selectedKeys];
        }

        return new SessionCheckpoint(
            currentProvider,
            currentContainer,
            currentBrowseRequest,
            currentSearchRequest,
            currentSearchPage,
            visibleItems,
            sort,
            filter,
            includeDescendants,
            navigation.Capture(),
            selection,
            searchContinuationTokens.ToHashSet(StringComparer.Ordinal),
            error,
            warnings,
            retryCommand);
    }

    private void RestoreCheckpoint(SessionCheckpoint checkpoint)
    {
        currentProvider = checkpoint.Provider;
        currentContainer = checkpoint.Container;
        currentBrowseRequest = checkpoint.BrowseRequest;
        currentSearchRequest = checkpoint.SearchRequest;
        currentSearchPage = checkpoint.SearchPage;
        visibleItems = checkpoint.VisibleItems;
        sort = checkpoint.Sort;
        filter = checkpoint.Filter;
        includeDescendants = checkpoint.IncludeDescendants;
        navigation.Restore(checkpoint.Navigation);
        lock (selectionSync)
        {
            selectedKeys.Clear();
            selectedKeys.UnionWith(checkpoint.SelectedKeys);
        }

        searchContinuationTokens.Clear();
        searchContinuationTokens.UnionWith(checkpoint.SearchContinuationTokens);

        error = checkpoint.Error;
        warnings = checkpoint.Warnings;
        retryCommand = checkpoint.RetryCommand;
        treeStore.SetProtectedPath(
            checkpoint.Navigation.Current?.Path.Select(item => item.Key) ?? []);
    }

    private sealed record RetryCommand(
        FileBrowserOperationKind OperationKind,
        Func<CancellationToken, ValueTask> Action);

    private sealed record ActionDispatch(
        FileBrowserItemKey? NavigationKey,
        FileBrowserActionResult? Result)
    {
        public static ActionDispatch Navigate(FileBrowserItemKey itemKey)
            => new(itemKey, null);

        public static ActionDispatch Complete(FileBrowserActionResult result)
            => new(null, result ?? throw new ArgumentNullException(nameof(result)));
    }

    private sealed record PreparedBrowse(
        FileBrowserLocation Location,
        FileBrowserBrowseRequest Request,
        FileBrowserContainerSnapshot? Cached,
        FileBrowserPage? Page);

    private sealed record SessionCheckpoint(
        IFileBrowserProvider? Provider,
        FileBrowserContainerSnapshot? Container,
        FileBrowserBrowseRequest? BrowseRequest,
        FileBrowserSearchRequest? SearchRequest,
        FileBrowserSearchPage? SearchPage,
        IReadOnlyList<FileBrowserItem> VisibleItems,
        FileBrowserSortDescriptor Sort,
        FileBrowserFilter Filter,
        bool IncludeDescendants,
        FileBrowserNavigationCheckpoint Navigation,
        IReadOnlySet<FileBrowserItemKey> SelectedKeys,
        IReadOnlySet<string> SearchContinuationTokens,
        FileBrowserError? Error,
        IReadOnlyList<FileBrowserPageWarning> Warnings,
        RetryCommand? RetryCommand);

    private sealed class SearchData : IFileBrowserSearchData
    {
        private readonly FileBrowserSession owner;
        private readonly FileBrowserTreeStore traversalStore;

        public SearchData(FileBrowserSession owner)
        {
            this.owner = owner;
            traversalStore = new FileBrowserTreeStore(owner.options.Cache);
        }

        public IReadOnlyList<FileBrowserItem> CurrentItems => owner.currentContainer?.Items ?? [];

        public bool TryGetItem(FileBrowserItemKey key, out FileBrowserItem? item)
            => owner.treeStore.TryGetItem(key, out item);

        public IReadOnlyList<FileBrowserItem> GetLoadedChildren(FileBrowserItemKey parentKey)
            => owner.treeStore.GetLoadedChildren(parentKey);

        public IReadOnlyList<FileBrowserItem> GetLoadedDescendants(FileBrowserItemKey parentKey)
            => owner.treeStore.GetLoadedDescendants(parentKey);

        public async ValueTask<FileBrowserPage> BrowseAndCacheAsync(
            FileBrowserBrowseRequest request,
            FileBrowserPageApplyMode applyMode,
            CancellationToken cancellationToken = default)
        {
            if (request.ParentKey.SourceId != owner.RequireProvider().Descriptor.Id)
            {
                throw new ArgumentException("A search cannot browse another source.", nameof(request));
            }

            var page = await owner.RequireProvider().BrowseAsync(request, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            traversalStore.ApplyPage(request, page, applyMode);
            return page;
        }
    }
}
