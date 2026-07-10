using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ContextMenuBehaviorTests
{
    [Fact]
    public async Task KeyboardNavigationSkipsDisabledItemsAndSelectsActiveItem()
    {
        var selectedId = string.Empty;
        var menu = new ContextMenu();
        SetParameter(menu, nameof(ContextMenu.IsOpen), true);
        SetParameter(menu, nameof(ContextMenu.Items), new ContextMenuItem[]
        {
            new() { Id = "first", Text = "First" },
            new() { Id = "disabled", Text = "Disabled", Disabled = true },
            new() { Id = "last", Text = "Last" }
        });
        SetParameter(menu, nameof(ContextMenu.ItemSelected), EventCallback.Factory.Create<string>(this, value => selectedId = value));

        Invoke(menu, "OnParametersSet");
        await InvokeAsync(menu, "HandleKeyDownAsync", new KeyboardEventArgs { Key = "ArrowDown" });
        await InvokeAsync(menu, "HandleKeyDownAsync", new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("last", selectedId);
    }

    [Fact]
    public async Task EscapeRequestsClose()
    {
        var closeCount = 0;
        var menu = new ContextMenu();
        SetParameter(menu, nameof(ContextMenu.IsOpen), true);
        SetParameter(menu, nameof(ContextMenu.Items), new ContextMenuItem[] { new() { Id = "first", Text = "First" } });
        SetParameter(menu, nameof(ContextMenu.OnClose), EventCallback.Factory.Create(this, () => closeCount++));

        Invoke(menu, "OnParametersSet");
        await InvokeAsync(menu, "HandleKeyDownAsync", new KeyboardEventArgs { Key = "Escape" });

        Assert.Equal(1, closeCount);
    }

    private static void Invoke(object target, string methodName)
        => target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, null);

    private static Task InvokeAsync(object target, string methodName, params object[] parameters)
        => (Task)target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, parameters)!;

    private static void SetParameter(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!.SetValue(target, value);
}
