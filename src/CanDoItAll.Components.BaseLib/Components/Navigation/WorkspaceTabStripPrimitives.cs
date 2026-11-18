namespace CanDoItAll.Components.BaseLib;

/// <summary>One opened workspace shown by <see cref="WorkspaceTabStrip"/>.</summary>
public sealed record WorkspaceTab
{
    /// <summary>Stable identity of the opened workspace; every tab event reports it.</summary>
    public required string Id { get; init; }

    /// <summary>Visible label. Long labels are truncated in the strip and shown in full in the tooltip and the menus.</summary>
    public required string Title { get; init; }

    /// <summary>
    /// Address that restores the workspace. When set, the tab is a link: a click navigates there (the host maps the new
    /// location back to its tab, so a bookmark or a reload restores the same tab) and Ctrl+click or a middle click opens
    /// it in a new browser tab. When empty, the tab is a button that raises <see cref="WorkspaceTabStrip.Activate"/>.
    /// </summary>
    public string? Href { get; init; }

    /// <summary>Optional icon token shown before the title.</summary>
    public string? Icon { get; init; }

    /// <summary>Optional secondary text, such as the workspace's group or location, shown in the tooltip and the menus.</summary>
    public string? Subtitle { get; init; }

    /// <summary>Shows a pin instead of the close control; the host decides what pinning keeps.</summary>
    public bool IsPinned { get; init; }

    /// <summary>Marks unsaved changes with a dot beside the title.</summary>
    public bool IsDirty { get; init; }

    /// <summary>False hides the close control, for example for a home workspace that always stays open.</summary>
    public bool CanClose { get; init; } = true;
}

/// <summary>Spacing of a <see cref="WorkspaceTabStrip"/>'s own chrome.</summary>
public enum WorkspaceTabStripDensity
{
    /// <summary>The default tab height and padding.</summary>
    Normal,

    /// <summary>Shorter tabs for a strip inside dense application chrome.</summary>
    Compact
}
