using System.Text.Json;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchComposerContractTests {
    [Fact]
    public async Task Optional_comparison_callback_keeps_actual_input_revision_and_unfinished_raw_draft() {
        CanvasWorkbenchComposerDraft? observed = null;
        var component = new ComposerCanvas(comparison: EventCallback.Factory.Create<CanvasWorkbenchComposerDraft>(this, value => observed = value));
        var opening = Guid.NewGuid();
        var draft = new CanvasWorkbenchComposerDraft(opening, 17, true, false, null,
            new("original-node", "Original title", " Unfinished Unicode draft \u017Elu\u0165ou\u010Dk\u00FD\n2e "));

        await component.OnComposerComparisonRequested(JsonSerializer.Serialize(draft));

        Assert.Equal(draft, observed);
        Assert.Equal(opening, observed!.OpeningId);
        Assert.Equal(17, observed.Revision);
        Assert.True(observed.HasPendingInput);
        Assert.False(observed.CanApply);
        Assert.Equal(draft.EditRequest!.Notes, observed.EditRequest!.Notes);
    }

    [Fact]
    public async Task Local_apply_reports_retirement_when_no_original_scene_is_mounted() {
        var component = new ComposerCanvas();

        var result = await component.ApplyComposerDraftAsync(new(Guid.NewGuid(), 17, null,
            new("original-node", "Reviewed title", "Reviewed body") { HasExplicitTitle = true }));

        Assert.Equal(CanvasWorkbenchComposerApplyStatus.Retired, result.Status);
        Assert.Null(await component.ReadComposerDraftAsync());
    }

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
        var submissionId = Guid.NewGuid();
        CanvasWorkbenchNodeEditRequest? edited = null;
        var component = new ComposerCanvas(EventCallback.Factory.Create<CanvasWorkbenchNodeEditRequest>(this, value => edited = value));
        await component.OnNodeEdited(JsonSerializer.Serialize(new CanvasWorkbenchNodeEditRequest("note", "Title", "Draft") {
            ComposerOpeningId = id, ComposerSubmissionId = submissionId
        }));
        Assert.Equal(id, edited!.ComposerOpeningId);
        Assert.Equal(submissionId, edited.ComposerSubmissionId);
        await component.OnNodeEdited("""{"nodeId":"legacy","title":"Title","notes":"Draft"}""");
        Assert.Null(edited!.ComposerOpeningId);
        Assert.Equal("legacy", edited.NodeId);
    }

    [Fact]
    public async Task Optional_close_decision_is_native_and_absent_or_unmounted_handlers_cannot_approve_discard() {
        var component = new ComposerCanvas();
        var request = new CanvasWorkbenchComposerCloseRequest(Guid.NewGuid(), 17, true, false);
        Assert.False(await component.OnComposerCloseRequested(JsonSerializer.Serialize(request)));
        Assert.Null(await component.ReadComposerCloseRequestAsync());
        CanvasWorkbenchComposerCloseRequest? observed = null;
        component = new ComposerCanvas(confirmClose: value => {
            observed = value;
            return Task.FromResult(true);
        });

        Assert.True(await component.OnComposerCloseRequested(JsonSerializer.Serialize(request)));

        Assert.Equal(request, observed);
    }

    private sealed class ComposerCanvas : CanvasWorkbench {
        public ComposerCanvas(EventCallback<CanvasWorkbenchComposerOpening> opened, EventCallback<Guid> closed) {
            ComposerOpened = opened;
            ComposerClosed = closed;
        }

        public ComposerCanvas(EventCallback<CanvasWorkbenchNodeEditRequest> edited = default,
            EventCallback<CanvasWorkbenchComposerDraft> comparison = default,
            Func<CanvasWorkbenchComposerCloseRequest, Task<bool>>? confirmClose = null) {
            NodeEdited = edited;
            ComposerComparisonRequested = comparison;
            ConfirmComposerClose = confirmClose;
        }
    }
}
