using Bunit;
using CanDoItAll.Components.CanvasLib;
using CanDoItAll.Components.OverlayLib;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class SharedCanvasDisposalTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Retired_overlay_releases_callback_when_browser_cleanup_cannot_complete(bool disconnected) {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cleanup = context.JSInterop.SetupVoid("CanDoItAll.overlayWindow.dispose", _ => true);
        cleanup.SetException<Exception>(disconnected
            ? new JSDisconnectedException("Retired circuit")
            : new TaskCanceledException("Retired browser cleanup"));
        var window = context.Render<OverlayWindow>();
        var callback = (DotNetObjectReference<OverlayWindow>)context.JSInterop.Invocations
            .Single(call => call.Identifier == "CanDoItAll.overlayWindow.create").Arguments[1]!;

        await window.InvokeAsync(() => window.Instance.DisposeAsync().AsTask());
        await window.InvokeAsync(() => window.Instance.DisposeAsync().AsTask());

        Assert.Single(cleanup.Invocations);
        Assert.Throws<ObjectDisposedException>(() => callback.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Retired_mirror_disposes_once_when_browser_cleanup_cannot_complete(bool disconnected) {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cleanup = context.JSInterop.SetupVoid("CanDoItAll.accessibilityMirrorLayer.dispose", _ => true);
        cleanup.SetException<Exception>(disconnected
            ? new JSDisconnectedException("Retired circuit")
            : new TaskCanceledException("Retired browser cleanup"));
        var mirror = context.Render<AccessibilityMirrorLayer>(parameters => parameters
            .Add(component => component.Snapshot, new AccessibilityMirrorSnapshot()));

        await mirror.InvokeAsync(() => mirror.Instance.DisposeAsync().AsTask());
        await mirror.InvokeAsync(() => mirror.Instance.DisposeAsync().AsTask());

        Assert.Single(cleanup.Invocations);
    }

    [Fact]
    public async Task Unexpected_overlay_cleanup_failure_is_observable_and_still_releases_callback() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var failure = new InvalidOperationException("Unexpected cleanup failure");
        var cleanup = context.JSInterop.SetupVoid("CanDoItAll.overlayWindow.dispose", _ => true);
        cleanup.SetException(failure);
        var window = context.Render<OverlayWindow>();
        var callback = (DotNetObjectReference<OverlayWindow>)context.JSInterop.Invocations
            .Single(call => call.Identifier == "CanDoItAll.overlayWindow.create").Arguments[1]!;

        var observed = await Assert.ThrowsAsync<InvalidOperationException>(
            () => window.InvokeAsync(() => window.Instance.DisposeAsync().AsTask()));
        await window.InvokeAsync(() => window.Instance.DisposeAsync().AsTask());

        Assert.Same(failure, observed);
        Assert.Single(cleanup.Invocations);
        Assert.Throws<ObjectDisposedException>(() => callback.Value);
    }

    [Fact]
    public async Task Unexpected_mirror_cleanup_failure_is_observable_and_not_retried() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var failure = new InvalidOperationException("Unexpected cleanup failure");
        var cleanup = context.JSInterop.SetupVoid("CanDoItAll.accessibilityMirrorLayer.dispose", _ => true);
        cleanup.SetException(failure);
        var mirror = context.Render<AccessibilityMirrorLayer>(parameters => parameters
            .Add(component => component.Snapshot, new AccessibilityMirrorSnapshot()));

        var observed = await Assert.ThrowsAsync<InvalidOperationException>(
            () => mirror.InvokeAsync(() => mirror.Instance.DisposeAsync().AsTask()));
        await mirror.InvokeAsync(() => mirror.Instance.DisposeAsync().AsTask());

        Assert.Same(failure, observed);
        Assert.Single(cleanup.Invocations);
    }

    [Fact]
    public void Active_overlay_update_failure_remains_observable() {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var window = context.Render<OverlayWindow>();
        var failure = new JSException("Active update failure");
        context.JSInterop.SetupVoid("CanDoItAll.overlayWindow.update", _ => true).SetException(failure);

        var observed = Record.Exception(() => window.Render(parameters => parameters
            .Add(component => component.State, new OverlayWindowState { Left = 40 })));

        Assert.Same(failure, observed);
    }

    [Fact]
    public void Active_mirror_update_failure_remains_observable() {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var mirror = context.Render<AccessibilityMirrorLayer>(parameters => parameters
            .Add(component => component.Snapshot, new AccessibilityMirrorSnapshot()));
        var failure = new JSException("Active update failure");
        context.JSInterop.SetupVoid("CanDoItAll.accessibilityMirrorLayer.update", _ => true).SetException(failure);

        var observed = Record.Exception(() => mirror.Render(parameters => parameters
            .Add(component => component.Snapshot, new AccessibilityMirrorSnapshot { SummaryLabel = "Updated selection" })));

        Assert.Same(failure, observed);
    }
}
