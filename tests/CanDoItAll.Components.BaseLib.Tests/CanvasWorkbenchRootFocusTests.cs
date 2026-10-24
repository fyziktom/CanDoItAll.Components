using Bunit;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchRootFocusTests {
    [Theory]
    [InlineData("active", "active")]
    [InlineData("missing", null)]
    [InlineData("", "child")]
    public async Task Focus_root_obeys_the_host_identity_and_preserves_unbound_behavior(string rootNodeId, string? expectedNodeId) {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.JSInterop.Setup<bool>("CanDoItAll.canvasWorkbench.create", _ => true).SetResult(true);
        var canvas = context.Render<CanvasWorkbench>(parameters => parameters.Add(component => component.Surface,
            new CanvasWorkbenchSurface {
                RootNodeId = rootNodeId,
                Chrome = new() { ShowFocusAction = true, FocusActionLabel = "Focus root" },
                Nodes = [
                    new() { Id = "child", Family = "root", ParentId = "active" },
                    new() { Id = "parent", Family = "root" },
                    new() { Id = "active", Family = "root", ParentId = "parent" }
                ]
            }));

        await canvas.Find("[aria-label='Focus root']").ClickAsync(new MouseEventArgs());

        var calls = context.JSInterop.Invocations.Where(call => call.Identifier == "CanDoItAll.canvasWorkbench.focusNode").ToArray();
        if (expectedNodeId is null) {
            Assert.Empty(calls);
        } else {
            Assert.NotEmpty(calls);
            Assert.All(calls, call => Assert.Equal(expectedNodeId, call.Arguments[1]));
        }
    }
}
