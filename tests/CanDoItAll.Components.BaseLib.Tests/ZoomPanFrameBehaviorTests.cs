using Bunit;
using CanDoItAll.Components.Mermaid;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ZoomPanFrameBehaviorTests
{
    private const string ModulePath =
        "./_content/CanDoItAll.Components.BaseLib/Components/Layout/ZoomPanFrame.razor.js";

    [Fact]
    public void FrameRendersAccessibleViewportAndBuiltInControls()
    {
        using var context = CreateLooseContext();
        var cut = context.Render<ZoomPanFrame>(parameters => parameters
            .Add(component => component.AriaLabel, "Image preview")
            .Add(component => component.ChildContent, Markup("<img src=\"preview.png\" alt=\"Preview\" />")));

        var root = cut.Find("[data-cda-zoom-pan-frame]");
        var viewport = cut.Find("[data-cda-zoom-pan-viewport]");

        Assert.Contains("cda-zoom-pan-frame", root.ClassList);
        Assert.Equal("group", viewport.GetAttribute("role"));
        Assert.Equal("Image preview", viewport.GetAttribute("aria-label"));
        Assert.Equal("0", viewport.GetAttribute("tabindex"));
        Assert.Equal(3, cut.FindAll("[role='toolbar'] button").Count);
        Assert.Equal("Preview", cut.Find("img").GetAttribute("alt"));
    }

    [Fact]
    public void DisabledFrameRemovesToolbarAndViewportTabStop()
    {
        using var context = CreateLooseContext();
        var cut = context.Render<ZoomPanFrame>(parameters => parameters
            .Add(component => component.Enabled, false)
            .Add(component => component.ChildContent, Markup("<svg aria-label=\"Diagram\"></svg>")));

        Assert.Empty(cut.FindAll("[role='toolbar']"));
        Assert.False(cut.Find("[data-cda-zoom-pan-viewport]").HasAttribute("tabindex"));
    }

    [Fact]
    public void ContentInteractionSuppressionIsExplicitInMarkup()
    {
        using var context = CreateLooseContext();
        var cut = context.Render<ZoomPanFrame>(parameters => parameters
            .Add(component => component.SuppressContentInteraction, true)
            .Add(component => component.ChildContent, Markup("<button type=\"button\">Node</button>")));

        Assert.Contains("is-content-suppressed", cut.Find("[data-cda-zoom-pan-frame]").ClassList);
        Assert.True(cut.Find(".cda-zoom-pan-frame__content").HasAttribute("inert"));
    }

    [Fact]
    public void ResetKeyUsesValueEqualityForTypedContentIdentity()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("initialize", static _ => true).SetVoidResult();
        module.SetupVoid("reset", static _ => true).SetVoidResult();
        module.SetupVoid("destroy", static _ => true).SetVoidResult();

        var identity = new PreviewIdentity("file-7", 3, 11);
        var cut = context.Render<ZoomPanFrame>(parameters => parameters
            .Add(component => component.ResetKey, identity)
            .Add(component => component.ChildContent, Markup("<img src=\"preview.png\" alt=\"\" />")));

        cut.Render(parameters => parameters
            .Add(component => component.ResetKey, new PreviewIdentity("file-7", 3, 11)));

        Assert.DoesNotContain(module.Invocations, static invocation => invocation.Identifier == "reset");

        cut.Render(parameters => parameters
            .Add(component => component.ResetKey, new PreviewIdentity("file-7", 4, 11)));

        Assert.Single(module.Invocations, static invocation => invocation.Identifier == "reset");
    }

    [Theory]
    [InlineData(0, 8, 1.2, 48, nameof(ZoomPanFrame.MinimumZoom))]
    [InlineData(0.25, 0.9, 1.2, 48, nameof(ZoomPanFrame.MaximumZoom))]
    [InlineData(0.25, 8, 1, 48, nameof(ZoomPanFrame.ZoomFactor))]
    [InlineData(0.25, 8, 1.2, 0, nameof(ZoomPanFrame.KeyboardPanStep))]
    public void InvalidInteractionBoundsFailPredictably(
        double minimumZoom,
        double maximumZoom,
        double zoomFactor,
        double keyboardPanStep,
        string parameterName)
    {
        using var context = CreateLooseContext();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            context.Render<ZoomPanFrame>(parameters => parameters
                .Add(component => component.MinimumZoom, minimumZoom)
                .Add(component => component.MaximumZoom, maximumZoom)
                .Add(component => component.ZoomFactor, zoomFactor)
                .Add(component => component.KeyboardPanStep, keyboardPanStep)
                .Add(component => component.ChildContent, Markup("<span>Preview</span>"))));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Fact]
    public void JavascriptContractCoversLimitsTouchKeyboardAndCleanup()
    {
        var source = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "TestAssets",
            "ZoomPanFrame.razor.js"));

        Assert.Contains("if (changed) {\n    event.preventDefault();", source, StringComparison.Ordinal);
        Assert.Contains("if (event.deltaY === 0) {\n    return;", source, StringComparison.Ordinal);
        Assert.Contains("clientPoint(state, state.pinchMidpoint.x, state.pinchMidpoint.y)", source, StringComparison.Ordinal);
        Assert.Contains("'pointercancel'", source, StringComparison.Ordinal);
        Assert.Contains("case 'ArrowLeft':", source, StringComparison.Ordinal);
        Assert.Contains("[data-cda-zoom-pan-interactive]", source, StringComparison.Ordinal);
        Assert.Contains("state.resizeObserver?.disconnect();", source, StringComparison.Ordinal);
    }

    [Fact]
    public void CssContractConstrainsContentTracksToTheViewport()
    {
        var source = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "TestAssets",
            "ZoomPanFrame.razor.css"));

        Assert.Contains("grid-template-columns: minmax(0, 1fr);", source, StringComparison.Ordinal);
        Assert.Contains("grid-template-rows: minmax(0, 1fr);", source, StringComparison.Ordinal);
    }

    [Fact]
    public void WheelModeValuesMatchTheJavascriptProtocol()
    {
        Assert.Equal(0, (int)ZoomPanWheelMode.Disabled);
        Assert.Equal(1, (int)ZoomPanWheelMode.Zoom);
        Assert.Equal(2, (int)ZoomPanWheelMode.ControlKey);
    }

    [Fact]
    public void MermaidUsesSharedFrameWhileKeepingHeaderControls()
    {
        using var context = CreateLooseContext();
        var cut = context.Render<MermaidDiagram>(parameters => parameters
            .Add(component => component.Title, "Flow")
            .Add(component => component.Source, "flowchart LR\nA --> B"));

        var frame = cut.FindComponent<ZoomPanFrame>();

        Assert.True(frame.Instance.Enabled);
        Assert.False(frame.Instance.ShowControls);
        Assert.Equal(3, cut.FindAll(".cda-mermaid__toolbar button").Count);
    }

    private static BunitContext CreateLooseContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static RenderFragment Markup(string value)
        => builder => builder.AddMarkupContent(0, value);

    private readonly record struct PreviewIdentity(string FileId, long ContentRevision, long EditRevision);
}
