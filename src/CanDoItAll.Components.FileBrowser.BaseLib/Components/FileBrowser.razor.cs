using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.FileBrowser.Core;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.FileBrowser.BaseLib;

public partial class FileBrowser : ComponentBase, IAsyncDisposable
{
    private readonly FileBrowserInteractionGuard interactionGuard = new();
    private IFileBrowserSession? subscribedSession;
    private CancellationTokenSource? searchDebounceCts;
    private FileBrowserSnapshot snapshot = default!;
    private FileBrowserViewMode viewMode;
    private FileBrowserSearchScope searchScope = FileBrowserSearchScope.LoadedFolder;
    private string searchText = string.Empty;
    private bool initializedViewMode;
    private bool initializationRequested;
    private bool menuOpen;
    private double menuX;
    private double menuY;
    private FileBrowserItem? menuItem;
    private IReadOnlyList<ContextMenuItem> menuItems = [];
    private FileBrowserInteractionStamp menuStamp;
    private bool disposed;

    [Parameter, EditorRequired]
    public IFileBrowserSession Session { get; set; } = default!;

    [Parameter]
    public string AriaLabel { get; set; } = "File browser";

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public bool InitializeOnFirstRender { get; set; } = true;

    [Parameter]
    public FileBrowserSourceId? InitialSourceId { get; set; }

    [Parameter]
    public FileBrowserItemKey? InitialItemKey { get; set; }

    [Parameter]
    public FileBrowserViewMode InitialViewMode { get; set; } = FileBrowserViewMode.List;

    [Parameter]
    public int SearchDebounceMilliseconds { get; set; } = 280;

    [Parameter]
    public EventCallback<FileBrowserSnapshot> SnapshotChanged { get; set; }

    [Parameter]
    public EventCallback<FileBrowserViewMode> ViewModeChanged { get; set; }

    [Parameter]
    public EventCallback<FileBrowserItemInvokedEventArgs> ItemInvoked { get; set; }

    [Parameter]
    public EventCallback<FileBrowserItemActionEventArgs> ActionRequested { get; set; }

    public FileBrowserSnapshot Snapshot => snapshot;

    protected override void OnParametersSet()
    {
        if (Session is null)
        {
            throw new InvalidOperationException("FileBrowser requires a session.");
        }

        if (SearchDebounceMilliseconds < 0)
        {
            throw new InvalidOperationException("SearchDebounceMilliseconds cannot be negative.");
        }

        if (!ReferenceEquals(subscribedSession, Session))
        {
            interactionGuard.ChangeSession();
            CancelPendingSearch();
            ResetMenu();
            if (subscribedSession is not null)
            {
                subscribedSession.Changed -= HandleSessionChanged;
            }

            subscribedSession = Session;
            subscribedSession.Changed += HandleSessionChanged;
            snapshot = subscribedSession.Snapshot;
            SynchronizeCommittedSearchText();
            initializationRequested = false;
        }

        if (!initializedViewMode)
        {
            viewMode = InitialViewMode;
            initializedViewMode = true;
        }

        EnsureSearchScopeAvailable();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!InitializeOnFirstRender
            || initializationRequested
            || Snapshot.CurrentSource is not null
            || Snapshot.IsBusy)
        {
            return;
        }

