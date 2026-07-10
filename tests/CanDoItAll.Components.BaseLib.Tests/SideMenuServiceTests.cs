namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class SideMenuServiceTests
{
    [Fact]
    public async Task SelectionPublishesTypedItemAndSourceToScopedSubscribers()
    {
        var service = new SideMenuService();
        var item = new SideMenuItemDefinition
        {
            Id = "overview",
            Text = "Overview",
            Icon = "dashboard",
            Payload = 42
        };
        SideMenuSelection? received = null;
        service.SetItems("workspace", [item]);
        using var subscription = service.Subscribe("workspace", selection =>
        {
            received = selection;
            return ValueTask.CompletedTask;
        });

        var selected = await service.SelectAsync(
            "workspace",
            "overview",
            SideMenuSelectionSource.External);

        Assert.True(selected);
        Assert.NotNull(received);
        Assert.Same(item, received.Item);
        Assert.Equal("workspace", received.MenuId);
        Assert.Equal("overview", received.ItemId);
        Assert.Equal(SideMenuSelectionSource.External, received.Source);
        Assert.Equal(42, received.Item.Payload);
        Assert.Equal("overview", service.GetSnapshot("workspace").SelectedItemId);
    }

    [Fact]
    public async Task DynamicItemSwitchClearsSelectionThatNoLongerExists()
    {
        var service = new SideMenuService();
        service.SetItems("workspace", [Item("project", "Project")]);
        await service.SelectAsync("workspace", "project");

        service.SetItems("workspace", [Item("analytics", "Analytics")]);

        var snapshot = service.GetSnapshot("workspace");
        Assert.True(snapshot.HasItemsOverride);
        Assert.Null(snapshot.SelectedItemId);
        Assert.Equal("analytics", Assert.Single(snapshot.Items).Id);
    }

    [Fact]
    public void ExpansionStateCanBeControlledAndToggledExternally()
    {
        var service = new SideMenuService();

        Assert.True(service.SetExpanded("workspace", false));
        Assert.False(service.GetSnapshot("workspace").IsExpanded);
        Assert.True(service.GetSnapshot("workspace").HasExplicitExpandedState);

        Assert.True(service.ToggleExpanded("workspace"));
        Assert.True(service.GetSnapshot("workspace").IsExpanded);
        Assert.False(service.SetExpanded("workspace", true));
    }

    [Fact]
    public async Task DisabledAndHiddenItemsDoNotPublishSelection()
    {
        var service = new SideMenuService();
        var publishCount = 0;
        service.SetItems(
            "workspace",
            [
                Item("disabled", "Disabled") with { Disabled = true },
                Item("hidden", "Hidden") with { Visible = false }
            ]);
        using var subscription = service.Subscribe("workspace", _ =>
        {
            publishCount++;
            return ValueTask.CompletedTask;
        });

        Assert.False(await service.SelectAsync("workspace", "disabled"));
        Assert.False(await service.SelectAsync("workspace", "hidden"));
        Assert.Equal(0, publishCount);
    }

    [Fact]
    public void DuplicateIdsAcrossNestedItemsAreRejected()
    {
        var service = new SideMenuService();
        var duplicateTree = Item("parent", "Parent") with
        {
            Children = [Item("duplicate", "First"), Item("duplicate", "Second")]
        };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.SetItems("workspace", [duplicateTree]));

        Assert.Contains("duplicate item id 'duplicate'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DisposedSubscriptionStopsReceivingSelections()
    {
        var service = new SideMenuService();
        var publishCount = 0;
        service.SetItems("workspace", [Item("overview", "Overview")]);
        var subscription = service.Subscribe("workspace", _ =>
        {
            publishCount++;
            return ValueTask.CompletedTask;
        });

        await service.SelectAsync("workspace", "overview");
        subscription.Dispose();
        await service.SelectAsync("workspace", "overview");

        Assert.Equal(1, publishCount);
    }

    private static SideMenuItemDefinition Item(string id, string text)
        => new() { Id = id, Text = text };
}
