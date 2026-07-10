using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib;

public enum SideMenuDisplayMode
{
    Large,
    Medium,
    Small
}

public enum SideMenuOverflowBehavior
{
    Auto,
    PreferVisible,
    AlwaysInMore
}

public enum SideMenuItemActivation
{
    Auto,
    Select,
    OpenPanel
}

public enum SideMenuSelectionSource
{
    Pointer,
    Keyboard,
    External
}

[Flags]
public enum SideMenuStateChangeKind
{
    None = 0,
    Items = 1,
    Selection = 2,
    Expanded = 4,
    MobileMenu = 8
}

public interface ISideMenuItem
{
    string Id { get; }

    string Text { get; }

    string Icon { get; }

    string Description { get; }

    string BadgeText { get; }

    string PanelTitle { get; }

    bool Disabled { get; }

    bool Visible { get; }

    SideMenuOverflowBehavior OverflowBehavior { get; }

    SideMenuItemActivation Activation { get; }

    IReadOnlyList<ISideMenuItem> Children { get; }

    RenderFragment? PanelContent { get; }

    object? Payload { get; }
}

public sealed record SideMenuItemDefinition : ISideMenuItem
{
    public required string Id { get; init; }

    public required string Text { get; init; }

    public string Icon { get; init; } = "circle";

    public string Description { get; init; } = string.Empty;

    public string BadgeText { get; init; } = string.Empty;

    public string PanelTitle { get; init; } = string.Empty;

    public bool Disabled { get; init; }

    public bool Visible { get; init; } = true;

    public SideMenuOverflowBehavior OverflowBehavior { get; init; }

    public SideMenuItemActivation Activation { get; init; }

    public IReadOnlyList<ISideMenuItem> Children { get; init; } = [];

    public RenderFragment? PanelContent { get; init; }

    public object? Payload { get; init; }
}

public sealed record SideMenuSelection(
    string MenuId,
    string ItemId,
    ISideMenuItem Item,
    SideMenuSelectionSource Source,
    DateTimeOffset SelectedAtUtc);

public sealed record SideMenuSnapshot(
    string MenuId,
    IReadOnlyList<ISideMenuItem> Items,
    string? SelectedItemId,
    bool IsExpanded,
    bool IsMobileMenuOpen,
    bool HasItemsOverride,
    bool HasExplicitExpandedState);

public sealed class SideMenuStateChangedEventArgs(
    string menuId,
    SideMenuStateChangeKind changeKind) : EventArgs
{
    public string MenuId { get; } = menuId;

    public SideMenuStateChangeKind ChangeKind { get; } = changeKind;
}

internal enum SideMenuItemPlacement
{
    Primary,
    More,
    Bottom
}

public sealed class SideMenuLayoutMetrics
{
    public SideMenuDisplayMode DisplayMode { get; set; } = SideMenuDisplayMode.Large;

    public int VisibleItemCapacity { get; set; } = 8;
}

internal sealed class SideMenuInteropInitialization
{
    public SideMenuLayoutMetrics Metrics { get; set; } = new();

    public bool? StoredExpanded { get; set; }
}