        initializationRequested = true;
        await Session.InitializeAsync(InitialSourceId, InitialItemKey);
    }

    private async void HandleSessionChanged(object? sender, FileBrowserSnapshotChangedEventArgs args)
    {
        var eventSession = sender as IFileBrowserSession;
        var expectedSessionVersion = interactionGuard.Capture().SessionVersion;
        if (disposed || !ReferenceEquals(eventSession, subscribedSession))
        {
            return;
        }

        try
        {
            await InvokeAsync(async () =>
            {
                if (disposed
                    || !ReferenceEquals(eventSession, subscribedSession)
                    || !interactionGuard.IsCurrentSession(expectedSessionVersion)
                    || args.Snapshot.Revision < snapshot.Revision)
                {
                    return;
                }

                interactionGuard.AcceptSnapshot();
                ResetMenu();
                snapshot = args.Snapshot;
                if (interactionGuard.CanSynchronizeCommittedSearch)
                {
                    SynchronizeCommittedSearchText();
                }

                EnsureSearchScopeAvailable();
                await SnapshotChanged.InvokeAsync(snapshot);
                StateHasChanged();
            });
        }
        catch (ObjectDisposedException) when (disposed)
        {
        }
        catch (Exception exception)
        {
            await DispatchExceptionAsync(exception);
        }
    }

    private Task HandleSearchTextChangedAsync(string? value)
    {
        var requestedQuery = value ?? string.Empty;
        searchText = requestedQuery;
        var searchEditVersion = interactionGuard.BeginSearchEdit();
        CancelPendingSearch();
        var debounce = new CancellationTokenSource();
        searchDebounceCts = debounce;
        var requestedScope = searchScope;
        var requestedSession = subscribedSession ?? Session;
        var expectedSessionVersion = interactionGuard.Capture().SessionVersion;

        _ = RunSearchEditAsync(
            requestedQuery,
            requestedScope,
            requestedSession,
            expectedSessionVersion,
            searchEditVersion,
            debounce);
        return Task.CompletedTask;
    }

    private async Task RunSearchEditAsync(
        string requestedQuery,
        FileBrowserSearchScope requestedScope,
        IFileBrowserSession requestedSession,
        long expectedSessionVersion,
        long searchEditVersion,
        CancellationTokenSource debounce)
    {
        var token = debounce.Token;
        try
        {
            if (string.IsNullOrWhiteSpace(requestedQuery))
            {
                await requestedSession.ClearSearchAsync(token);
                return;
            }

            await Task.Delay(SearchDebounceMilliseconds, token);
            if (!interactionGuard.IsCurrentSession(expectedSessionVersion)
                || !ReferenceEquals(requestedSession, subscribedSession))
            {
                return;
            }

            await requestedSession.SearchAsync(requestedQuery, requestedScope, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (ObjectDisposedException) when (disposed)
        {
        }
        catch (Exception exception)
        {
            if (!disposed)
            {
                await InvokeAsync(() => DispatchExceptionAsync(exception));
            }
        }
        finally
        {
            if (ReferenceEquals(searchDebounceCts, debounce))
            {
                searchDebounceCts = null;
            }

            debounce.Dispose();
            if (!disposed
                && interactionGuard.TryCompleteSearchEdit(searchEditVersion)
                && interactionGuard.IsCurrentSession(expectedSessionVersion)
                && ReferenceEquals(requestedSession, subscribedSession))
            {
                SynchronizeCommittedSearchText();
            }
        }
    }

    private async Task HandleSearchScopeChangedAsync(FileBrowserSearchScope scope)
    {
        searchScope = scope;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var requestedQuery = searchText;
        var searchEditVersion = interactionGuard.BeginSearchEdit();
        CancelPendingSearch();
        var search = new CancellationTokenSource();
        searchDebounceCts = search;
        var requestedSession = subscribedSession ?? Session;
        var expectedSessionVersion = interactionGuard.Capture().SessionVersion;
        try
        {
            await requestedSession.SearchAsync(requestedQuery, scope, search.Token);
        }
        catch (OperationCanceledException) when (search.IsCancellationRequested)
        {
        }
        finally
        {
            if (ReferenceEquals(searchDebounceCts, search))
            {
                searchDebounceCts = null;
            }

            search.Dispose();
            if (interactionGuard.TryCompleteSearchEdit(searchEditVersion)
                && interactionGuard.IsCurrentSession(expectedSessionVersion)
                && ReferenceEquals(requestedSession, subscribedSession))
            {
                SynchronizeCommittedSearchText();
            }
        }
    }

    private async Task HandleViewModeChangedAsync(FileBrowserViewMode mode)
    {
        viewMode = mode;
        await ViewModeChanged.InvokeAsync(mode);
    }

    private Task HandleCategoryChangedAsync(FileBrowserItemCategory? category)
        => Session.SetFilterAsync(category.HasValue
            ? new FileBrowserFilter(categories: [category.Value])
            : FileBrowserFilter.None).AsTask();

    private Task HandleSortChangedAsync(FileBrowserSortField field)
        => Session.SetSortAsync(Snapshot.Sort with { Field = field }).AsTask();

    private Task HandleSortRequestedAsync(FileBrowserSortField field)
        => field == Snapshot.Sort.Field
            ? ReverseSortAsync()
            : HandleSortChangedAsync(field);

    private Task ReverseSortAsync()
        => Session.SetSortAsync(Snapshot.Sort with
        {
            Direction = Snapshot.Sort.Direction == FileBrowserSortDirection.Ascending
                ? FileBrowserSortDirection.Descending
                : FileBrowserSortDirection.Ascending
        }).AsTask();

    private Task SetIncludeDescendantsAsync(bool value)
        => Session.SetIncludeDescendantsAsync(value).AsTask();

    private Task RefreshAsync() => Session.RefreshAsync().AsTask();

    private Task RetryAsync() => Session.RetryAsync().AsTask();

    private Task ClearSearchAsync() => HandleSearchTextChangedAsync(string.Empty);

    private Task GoBackAsync() => Session.GoBackAsync().AsTask();

    private Task GoForwardAsync() => Session.GoForwardAsync().AsTask();

    private Task GoUpAsync() => Session.GoUpAsync().AsTask();

    private Task NavigateAsync(FileBrowserItemKey key) => Session.NavigateAsync(key).AsTask();

    private Task LoadMoreAsync() => Session.LoadMoreAsync().AsTask();

    private void SelectItem(FileBrowserItem item)
    {
        if (!ResultsAreReplacing && Snapshot.Items.Any(current => current.Key == item.Key))
        {
            Session.Select(item.Key);
        }
    }

    private void ToggleSelection(FileBrowserItem item)
    {
        if (!ResultsAreReplacing && Snapshot.Items.Any(current => current.Key == item.Key))
        {
            Session.Select(item.Key, toggle: true);
        }
    }

    private async Task InvokeItemAsync(FileBrowserItemInvokedEventArgs args)
    {
        var currentItem = Snapshot.Items.FirstOrDefault(item => item.Key == args.Item.Key);
        if (ResultsAreReplacing || currentItem is null)
        {
            return;
        }

        if (currentItem.IsContainer && currentItem.Supports(FileBrowserItemCapabilities.Navigate))
        {
            await Session.NavigateAsync(currentItem.Key);
            return;
        }

        if (currentItem.Supports(FileBrowserItemCapabilities.Open)
            || currentItem.Supports(FileBrowserItemCapabilities.Preview))
        {
            await ItemInvoked.InvokeAsync(args with { Item = currentItem });
        }
    }

    private Task HandleActionRequestedAsync(FileBrowserItemActionEventArgs args)
    {
        var currentItem = Snapshot.Items.FirstOrDefault(item => item.Key == args.Item.Key);
        return ResultsAreReplacing
            || currentItem is null
            || !IsActionStillSupported(currentItem, args.ActionId)
                ? Task.CompletedTask
                : ActionRequested.InvokeAsync(args with { Item = currentItem });
    }

    private async Task OpenMenu(FileBrowserActionMenuRequest request)
    {
        if (ResultsAreReplacing || !Snapshot.Items.Any(item => item.Key == request.Item.Key))
        {
            return;
        }

        var requestedSession = subscribedSession ?? Session;
        var requestStamp = interactionGuard.Capture();
        IReadOnlyList<FileBrowserActionDescriptor> actions;
        try
        {
            actions = await requestedSession.GetActionsAsync(request.Item.Key);
        }
        catch (FileBrowserProviderException)
        {
            actions = FileBrowserBuiltInActions.GetFor(request.Item);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            actions = FileBrowserBuiltInActions.GetFor(request.Item);
        }

        if (ResultsAreReplacing
            || !ReferenceEquals(requestedSession, subscribedSession)
            || !interactionGuard.IsCurrent(requestStamp))
        {
            return;
        }

        var currentItem = Snapshot.Items.FirstOrDefault(item => item.Key == request.Item.Key);
        if (currentItem is null)
        {
            return;
        }

        menuItems = BuildMenuItems(actions);
        if (menuItems.Count == 0)
        {
            ResetMenu();
            return;
        }

        menuItem = currentItem;
        menuX = request.X;
        menuY = request.Y;
        menuStamp = requestStamp;
        menuOpen = true;
    }

    private async Task HandleMenuItemSelectedAsync(string actionId)
    {
        var item = menuItem;
        var currentItem = item is null
            ? null
            : Snapshot.Items.FirstOrDefault(candidate => candidate.Key == item.Key);
        var actionWasOffered = menuItems.Any(candidate => candidate.Id == actionId);
        var isCurrent = menuOpen
            && interactionGuard.IsCurrent(menuStamp)
            && currentItem is not null
            && actionWasOffered
            && IsActionStillSupported(currentItem, actionId);
        await CloseMenuAsync();
        if (!isCurrent || currentItem is null)
        {
            return;
        }

        if (actionId == FileBrowserActionIds.Open)
        {
            await InvokeItemAsync(new FileBrowserItemInvokedEventArgs(currentItem, FileBrowserInvocationKind.PrimaryAction));
            return;
        }

        await ActionRequested.InvokeAsync(new FileBrowserItemActionEventArgs(currentItem, actionId));
    }

    private Task CloseMenuAsync()
    {
        ResetMenu();
        return Task.CompletedTask;
    }

    private void ResetMenu()
    {
        menuOpen = false;
        menuItem = null;
        menuItems = [];
        menuStamp = default;
    }

    private static bool IsActionStillSupported(FileBrowserItem item, string actionId)
    {
        if (FileBrowserBuiltInActions.GetFor(item).Any(action => action.Id == actionId))
        {
            return true;
        }

        return item.Supports(FileBrowserItemCapabilities.CustomActions)
            && actionId is not FileBrowserActionIds.Open
                and not FileBrowserActionIds.OpenInNewTab
                and not FileBrowserActionIds.CopyPath
                and not FileBrowserActionIds.CopyContentIdentity
                and not FileBrowserActionIds.Download;
    }

    private static IReadOnlyList<ContextMenuItem> BuildMenuItems(
        IReadOnlyList<FileBrowserActionDescriptor> actions)
    {
        var items = new List<ContextMenuItem>();
        foreach (var action in actions)
        {
            var startsCopyGroup = action.Id == FileBrowserActionIds.CopyPath;
            var startsDownloadGroup = action.Id == FileBrowserActionIds.Download;
            items.Add(new ContextMenuItem
            {
                Id = action.Id,
                Text = action.Label,
                Icon = action.Icon,
                Danger = action.Tone == FileBrowserActionTone.Danger,
                SeparatorBefore = items.Count > 0 && (startsCopyGroup || startsDownloadGroup)
            });
        }

        return items;
    }

    private void EnsureSearchScopeAvailable()
    {
        if (snapshot.AvailableSearchScopes.Count > 0
            && !snapshot.AvailableSearchScopes.Contains(searchScope))
        {
            searchScope = snapshot.AvailableSearchScopes[0];
        }
    }

    private bool ResultsAreReplacing
        => Snapshot.Operation is FileBrowserOperationKind.Initializing
            or FileBrowserOperationKind.LoadingFolder
            or FileBrowserOperationKind.Refreshing
            or FileBrowserOperationKind.Searching;

    private void CancelPendingSearch()
    {
        searchDebounceCts?.Cancel();
        searchDebounceCts = null;
    }

    private void SynchronizeCommittedSearchText()
        => searchText = snapshot.Search?.Query ?? string.Empty;

    public async ValueTask DisposeAsync()
    {
        disposed = true;
        interactionGuard.ChangeSession();
        CancelPendingSearch();
        ResetMenu();
        if (subscribedSession is not null)
        {
            subscribedSession.Changed -= HandleSessionChanged;
        }

        await Task.CompletedTask;
    }
}
