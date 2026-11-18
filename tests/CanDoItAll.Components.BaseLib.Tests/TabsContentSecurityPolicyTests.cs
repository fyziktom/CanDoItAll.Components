using Bunit;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TabsContentSecurityPolicyTests {
    private const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Navigation/Tabs.razor.js";

    private static readonly RenderFragment TwoTabs = builder => {
        builder.OpenComponent<TabsItem>(0);
        builder.AddAttribute(1, nameof(TabsItem.Text), "Overview");
        builder.CloseComponent();
        builder.OpenComponent<TabsItem>(2);
        builder.AddAttribute(3, nameof(TabsItem.Text), "Details");
        builder.CloseComponent();
    };

    [Theory]
    [InlineData(TabPosition.Top, "horizontal")]
    [InlineData(TabPosition.Left, "vertical")]
    public async Task The_tab_list_has_no_inline_handler_and_the_module_guards_its_keys(TabPosition position, string orientation) {
        await using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("guardNavigationKeys", _ => true).SetVoidResult();

        var tabs = context.Render<Tabs>(parameters => parameters
            .Add(component => component.NoContent, true)
            .Add(component => component.TabPosition, position)
            .Add(component => component.ChildContent, TwoTabs));

        var list = tabs.Find("[role='tablist']");
        Assert.Equal(orientation, list.GetAttribute("aria-orientation"));
        Assert.DoesNotContain(
            tabs.FindAll("*"),
            element => element.Attributes.Any(attribute => attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase)));
        tabs.WaitForAssertion(() => Assert.Single(module.Invocations, invocation => invocation.Identifier == "guardNavigationKeys"));

        tabs.FindAll("[role='tab']")[1].Click();
        Assert.Single(module.Invocations, invocation => invocation.Identifier == "guardNavigationKeys");
    }

    [Fact]
    public void Tabs_render_and_select_when_a_strict_test_renderer_refuses_the_module() {
        using var context = new BunitContext();
        var selected = -1;
        var tabs = context.Render<Tabs>(parameters => parameters
            .Add(component => component.NoContent, true)
            .Add(component => component.SelectedIndexChanged, (int index) => selected = index)
            .Add(component => component.ChildContent, TwoTabs));

        tabs.FindAll("[role='tab']")[1].Click();

        Assert.Equal(1, selected);
        Assert.Equal("true", tabs.FindAll("[role='tab']")[1].GetAttribute("aria-selected"));
    }

    [Fact]
    public void The_module_guards_the_same_keys_the_inline_handler_did() {
        var source = File.ReadAllText(Path.Combine(
            FindRepoRoot(), "src", "CanDoItAll.Components.BaseLib", "Components", "Navigation", "Tabs.razor.js"));

        Assert.Contains("export function guardNavigationKeys(list)", source, StringComparison.Ordinal);
        Assert.Contains("list.getAttribute('aria-orientation') === 'vertical'", source, StringComparison.Ordinal);
        Assert.Contains("['ArrowUp', 'ArrowDown', 'Home', 'End', ' ']", source, StringComparison.Ordinal);
        Assert.Contains("['ArrowLeft', 'ArrowRight', 'Home', 'End', ' ']", source, StringComparison.Ordinal);
        Assert.Contains("event.preventDefault()", source, StringComparison.Ordinal);
    }

    private static string FindRepoRoot() {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx"))) {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Could not locate the repository root.");
    }
}
