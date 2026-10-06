using System.Text.Json;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchComposerContractTests {
    [Fact]
    public async Task Optional_opening_callbacks_preserve_the_original_create_and_close_identity() {
        CanvasWorkbenchComposerOpening? opened = null;
        Guid? closed = null;
        var component = new ComposerCanvas(
            EventCallback.Factory.Create<CanvasWorkbenchComposerOpening>(this, value => opened = value),
            EventCallback.Factory.Create<Guid>(this, value => closed = value));
        var id = Guid.NewGuid();
        var request = new CanvasWorkbenchCreateActionRequest("create", "source", 0, -10.125, "original-parent", "", "", "", "child", "dialog", "note", null);
        await component.OnComposerOpened(JsonSerializer.Serialize(new CanvasWorkbenchComposerOpening(id, request, null)));
        await component.OnComposerClosed(id);
        Assert.Equal(id, opened!.OpeningId);
        Assert.Equal(request, opened.CreateRequest);
        Assert.Null(opened.EditRequest);
        Assert.Equal(id, closed);
    }

    [Fact]
    public async Task Submission_retains_opening_identity_and_untracked_consumers_remain_compatible() {
        var id = Guid.NewGuid();
        CanvasWorkbenchNodeEditRequest? edited = null;
        var component = new ComposerCanvas(EventCallback.Factory.Create<CanvasWorkbenchNodeEditRequest>(this, value => edited = value));
        await component.OnNodeEdited(JsonSerializer.Serialize(new CanvasWorkbenchNodeEditRequest("note", "Title", "Draft") { ComposerOpeningId = id }));
        Assert.Equal(id, edited!.ComposerOpeningId);
        await component.OnNodeEdited("""{"nodeId":"legacy","title":"Title","notes":"Draft"}""");
        Assert.Null(edited!.ComposerOpeningId);
        Assert.Equal("legacy", edited.NodeId);
    }

    private sealed class ComposerCanvas : CanvasWorkbench {
        public ComposerCanvas(EventCallback<CanvasWorkbenchComposerOpening> opened, EventCallback<Guid> closed) {
            ComposerOpened = opened;
            ComposerClosed = closed;
        }

        public ComposerCanvas(EventCallback<CanvasWorkbenchNodeEditRequest> edited) {
            NodeEdited = edited;
        }
    }
}
