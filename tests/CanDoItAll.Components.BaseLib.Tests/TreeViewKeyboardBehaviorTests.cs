using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TreeViewKeyboardBehaviorTests
{
    [Fact]
    public async Task KeyboardMovesIntoChildSelectsItAndReturnsToParent()
    {
        var selectedId = string.Empty;
        var toggledId = string.Empty;
        var tree = new TreeView();
        SetParameter(tree, nameof(TreeView.Items), new TreeViewNode[]
        {
            new()
            {
                Id = "branch",
                Text = "Branch",
                IsExpanded = true,
                Children = [new TreeViewNode { Id = "child", Text = "Child" }]
            }
        });
        SetParameter(tree, nameof(TreeView.OnSelect), EventCallback.Factory.Create<string>(this, value => selectedId = value));
        SetParameter(tree, nameof(TreeView.OnToggle), EventCallback.Factory.Create<string>(this, value => toggledId = value));

        Invoke(tree, "OnParametersSet");
        await KeyAsync(tree, "ArrowRight");
        await KeyAsync(tree, "Enter");

        Assert.Equal("child", selectedId);

        await KeyAsync(tree, "ArrowLeft");
        await KeyAsync(tree, "ArrowLeft");

        Assert.Equal("branch", ReadField<string>(tree, "activeNodeId"));
        Assert.Equal("branch", toggledId);
    }

    [Fact]
    public async Task HomeEndAndTypeAheadChangeActiveDescendant()
    {
        var tree = new TreeView();
        SetParameter(tree, nameof(TreeView.Items), new TreeViewNode[]
        {
            new() { Id = "alpha", Text = "Alpha" },
            new() { Id = "beta", Text = "Beta" },
            new() { Id = "gamma", Text = "Gamma" }
        });

        Invoke(tree, "OnParametersSet");
        await KeyAsync(tree, "End");
        Assert.Equal("gamma", ReadField<string>(tree, "activeNodeId"));

        await KeyAsync(tree, "Home");
        await KeyAsync(tree, "b");
        Assert.Equal("beta", ReadField<string>(tree, "activeNodeId"));
    }

    private static Task KeyAsync(TreeView tree, string key)
        => (Task)tree.GetType()
            .GetMethod("HandleTreeKeyDownAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(tree, [new KeyboardEventArgs { Key = key }])!;

    private static void Invoke(object target, string methodName)
        => target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, null);

    private static T? ReadField<T>(object target, string fieldName)
        => (T?)target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target);

    private static void SetParameter(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!.SetValue(target, value);
}
