using Bunit;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class WorkspaceTabStripBehaviorTests {
    private static readonly IReadOnlyList<WorkspaceTab> Tabs = [
        new() { Id = "home", Title = "Home", Href = "/home", IsPinned = true, CanClose = false },
        new() { Id = "a", Title = "Alpha", Href = "/projects/a?step=source", IsDirty = true },
        new() { Id = "b", Title = "Beta", Subtitle = "Projects / B" }
    ];

    private static BunitContext CreateContext() {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task Tabs_with_an_address_are_links_and_the_shown_tab_is_selected() {
        await using var context = CreateContext();
        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.ActiveId, "a"));

        var alpha = strip.Find("[data-tab-id='a'] [role='tab']");
        Assert.Equal("a", alpha.TagName, ignoreCase: true);
        Assert.Equal("/projects/a?step=source", alpha.GetAttribute("href"));
        Assert.Equal("true", alpha.GetAttribute("aria-selected"));
        Assert.Equal("page", alpha.GetAttribute("aria-current"));
        Assert.Single(strip.FindAll("[data-tab-id='a'] .ui-workspace-tab__dirty"));
        Assert.Contains("ui-workspace-tab--active", strip.Find("[data-tab-id='a']").ClassList);

        var beta = strip.Find("[data-tab-id='b'] [role='tab']");
        Assert.Equal("button", beta.TagName, ignoreCase: true);
        Assert.Equal("false", beta.GetAttribute("aria-selected"));
        Assert.Equal("Beta — Projects / B", beta.GetAttribute("title"));
        Assert.Equal("Open workspaces", strip.Find("[role='tablist']").GetAttribute("aria-label"));
    }

    [Fact]
    public async Task A_tab_without_an_address_raises_activate_and_close_reports_its_id() {
        await using var context = CreateContext();
        string? activated = null, closed = null;
        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.ActiveId, "a")
            .Add(component => component.Activate, (string id) => activated = id)
            .Add(component => component.Close, (string id) => closed = id));

        strip.Find("[data-tab-id='b'] [role='tab']").Click();
        strip.Find("[data-tab-id='b'] .ui-workspace-tab__close").Click();

        Assert.Equal("b", activated);
        Assert.Equal("b", closed);
    }

    [Fact]
    public async Task Pinned_and_unclosable_tabs_have_no_close_control_and_without_handlers_nothing_is_offered() {
        await using var context = CreateContext();
        var bare = context.Render<WorkspaceTabStrip>(parameters => parameters.Add(component => component.Items, Tabs));
        Assert.Empty(bare.FindAll(".ui-workspace-tab__close"));
        Assert.Empty(bare.FindAll(".ui-workspace-tab__menu-trigger"));

        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.Close, (string _) => { }));
        Assert.Empty(strip.FindAll("[data-tab-id='home'] .ui-workspace-tab__close"));
        Assert.Single(strip.FindAll("[data-tab-id='a'] .ui-workspace-tab__close"));
        Assert.Single(strip.FindAll("[data-tab-id='b'] .ui-workspace-tab__close"));
    }

    [Fact]
    public async Task The_tab_menu_offers_only_handled_commands_and_routes_them_to_the_tab() {
        await using var context = CreateContext();
        string? movedRight = null, pinned = null;
        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.ActiveId, "a")
            .Add(component => component.MoveRight, (string id) => movedRight = id)
            .Add(component => component.TogglePin, (string id) => pinned = id));

        strip.Find("[data-tab-id='a'] .ui-workspace-tab__menu-trigger").Click();
        var menu = strip.FindComponent<ContextMenu>();
        Assert.True(menu.Instance.IsOpen);
        Assert.Equal("Alpha", menu.Instance.Heading);
        Assert.Equal(["pin", "move-right"], menu.Instance.Items.Select(item => item.Id).ToArray());
        Assert.False(menu.Instance.Items.Single(item => item.Id == "move-right").Disabled);

        await strip.InvokeAsync(() => menu.Instance.ItemSelected.InvokeAsync("move-right"));
        Assert.Equal("a", movedRight);
        Assert.False(strip.FindComponent<ContextMenu>().Instance.IsOpen);

        strip.Find("[data-tab-id='b'] .ui-workspace-tab__menu-trigger").Click();
        menu = strip.FindComponent<ContextMenu>();
        Assert.True(menu.Instance.Items.Single(item => item.Id == "move-right").Disabled);
        await strip.InvokeAsync(() => menu.Instance.ItemSelected.InvokeAsync("pin"));
        Assert.Equal("b", pinned);
    }

    [Fact]
    public async Task Overview_and_reopen_menus_appear_when_useful() {
        await using var context = CreateContext();
        string? reopened = null;
        var cleared = false;
        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.OverviewThreshold, 3)
            .Add(component => component.RecentItems, [new WorkspaceTab { Id = "old", Title = "Old" }])
            .Add(component => component.Reopen, (string id) => reopened = id)
            .Add(component => component.ClearRecent, () => cleared = true));

        Assert.Contains("All tabs (3)", strip.Find("[data-menu='overview']").TextContent);
        strip.Find("[data-menu='recent']").Click();
        var menu = strip.FindComponent<ContextMenu>();
        Assert.Equal(["recent:old", "clear-recent"], menu.Instance.Items.Select(item => item.Id).ToArray());
        await strip.InvokeAsync(() => menu.Instance.ItemSelected.InvokeAsync("recent:old"));
        Assert.Equal("old", reopened);

        strip.Find("[data-menu='recent']").Click();
        await strip.InvokeAsync(() => strip.FindComponent<ContextMenu>().Instance.ItemSelected.InvokeAsync("clear-recent"));
        Assert.True(cleared);

        var few = context.Render<WorkspaceTabStrip>(parameters => parameters.Add(component => component.Items, Tabs.Take(2).ToList()));
        Assert.Empty(few.FindAll("[data-menu]"));
    }

    [Fact]
    public async Task Compact_density_and_consumer_attributes_reach_the_root() {
        await using var context = CreateContext();
        var strip = context.Render<WorkspaceTabStrip>(parameters => parameters
            .Add(component => component.Items, Tabs)
            .Add(component => component.Density, WorkspaceTabStripDensity.Compact)
            .AddUnmatched("data-testid", "project-tabs"));

        var root = strip.Find("[data-testid='project-tabs']");
        Assert.Contains("ui-workspace-tabs", root.ClassList);
        Assert.Contains("ui-workspace-tabs--compact", root.ClassList);
    }
}
