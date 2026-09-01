using CanDoItAll.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

public partial class SideMenu : IAsyncDisposable
{
    private const string MorePanelId = "__more";
    private const string MobileMorePanelId = "__mobile-more";
    private const int DefaultVisibleItemCapacity = 8;

    private readonly string instanceId = $"cad-side-menu-{Guid.NewGuid():N}";
    private readonly Dictionary<SideMenuItem, int> declarativeItemVersions = [];
    private ElementReference rootElement;
    private ElementReference itemViewportElement;
    private ElementReference measureItemElement;
    private ElementReference openPanelElement;
    private DotNetObjectReference<SideMenu>? dotNetReference;
    private SideMenuInterop? interop;
    private IDisposable? selectionSubscription;
    private string? configuredMenuId;
    private string? openPanelId;
    private SideMenuLayoutMetrics layoutMetrics = new()
    {
        DisplayMode = SideMenuDisplayMode.Large,
        VisibleItemCapacity = DefaultVisibleItemCapacity
    };
    private bool expandedParameterInitialized;
    private bool lastExpandedParameter;
    private bool? lastReportedExpanded;
    private bool interopInitialized;
    private bool interopReinitializePending;
    private bool storageReady;
    private bool persistExpandedPending;
    private bool layoutRefreshPending;
    private bool panelPositionPending;
    private bool disposed;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Inject]
    private SideMenuService MenuService { get; set; } = default!;

    /// <summary>Identifier used to look up this menu's state in <see cref="SideMenuService"/>; must be unique per menu instance.</summary>
    [Parameter]
    public string MenuId { get; set; } = "primary";

    /// <summary>The menu's title, shown in the header when expanded and used as the mobile toggle's small label.</summary>
    [Parameter]
    public string Title { get; set; } = "Menu";

    /// <summary>Optional subtitle shown below <see cref="Title"/> when expanded and <see cref="HeaderContent"/> is unset.</summary>
    [Parameter]
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>Material icon token shown as the menu's brand mark.</summary>
    [Parameter]
    public string Icon { get; set; } = "apps";

    /// <summary>Accessible label applied to the menu's navigation regions.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Primary navigation";

    /// <summary>Label for the overflow "More" entry point.</summary>
    [Parameter]
    public string MoreText { get; set; } = "More";

    /// <summary>Whether the menu is expanded (showing labels) or collapsed (icons only).</summary>
    [Parameter]
    public bool Expanded { get; set; } = true;

    /// <summary>Raised whenever <see cref="Expanded"/> changes, including via the collapse toggle or restored state.</summary>
    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>When <c>true</c>, the collapse/expand toggle button is shown.</summary>
    [Parameter]
    public bool AllowCollapse { get; set; } = true;

    /// <summary>When <c>true</c>, the menu stretches to fill the height of its parent.</summary>
    [Parameter]
    public bool FillHeight { get; set; } = true;

    /// <summary>When <c>true</c>, the current <see cref="Expanded"/> state is persisted to browser storage.</summary>
    [Parameter]
    public bool PersistExpandedState { get; set; } = true;

    /// <summary>When <c>true</c>, a previously persisted expanded state is restored on load.</summary>
    [Parameter]
    public bool RestoreExpandedState { get; set; } = true;

    /// <summary>Storage key used to persist the expanded state; defaults to a key derived from <see cref="MenuId"/>.</summary>
    [Parameter]
    public string? PersistenceKey { get; set; }

    /// <summary>Maximum number of primary items shown before overflowing into the "More" panel; defaults to the measured layout capacity.</summary>
    [Parameter]
    public int? VisibleItemCapacity { get; set; }

    /// <summary>The menu's primary items, in addition to any declared as <see cref="SideMenuItem"/> children.</summary>
    [Parameter]
    public IReadOnlyList<ISideMenuItem> Items { get; set; } = [];

    /// <summary>Items always shown in the overflow "More" panel, regardless of available space.</summary>
    [Parameter]
    public IReadOnlyList<ISideMenuItem> MoreItems { get; set; } = [];

    /// <summary>Items pinned to the bottom of the menu, separate from the scrollable primary list.</summary>
    [Parameter]
    public IReadOnlyList<ISideMenuItem> BottomItems { get; set; } = [];

    /// <summary>Custom content replacing the default title/subtitle header when expanded.</summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>Declarative <see cref="SideMenuItem"/> children for the primary item list.</summary>
    [Parameter]
    public RenderFragment? MenuItems { get; set; }

    /// <summary>Declarative <see cref="SideMenuItem"/> children for the overflow "More" panel.</summary>
    [Parameter]
    public RenderFragment? MoreMenuItems { get; set; }

    /// <summary>Custom content appended inside the "More" panel, alongside any configured more items.</summary>
    [Parameter]
    public RenderFragment? MoreContent { get; set; }

    /// <summary>Declarative <see cref="SideMenuItem"/> children for the bottom-pinned item list.</summary>
    [Parameter]
    public RenderFragment? BottomMenuItems { get; set; }

    /// <summary>Custom content shown when the menu has no visible items.</summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>Raised whenever an item is selected, from any input source (pointer, keyboard, or programmatic).</summary>
    [Parameter]
    public EventCallback<SideMenuSelection> ItemSelected { get; set; }

    private SideMenuSnapshot CurrentSnapshot => MenuService.GetSnapshot(MenuId);

    private string ResolvedPersistenceKey => string.IsNullOrWhiteSpace(PersistenceKey)
        ? $"cda.side-menu.{MenuId}.expanded"
        : PersistenceKey.Trim();

    protected override void OnInitialized()
    {
        MenuService.Changed += HandleServiceChanged;
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(MenuId))
        {
            throw new InvalidOperationException("SideMenu requires a non-empty MenuId.");
        }

        if (!string.Equals(MenuId, MenuId.Trim(), StringComparison.Ordinal))
        {
            throw new InvalidOperationException("SideMenu MenuId cannot contain leading or trailing whitespace.");
        }

        var menuChanged = !string.Equals(configuredMenuId, MenuId, StringComparison.Ordinal);
        if (menuChanged)
        {
            ConfigureMenu();
        }

        if (!expandedParameterInitialized || menuChanged)
        {
            lastExpandedParameter = Expanded;
            expandedParameterInitialized = true;
        }
        else if (lastExpandedParameter != Expanded)
        {
            lastExpandedParameter = Expanded;
            MenuService.SetExpanded(MenuId, Expanded);
        }

        SyncDeclaredItems();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || interopReinitializePending)
        {
            interop ??= new SideMenuInterop(JS);
            dotNetReference ??= DotNetObjectReference.Create(this);
            storageReady = false;

            var initialization = await interop.InitializeAsync(
                instanceId,
                rootElement,
                itemViewportElement,
                measureItemElement,
                dotNetReference,
                ResolvedPersistenceKey,
                PersistExpandedState && RestoreExpandedState);

            layoutMetrics = NormalizeMetrics(initialization.Metrics);
            if (initialization.StoredExpanded.HasValue)
            {
                MenuService.RestoreExpandedState(MenuId, initialization.StoredExpanded.Value);
            }

            storageReady = true;
            interopInitialized = true;
            interopReinitializePending = false;
            persistExpandedPending = PersistExpandedState && CurrentSnapshot.HasExplicitExpandedState;
            layoutRefreshPending = true;
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (persistExpandedPending && interop is not null)
        {
            persistExpandedPending = false;
            await interop.SaveExpandedAsync(ResolvedPersistenceKey, CurrentSnapshot.IsExpanded);
        }

        if (layoutRefreshPending && interop is not null)
        {
            layoutRefreshPending = false;
            await interop.RefreshAsync(instanceId);
        }

        if (panelPositionPending && interop is not null)
        {
            panelPositionPending = false;
            await interop.PositionPanelAsync(openPanelElement);
        }
    }

    [JSInvokable]
    public async Task OnSideMenuLayoutChanged(SideMenuLayoutMetrics metrics)
    {
        var normalized = NormalizeMetrics(metrics);
        if (layoutMetrics.DisplayMode == normalized.DisplayMode
            && layoutMetrics.VisibleItemCapacity == normalized.VisibleItemCapacity)
        {
            return;
        }

        await InvokeAsync(() =>
        {
            layoutMetrics = normalized;
            panelPositionPending = openPanelId is not null
                && layoutMetrics.DisplayMode is not SideMenuDisplayMode.Small;
            if (layoutMetrics.DisplayMode is not SideMenuDisplayMode.Small)
            {
                MenuService.SetMobileMenuOpen(MenuId, false);
            }

            StateHasChanged();
        });
    }

    public Task<bool> SelectAsync(string itemId)
        => MenuService.SelectAsync(MenuId, itemId, SideMenuSelectionSource.External);

    public bool SetExpanded(bool isExpanded)
        => MenuService.SetExpanded(MenuId, isExpanded);

    public bool ToggleExpanded()
        => MenuService.ToggleExpanded(MenuId);

    public void SetItems(IReadOnlyList<ISideMenuItem> items)
        => MenuService.SetItems(MenuId, items);

    public void ResetItems()
        => MenuService.ResetItems(MenuId);

    internal async void RegisterDeclarativeItem(SideMenuItem item)
    {
        if (disposed
            || declarativeItemVersions.TryGetValue(item, out var version)
            && version == item.RegistrationVersion)
        {
            return;
        }

        declarativeItemVersions[item] = item.RegistrationVersion;

        try
        {
            SyncDeclaredItems();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            await DispatchExceptionAsync(exception);
        }
    }

    internal async void UnregisterDeclarativeItem(SideMenuItem item)
    {
        if (disposed || !declarativeItemVersions.Remove(item))
        {
            return;
        }

        try
        {
            SyncDeclaredItems();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            await DispatchExceptionAsync(exception);
        }
    }

    private void ConfigureMenu()
    {
        selectionSubscription?.Dispose();
        configuredMenuId = MenuId;
        var snapshot = MenuService.EnsureMenu(MenuId, Expanded);
        lastReportedExpanded = snapshot.IsExpanded;
        selectionSubscription = MenuService.Subscribe(MenuId, HandleSelectionAsync);
        openPanelId = null;

        if (interopInitialized)
        {
            interopReinitializePending = true;
        }
    }

    private void SyncDeclaredItems()
    {
        if (configuredMenuId is null)
        {
            return;
        }

        var primaryItems = GetItems(SideMenuItemPlacement.Primary);
        var moreItems = GetItems(SideMenuItemPlacement.More);
        var bottomItems = GetItems(SideMenuItemPlacement.Bottom);
        MenuService.SetDeclaredItems(MenuId, primaryItems, moreItems.Concat(bottomItems).ToArray());
    }

    private IReadOnlyList<ISideMenuItem> GetItems(SideMenuItemPlacement placement)
    {
        var items = placement switch
        {
            SideMenuItemPlacement.More => MoreItems,
            SideMenuItemPlacement.Bottom => BottomItems,
            _ => Items
        };

        return items
            .Concat(declarativeItemVersions.Keys
                .Where(item => item.RegisteredPlacement == placement)
                .Select(item => (ISideMenuItem)item.ToDefinition()))
            .ToArray();
    }

    private SideMenuItemSplit ResolveDesktopItems(
        IReadOnlyList<ISideMenuItem> primaryItems,
        IReadOnlyList<ISideMenuItem> configuredMoreItems,
        string? selectedItemId)
    {
        var availablePrimaryItems = primaryItems.Where(item => item.Visible).ToArray();
        var candidates = availablePrimaryItems
            .Where(item => item.OverflowBehavior != SideMenuOverflowBehavior.AlwaysInMore)
            .ToArray();
        var configuredMore = configuredMoreItems.Where(item => item.Visible).ToArray();
        var forcedMore = availablePrimaryItems
            .Where(item => item.OverflowBehavior == SideMenuOverflowBehavior.AlwaysInMore)
            .ToArray();
        var capacity = Math.Clamp(
            VisibleItemCapacity ?? layoutMetrics.VisibleItemCapacity,
            2,
            64);
        var needsMore = forcedMore.Length > 0
            || configuredMore.Length > 0
            || MoreContent is not null
            || candidates.Length > capacity;

        if (!needsMore)
        {
            return new SideMenuItemSplit(candidates, []);
        }

        var visibleSlots = Math.Max(1, capacity - 1);
        var prioritizedIds = candidates
            .Select((item, index) => new { Item = item, Index = index })
            .OrderBy(entry => entry.Item.OverflowBehavior == SideMenuOverflowBehavior.PreferVisible ? 0 : 1)
            .ThenBy(entry => entry.Index)
            .Take(visibleSlots)
            .Select(entry => entry.Item.Id)
            .ToHashSet(StringComparer.Ordinal);

        var selectedCandidate = candidates.FirstOrDefault(item =>
            string.Equals(item.Id, selectedItemId, StringComparison.Ordinal));
        if (selectedCandidate is not null && !prioritizedIds.Contains(selectedCandidate.Id))
        {
            var replacement = candidates
                .Where(item => prioritizedIds.Contains(item.Id))
                .LastOrDefault(item => item.OverflowBehavior != SideMenuOverflowBehavior.PreferVisible)
                ?? candidates.LastOrDefault(item => prioritizedIds.Contains(item.Id));

            if (replacement is not null)
            {
                prioritizedIds.Remove(replacement.Id);
                prioritizedIds.Add(selectedCandidate.Id);
            }
        }

        var visibleItems = candidates.Where(item => prioritizedIds.Contains(item.Id)).ToArray();
        var moreItems = availablePrimaryItems
            .Where(item => !prioritizedIds.Contains(item.Id))
            .Concat(configuredMore)
            .ToArray();
        return new SideMenuItemSplit(visibleItems, moreItems);
    }

    private IReadOnlyList<ISideMenuItem> ResolveMobilePrimaryItems(IReadOnlyList<ISideMenuItem> primaryItems)
        => primaryItems
            .Where(item => item.Visible && item.OverflowBehavior != SideMenuOverflowBehavior.AlwaysInMore)
            .ToArray();

    private IReadOnlyList<ISideMenuItem> ResolveMobileMoreItems(
        IReadOnlyList<ISideMenuItem> primaryItems,
        IReadOnlyList<ISideMenuItem> configuredMoreItems)
        => primaryItems
            .Where(item => item.Visible && item.OverflowBehavior == SideMenuOverflowBehavior.AlwaysInMore)
            .Concat(configuredMoreItems.Where(item => item.Visible))
            .ToArray();

    private async Task ActivateItemAsync(
        ISideMenuItem item,
        SideMenuSelectionSource source = SideMenuSelectionSource.Pointer)
    {
        if (item.Disabled || !item.Visible)
        {
            return;
        }

        if (ShouldOpenPanel(item))
        {
            var isClosing = string.Equals(openPanelId, item.Id, StringComparison.Ordinal);
            openPanelId = isClosing
                ? null
                : item.Id;
            panelPositionPending = !isClosing
                && layoutMetrics.DisplayMode is not SideMenuDisplayMode.Small;
            await InvokeAsync(StateHasChanged);
            return;
        }

        openPanelId = null;
        await MenuService.SelectAsync(MenuId, item.Id, source);
    }

    private async Task SelectPanelItemAsync(ISideMenuItem item)
    {
        if (item.Disabled || !item.Visible)
        {
            return;
        }

        openPanelId = null;
        await MenuService.SelectAsync(MenuId, item.Id, SideMenuSelectionSource.Pointer);
    }

    private async Task ToggleMorePanelAsync(bool mobile = false)
    {
        var panelId = mobile ? MobileMorePanelId : MorePanelId;
        var isClosing = string.Equals(openPanelId, panelId, StringComparison.Ordinal);
        openPanelId = isClosing
            ? null
            : panelId;
        panelPositionPending = !isClosing && !mobile;
        await InvokeAsync(StateHasChanged);
    }

    private Task ToggleMobileMenuAsync()
    {
        openPanelId = null;
        panelPositionPending = false;
        MenuService.ToggleMobileMenu(MenuId);
        return Task.CompletedTask;
    }

    private Task ToggleExpandedAsync()
    {
        openPanelId = null;
        panelPositionPending = false;
        MenuService.ToggleExpanded(MenuId);
        return Task.CompletedTask;
    }

    private Task CloseOverlaysAsync()
    {
        openPanelId = null;
        panelPositionPending = false;
        MenuService.SetMobileMenuOpen(MenuId, false);
        return InvokeAsync(StateHasChanged);
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        return string.Equals(args.Key, "Escape", StringComparison.Ordinal)
            ? CloseOverlaysAsync()
            : Task.CompletedTask;
    }

    private async ValueTask HandleSelectionAsync(SideMenuSelection selection)
    {
        var declarativeItem = declarativeItemVersions.Keys.FirstOrDefault(item =>
            string.Equals(item.Id, selection.ItemId, StringComparison.Ordinal)
            && (item.RegisteredPlacement != SideMenuItemPlacement.Primary || !CurrentSnapshot.HasItemsOverride));

        if (declarativeItem is not null)
        {
            await declarativeItem.NotifySelectedAsync(selection);
        }

        await ItemSelected.InvokeAsync(selection);
        openPanelId = null;
        panelPositionPending = false;

        if (layoutMetrics.DisplayMode == SideMenuDisplayMode.Small)
        {
            MenuService.SetMobileMenuOpen(MenuId, false);
        }
    }

    private async void HandleServiceChanged(object? sender, SideMenuStateChangedEventArgs args)
    {
        if (disposed || !string.Equals(args.MenuId, MenuId, StringComparison.Ordinal))
        {
            return;
        }

        try
        {
            var snapshot = CurrentSnapshot;
            if (args.ChangeKind.HasFlag(SideMenuStateChangeKind.Expanded))
            {
                openPanelId = null;
                panelPositionPending = false;
                layoutRefreshPending = true;
                if (storageReady && PersistExpandedState)
                {
                    persistExpandedPending = true;
                }

                if (lastReportedExpanded != snapshot.IsExpanded)
                {
                    lastReportedExpanded = snapshot.IsExpanded;
                    await ExpandedChanged.InvokeAsync(snapshot.IsExpanded);
                }
            }

            if (args.ChangeKind.HasFlag(SideMenuStateChangeKind.Items))
            {
                openPanelId = null;
                panelPositionPending = false;
                layoutRefreshPending = true;
            }

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            await DispatchExceptionAsync(exception);
        }
    }

    private string ResolveRootCssClass(SideMenuSnapshot snapshot)
    {
        return BuildClass(
            "cad-side-menu",
            snapshot.IsExpanded ? "cad-side-menu--expanded" : "cad-side-menu--collapsed",
            FillHeight ? "cad-side-menu--fill-height" : null,
            layoutMetrics.DisplayMode switch
            {
                SideMenuDisplayMode.Small => "cad-side-menu--small",
                SideMenuDisplayMode.Medium => "cad-side-menu--medium",
                _ => "cad-side-menu--large"
            });
    }

    private static string ResolveItemCssClass(ISideMenuItem item, SideMenuSnapshot snapshot)
    {
        return CssClassBuilder.Join(
            "cad-side-menu__item",
            snapshot.IsExpanded ? "cad-side-menu__item--expanded" : "cad-side-menu__item--collapsed",
            string.Equals(snapshot.SelectedItemId, item.Id, StringComparison.Ordinal)
                ? "is-selected"
                : null,
            item.Disabled ? "is-disabled" : null);
    }

    private static string ResolvePanelItemCssClass(ISideMenuItem item, SideMenuSnapshot snapshot)
    {
        return CssClassBuilder.Join(
            "cad-side-menu__panel-item",
            string.Equals(snapshot.SelectedItemId, item.Id, StringComparison.Ordinal)
                ? "is-selected"
                : null,
            item.Disabled ? "is-disabled" : null);
    }

    private static string ResolveMobileItemCssClass(ISideMenuItem item, SideMenuSnapshot snapshot)
    {
        return CssClassBuilder.Join(
            "cad-side-menu__mobile-item",
            string.Equals(snapshot.SelectedItemId, item.Id, StringComparison.Ordinal)
                ? "is-selected"
                : null,
            item.Disabled ? "is-disabled" : null);
    }

    private static bool ShouldOpenPanel(ISideMenuItem item)
    {
        return item.Activation == SideMenuItemActivation.OpenPanel
            || item.Activation == SideMenuItemActivation.Auto && HasPanel(item);
    }

    private static bool HasPanel(ISideMenuItem item)
        => item.PanelContent is not null || item.Children.Count > 0;

    private static string ResolvePanelTitle(ISideMenuItem item)
        => string.IsNullOrWhiteSpace(item.PanelTitle) ? item.Text : item.PanelTitle;

    private static string ResolveIcon(ISideMenuItem item)
        => string.IsNullOrWhiteSpace(item.Icon) ? "circle" : item.Icon;

    private static string ResolveItemTestId(string menuId, string itemId)
        => $"side-menu-{NormalizeForTestId(menuId)}-item-{NormalizeForTestId(itemId)}";

    private static string NormalizeForTestId(string value)
        => string.Concat(value.Trim().Select(character =>
            char.IsLetterOrDigit(character) || character is '-' or '_'
                ? char.ToLowerInvariant(character)
                : '-'));

    private static ISideMenuItem? FindItem(
        IEnumerable<ISideMenuItem> items,
        string? itemId)
    {
        if (itemId is null)
        {
            return null;
        }

        foreach (var item in items)
        {
            if (string.Equals(item.Id, itemId, StringComparison.Ordinal))
            {
                return item;
            }

            var child = FindItem(item.Children, itemId);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private static SideMenuLayoutMetrics NormalizeMetrics(SideMenuLayoutMetrics? metrics)
    {
        return new SideMenuLayoutMetrics
        {
            DisplayMode = metrics?.DisplayMode ?? SideMenuDisplayMode.Large,
            VisibleItemCapacity = Math.Clamp(
                metrics?.VisibleItemCapacity ?? DefaultVisibleItemCapacity,
                2,
                64)
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        MenuService.Changed -= HandleServiceChanged;
        selectionSubscription?.Dispose();

        if (interop is not null)
        {
            await interop.DisposeInstanceAsync(instanceId);
            await interop.DisposeAsync();
        }

        dotNetReference?.Dispose();
    }

    private sealed record SideMenuItemSplit(
        IReadOnlyList<ISideMenuItem> VisibleItems,
        IReadOnlyList<ISideMenuItem> MoreItems);
}
