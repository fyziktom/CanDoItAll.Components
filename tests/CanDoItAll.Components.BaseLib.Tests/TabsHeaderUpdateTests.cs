using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TabsHeaderUpdateTests {
    [Theory]
    [InlineData(TabRenderMode.Server)]
    [InlineData(TabRenderMode.Client)]
    public void Replaced_panel_snapshot_refreshes_without_changing_the_header_or_selection(TabRenderMode mode) {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddLogging();
        context.Services.AddCanDoItAllBaseLib();
        var definitionRenders = 0;
        RenderFragment Items(string snapshot) => builder => {
            Assert.InRange(++definitionRenders, 1, 10);
            var capturedSnapshot = snapshot;
            builder.OpenComponent<TabsItem>(0);
            builder.AddAttribute(1, nameof(TabsItem.Text), "Skills");
            builder.AddAttribute(2, nameof(TabsItem.ChildContent), (RenderFragment)(panel => panel.AddContent(0, capturedSnapshot)));
            builder.CloseComponent();
        };
        var cut = context.Render<Tabs>(p => p.Add(x => x.RenderMode, mode).Add(x => x.ChildContent, Items("Empty catalog")));
        var originalItem = cut.FindComponent<TabsItem>().Instance;
        var originalButtonId = cut.Find("[role='tab']").Id;
        Assert.Equal("Empty catalog", cut.Find("[role='tabpanel']").TextContent);

        cut.Render(p => p.Add(x => x.ChildContent, Items("New skill")));

        cut.WaitForAssertion(() => {
            Assert.Equal("New skill", cut.Find("[role='tabpanel']").TextContent);
            Assert.Equal("true", cut.Find("[role='tab']").GetAttribute("aria-selected"));
            Assert.Equal(originalButtonId, cut.Find("[role='tab']").Id);
            Assert.Same(originalItem, cut.FindComponent<TabsItem>().Instance);
        });
    }

    [Fact]
    public void Changed_item_header_refreshes_its_button_without_replacing_the_item_or_selection() {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddLogging();
        context.Services.AddCanDoItAllBaseLib();
        var title = "Skills";
        var badge = "0";
        RenderFragment items = builder => {
            builder.OpenComponent<TabsItem>(0);
            builder.AddAttribute(1, nameof(TabsItem.Text), "Overview");
            builder.CloseComponent();
            builder.OpenComponent<TabsItem>(2);
            builder.AddAttribute(3, nameof(TabsItem.Text), title);
            builder.AddAttribute(4, nameof(TabsItem.BadgeText), badge);
            builder.CloseComponent();
        };
        var cut = context.Render<Tabs>(p => p.Add(x => x.SelectedIndex, 1).Add(x => x.ChildContent, items));
        var originalItem = cut.FindComponents<TabsItem>()[1].Instance;
        var originalButtonId = cut.FindAll("[role='tab']")[1].Id;
        Assert.Equal("0", cut.Find(".cad-tabs__badge").TextContent);

        title = "Validated skills";
        badge = "1";
        cut.Render(p => p.Add(x => x.ChildContent, items));

        cut.WaitForAssertion(() => {
            var button = cut.FindAll("[role='tab']")[1];
            Assert.Contains("Validated skills", button.TextContent, StringComparison.Ordinal);
            Assert.Equal("1", cut.Find(".cad-tabs__badge").TextContent);
            Assert.Equal("true", button.GetAttribute("aria-selected"));
            Assert.Equal(originalButtonId, button.Id);
            Assert.Same(originalItem, cut.FindComponents<TabsItem>()[1].Instance);
        });
    }
}
