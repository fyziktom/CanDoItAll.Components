using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TreeViewAdaptiveTextTests
{
    [Fact]
    public void CompactBadgeUsesThreeGraphemeClustersAndThreeDots()
    {
        var row = CreateRow("Label", "A\u0308BCDE", "Details");

        var compact = Invoke<string>(row, "ResolveCompactBadgeText");

        Assert.Equal("A\u0308BC...", compact);
    }

    [Fact]
    public void ExplicitTooltipIsAuthoritativeForDomainDetails()
    {
        var row = CreateRow("Project configuration", "ProjectConfiguration", "Simulator-owned definition");

        var details = Invoke<string>(row, "ResolveTooltipDetails");

        Assert.Equal("Simulator-owned definition", details);
    }

    [Fact]
    public void TooltipFallbackIncludesTheFullLabelAndBadge()
    {
        var row = CreateRow("Project configuration", "ProjectConfiguration", string.Empty);

        var details = Invoke<string>(row, "ResolveTooltipDetails");

        Assert.Equal("Project configuration · ProjectConfiguration", details);
    }

    [Fact]
    public void RenderedRowUsesRightTooltipFullAccessibleTextAndBothBadgeVariants()
    {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton<TooltipService>();
        var node = new TreeViewNode
        {
            Id = "project-configuration",
            Text = "Project configuration with a deliberately long title",
            BadgeText = "DocumentationReviewRequired",
            Tooltip = "Complete project configuration details",
            DataTestId = "adaptive-project-row"
        };

        var cut = context.Render<TreeView>(parameters => parameters
            .Add(component => component.Items, new[] { node }));
        var target = Assert.Single(cut.FindComponents<TooltipTarget>());
        var row = cut.Find("[data-testid='adaptive-project-row']");

        Assert.Equal(TooltipPosition.Right, target.Instance.Position);
        Assert.Equal(
            "Project configuration with a deliberately long title DocumentationReviewRequired",
            row.GetAttribute("aria-label"));
        Assert.Equal("Complete project configuration details", row.GetAttribute("aria-description"));
        Assert.Equal("DocumentationReviewRequired", row.QuerySelector(".cda-treeview__badge-full")!.TextContent);
        Assert.Equal("Doc...", row.QuerySelector(".cda-treeview__badge-compact")!.TextContent);

        target.Find(".cda-treeview__tooltip-target").MouseEnter(new MouseEventArgs { ClientX = 40, ClientY = 80 });
        var service = context.Services.GetRequiredService<TooltipService>();
        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(service.Current);
            Assert.Equal("Complete project configuration details", service.Current!.Text);
            Assert.Equal(TooltipPosition.Right, service.Current.Options.Position);
        });

        target.Find(".cda-treeview__tooltip-target").MouseLeave();
        Assert.Null(service.Current);
    }

    private static TreeViewNodeRow CreateRow(string text, string badgeText, string tooltip)
    {
        var row = new TreeViewNodeRow();
        row.GetType().GetProperty(nameof(TreeViewNodeRow.Node))!.SetValue(row, new TreeViewNode
        {
            Id = "node",
            Text = text,
            BadgeText = badgeText,
            Tooltip = tooltip
        });
        return row;
    }

    private static T Invoke<T>(object target, string methodName)
        => (T)target.GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(target, null)!;
}
