namespace CanDoItAll.Components.BaseLib;

public sealed class SideMenuService
{
    private readonly object gate = new();
    private readonly Dictionary<string, MenuState> states = new(StringComparer.Ordinal);
    private readonly Dictionary<Guid, SelectionSubscription> subscriptions = [];

    public event EventHandler<SideMenuStateChangedEventArgs>? Changed;

    public SideMenuSnapshot GetSnapshot(string menuId)
    {
        menuId = NormalizeMenuId(menuId);

        lock (gate)
        {
            return CreateSnapshot(GetOrCreateState(menuId));
        }
    }

    public void SetItems(string menuId, IReadOnlyList<ISideMenuItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        menuId = NormalizeMenuId(menuId);
        var nextItems = items.ToArray();
        SideMenuStateChangeKind changeKind;

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            ValidateItems(nextItems, state.AuxiliaryItems, menuId);

            if (state.ItemsOverride is not null && AreEquivalent(state.ItemsOverride, nextItems))
            {
                return;
            }

            state.ItemsOverride = nextItems;
            changeKind = SideMenuStateChangeKind.Items;
            if (!IsSelectedItemAvailable(state))
            {
                state.SelectedItemId = null;
                changeKind |= SideMenuStateChangeKind.Selection;
            }
        }

        RaiseChanged(menuId, changeKind);
    }

    public void ResetItems(string menuId)
    {
        menuId = NormalizeMenuId(menuId);
        SideMenuStateChangeKind changeKind;

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            if (state.ItemsOverride is null)
            {
                return;
            }

            state.ItemsOverride = null;
            changeKind = SideMenuStateChangeKind.Items;
            if (!IsSelectedItemAvailable(state))
            {
                state.SelectedItemId = null;
                changeKind |= SideMenuStateChangeKind.Selection;
            }
        }

        RaiseChanged(menuId, changeKind);
    }

    public bool SetExpanded(string menuId, bool isExpanded)
    {
        menuId = NormalizeMenuId(menuId);

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            state.HasExplicitExpandedState = true;
            if (state.IsExpanded == isExpanded)
            {
                return false;
            }

            state.IsExpanded = isExpanded;
        }

        RaiseChanged(menuId, SideMenuStateChangeKind.Expanded);
        return true;
    }

    public bool ToggleExpanded(string menuId)
    {
        var snapshot = GetSnapshot(menuId);
        return SetExpanded(menuId, !snapshot.IsExpanded);
    }

    public bool SetMobileMenuOpen(string menuId, bool isOpen)
    {
        menuId = NormalizeMenuId(menuId);

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            if (state.IsMobileMenuOpen == isOpen)
            {
                return false;
            }

            state.IsMobileMenuOpen = isOpen;
        }

        RaiseChanged(menuId, SideMenuStateChangeKind.MobileMenu);
        return true;
    }

    public bool ToggleMobileMenu(string menuId)
    {
        var snapshot = GetSnapshot(menuId);
        return SetMobileMenuOpen(menuId, !snapshot.IsMobileMenuOpen);
    }

    public async Task<bool> SelectAsync(
        string menuId,
        string itemId,
        SideMenuSelectionSource source = SideMenuSelectionSource.External)
    {
        menuId = NormalizeMenuId(menuId);
        itemId = NormalizeItemId(itemId);
        ISideMenuItem? item;
        Func<SideMenuSelection, ValueTask>[] handlers;
        var selectionChanged = false;

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            item = FindItem(state.EffectiveItems, itemId) ?? FindItem(state.AuxiliaryItems, itemId);
            if (item is null || !item.Visible || item.Disabled)
            {
                return false;
            }

            if (!string.Equals(state.SelectedItemId, item.Id, StringComparison.Ordinal))
            {
                state.SelectedItemId = item.Id;
                selectionChanged = true;
            }

            handlers = subscriptions.Values
                .Where(subscription => string.Equals(subscription.MenuId, menuId, StringComparison.Ordinal))
                .Select(subscription => subscription.Handler)
                .ToArray();
        }

        if (selectionChanged)
        {
            RaiseChanged(menuId, SideMenuStateChangeKind.Selection);
        }

        var selection = new SideMenuSelection(
            menuId,
            item.Id,
            item,
            source,
            DateTimeOffset.UtcNow);

        foreach (var handler in handlers)
        {
            await handler(selection);
        }

        return true;
    }

    public IDisposable Subscribe(
        string menuId,
        Func<SideMenuSelection, ValueTask> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        menuId = NormalizeMenuId(menuId);
        var id = Guid.NewGuid();

        lock (gate)
        {
            subscriptions[id] = new SelectionSubscription(menuId, handler);
        }

        return new SubscriptionReference(this, id);
    }

    internal SideMenuSnapshot EnsureMenu(string menuId, bool defaultExpanded)
    {
        menuId = NormalizeMenuId(menuId);

        lock (gate)
        {
            return CreateSnapshot(GetOrCreateState(menuId, defaultExpanded));
        }
    }

    internal void SetDeclaredItems(
        string menuId,
        IReadOnlyList<ISideMenuItem> primaryItems,
        IReadOnlyList<ISideMenuItem> auxiliaryItems)
    {
        ArgumentNullException.ThrowIfNull(primaryItems);
        ArgumentNullException.ThrowIfNull(auxiliaryItems);
        menuId = NormalizeMenuId(menuId);
        var nextPrimaryItems = primaryItems.ToArray();
        var nextAuxiliaryItems = auxiliaryItems.ToArray();
        SideMenuStateChangeKind changeKind;

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            ValidateItems(state.ItemsOverride ?? nextPrimaryItems, nextAuxiliaryItems, menuId);

            if (AreEquivalent(state.DeclaredItems, nextPrimaryItems)
                && AreEquivalent(state.AuxiliaryItems, nextAuxiliaryItems))
            {
                return;
            }

            state.DeclaredItems = nextPrimaryItems;
            state.AuxiliaryItems = nextAuxiliaryItems;
            changeKind = SideMenuStateChangeKind.Items;
            if (!IsSelectedItemAvailable(state))
            {
                state.SelectedItemId = null;
                changeKind |= SideMenuStateChangeKind.Selection;
            }
        }

        RaiseChanged(menuId, changeKind);
    }

    internal bool RestoreExpandedState(string menuId, bool isExpanded)
    {
        menuId = NormalizeMenuId(menuId);

        lock (gate)
        {
            var state = GetOrCreateState(menuId);
            if (state.HasExplicitExpandedState || state.IsExpanded == isExpanded)
            {
                return false;
            }

            state.IsExpanded = isExpanded;
        }

        RaiseChanged(menuId, SideMenuStateChangeKind.Expanded);
        return true;
    }

    private void Unsubscribe(Guid id)
    {
        lock (gate)
        {
            subscriptions.Remove(id);
        }
    }

    private MenuState GetOrCreateState(string menuId, bool defaultExpanded = true)
    {
        if (states.TryGetValue(menuId, out var state))
        {
            return state;
        }

        state = new MenuState(menuId, defaultExpanded);
        states.Add(menuId, state);
        return state;
    }

    private void RaiseChanged(string menuId, SideMenuStateChangeKind changeKind)
    {
        Changed?.Invoke(this, new SideMenuStateChangedEventArgs(menuId, changeKind));
    }

    private static SideMenuSnapshot CreateSnapshot(MenuState state)
    {
        return new SideMenuSnapshot(
            state.MenuId,
            state.EffectiveItems,
            state.SelectedItemId,
            state.IsExpanded,
            state.IsMobileMenuOpen,
            state.ItemsOverride is not null,
            state.HasExplicitExpandedState);
    }

    private static bool IsSelectedItemAvailable(MenuState state)
    {
        if (state.SelectedItemId is null)
        {
            return true;
        }

        var item = FindItem(state.EffectiveItems, state.SelectedItemId)
            ?? FindItem(state.AuxiliaryItems, state.SelectedItemId);
        return item is { Visible: true, Disabled: false };
    }

    private static ISideMenuItem? FindItem(IReadOnlyList<ISideMenuItem> items, string itemId)
    {
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

    private static void ValidateItems(
        IReadOnlyList<ISideMenuItem> primaryItems,
        IReadOnlyList<ISideMenuItem> auxiliaryItems,
        string menuId)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        foreach (var item in primaryItems.Concat(auxiliaryItems))
        {
            ValidateItem(item, ids, menuId);
        }
    }

    private static void ValidateItem(
        ISideMenuItem item,
        HashSet<string> ids,
        string menuId)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            throw new InvalidOperationException($"Side menu '{menuId}' contains an item without an id.");
        }

        if (!ids.Add(item.Id))
        {
            throw new InvalidOperationException($"Side menu '{menuId}' contains duplicate item id '{item.Id}'.");
        }

        foreach (var child in item.Children)
        {
            ValidateItem(child, ids, menuId);
        }
    }

    private static bool AreEquivalent(
        IReadOnlyList<ISideMenuItem> left,
        IReadOnlyList<ISideMenuItem> right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!AreEquivalent(left[index], right[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreEquivalent(ISideMenuItem left, ISideMenuItem right)
    {
        return string.Equals(left.Id, right.Id, StringComparison.Ordinal)
            && string.Equals(left.Text, right.Text, StringComparison.Ordinal)
            && string.Equals(left.Icon, right.Icon, StringComparison.Ordinal)
            && string.Equals(left.Description, right.Description, StringComparison.Ordinal)
            && string.Equals(left.BadgeText, right.BadgeText, StringComparison.Ordinal)
            && string.Equals(left.PanelTitle, right.PanelTitle, StringComparison.Ordinal)
            && left.Disabled == right.Disabled
            && left.Visible == right.Visible
            && left.OverflowBehavior == right.OverflowBehavior
            && left.Activation == right.Activation
            && (left.PanelContent is null) == (right.PanelContent is null)
            && Equals(left.Payload, right.Payload)
            && AreEquivalent(left.Children, right.Children);
    }

    private static string NormalizeMenuId(string menuId)
    {
        if (string.IsNullOrWhiteSpace(menuId))
        {
            throw new ArgumentException("A side menu id is required.", nameof(menuId));
        }

        return menuId.Trim();
    }

    private static string NormalizeItemId(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("A side menu item id is required.", nameof(itemId));
        }

        return itemId.Trim();
    }

    private sealed class MenuState(string menuId, bool defaultExpanded)
    {
        public string MenuId { get; } = menuId;

        public IReadOnlyList<ISideMenuItem> DeclaredItems { get; set; } = [];

        public IReadOnlyList<ISideMenuItem>? ItemsOverride { get; set; }

        public IReadOnlyList<ISideMenuItem> AuxiliaryItems { get; set; } = [];

        public IReadOnlyList<ISideMenuItem> EffectiveItems => ItemsOverride ?? DeclaredItems;

        public string? SelectedItemId { get; set; }

        public bool IsExpanded { get; set; } = defaultExpanded;

        public bool IsMobileMenuOpen { get; set; }

        public bool HasExplicitExpandedState { get; set; }
    }

    private sealed record SelectionSubscription(
        string MenuId,
        Func<SideMenuSelection, ValueTask> Handler);

    private sealed class SubscriptionReference(SideMenuService owner, Guid id) : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            owner.Unsubscribe(id);
        }
    }
}
