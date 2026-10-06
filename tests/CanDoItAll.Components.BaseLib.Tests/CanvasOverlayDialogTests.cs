using Bunit;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasOverlayDialogTests : BunitContext {
    [Fact]
    public void Maximize_is_instance_owned_and_stays_inside_its_canvas() {
        var first = Render<CanvasOverlayDialog>(parameters => parameters
            .Add(component => component.CanvasScoped, true)
            .Add(component => component.CanMaximize, true)
            .Add(component => component.FillBody, true));
        var second = Render<CanvasOverlayDialog>(parameters => parameters
            .Add(component => component.CanvasScoped, true)
            .Add(component => component.CanMaximize, true));

        first.Find("[data-testid='canvas-overlay-size-toggle']").Click();

        Assert.Equal("true", first.Find("[role='dialog']").GetAttribute("data-maximized"));
        Assert.Contains("position:absolute", first.Find(".canvas-overlay-backdrop").GetAttribute("style"));
        Assert.Contains("width:100%;height:100%", first.Find("[role='dialog']").GetAttribute("style"));
        Assert.Equal("false", second.Find("[role='dialog']").GetAttribute("data-maximized"));

        first.Find("[data-testid='canvas-overlay-size-toggle']").Click();
        Assert.Equal("false", first.Find("[role='dialog']").GetAttribute("data-maximized"));
    }

    [Fact]
    public async Task Backdrop_requests_close_without_hiding_the_owners_guarded_dialog() {
        var closeRequests = 0;
        var rendered = Render<CanvasOverlayDialog>(parameters => parameters
            .Add(component => component.Close, EventCallback.Factory.Create(this, () => closeRequests++))
            .Add(component => component.Body, builder => builder.AddContent(0, "Guarded editor"))
            .Add(component => component.FillBody, true));

        await rendered.Find(".canvas-overlay-backdrop").ClickAsync(new());

        Assert.Equal(1, closeRequests);
        Assert.Equal("Guarded editor", rendered.Find(".canvas-overlay-dialog__body--fill").TextContent);
        Assert.Single(rendered.FindAll("[role='dialog']"));
    }
}
