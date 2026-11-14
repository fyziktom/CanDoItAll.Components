using Bunit;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchNodeToolbarTests {
    [Fact]
    public void Toolbar_roles_round_trip_with_the_runtime_string_vocabulary() {
        var node = new CanvasWorkbenchNode {
            Id = "a",
            ToolbarActions = [new() { ActionId = "host:copy", KeyboardCommand = CanvasWorkbenchKeyboardCommand.CopyIdentifier }]
        };
        var json = SerializationPersistencePack.Serialize(node);
        Assert.Contains("\"keyboardCommand\":\"CopyIdentifier\"", json, StringComparison.Ordinal);
        var copy = SerializationPersistencePack.Deserialize<CanvasWorkbenchNode>(json);
        Assert.Equal(CanvasWorkbenchKeyboardCommand.CopyIdentifier, Assert.Single(copy!.ToolbarActions).KeyboardCommand);
        Assert.Empty(new CanvasWorkbenchNode().ToolbarActions);
        Assert.Equal(CanvasWorkbenchKeyboardCommand.None, new CanvasWorkbenchAction().KeyboardCommand);
    }

    [Fact]
    public async Task Entry_tracks_browser_selection_without_requiring_a_parent_state_callback() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.JSInterop.Setup<bool>("CanDoItAll.canvasWorkbench.create", _ => true).SetResult(true);
        var surface = new CanvasWorkbenchSurface {
            Nodes = [new() { Id = "a", ToolbarActions = [new() { ActionId = "inspect", Label = "Inspect" }] }, new() { Id = "b" }]
        };
        var canvas = context.Render<CanvasWorkbench>(parameters => parameters.Add(component => component.Surface, surface));
        var selector = "[data-cw-node-actions]";
        Assert.True(canvas.Find(selector).HasAttribute("disabled"));
        Assert.Contains("Select exactly one", canvas.Find(selector).GetAttribute("title"));

        await canvas.InvokeAsync(() => canvas.Instance.OnSelectionChanged("a", "[\"a\"]", 1));
        Assert.False(canvas.Find(selector).HasAttribute("disabled"));
        await canvas.Find(selector).ClickAsync(new MouseEventArgs());
        Assert.Single(context.JSInterop.Invocations, call => call.Identifier == "CanDoItAll.canvasWorkbench.toggleNodeToolbar");

        await canvas.InvokeAsync(() => canvas.Instance.OnSelectionChanged("a", "[\"a\",\"b\"]", 2));
        Assert.True(canvas.Find(selector).HasAttribute("disabled"));
        await canvas.InvokeAsync(() => canvas.Instance.OnSelectionChanged("a", "[\"a\"]", 1));
        Assert.True(canvas.Find(selector).HasAttribute("disabled"));

        surface.UiState.SelectedNodeIds = ["a"];
        canvas.Render(parameters => parameters.Add(component => component.Surface, surface));
        Assert.False(canvas.Find(selector).HasAttribute("disabled"));
        surface.Nodes[0].ToolbarActions.Clear();
        canvas.Render(parameters => parameters.Add(component => component.Surface, surface));
        Assert.Empty(canvas.FindAll(selector));
    }
}
