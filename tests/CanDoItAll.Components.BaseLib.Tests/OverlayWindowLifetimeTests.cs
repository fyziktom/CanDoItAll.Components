using Bunit;
using CanDoItAll.Components.OverlayLib;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class OverlayWindowLifetimeTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Geometry_preserves_visibility_and_minimized_dimensions(bool minimized) {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var changes = new List<OverlayWindowState>();
        var window = context.Render<OverlayWindow>(parameters => parameters
            .Add(component => component.State, new() { IsMinimized = minimized, Width = 360, Height = 480 })
            .Add(component => component.StateChanged, changes.Add));

        await window.InvokeAsync(() => window.Instance.OnGeometryChanged(40, 50, 400, 600));

        var change = Assert.Single(changes);
        Assert.True(change.IsVisible);
        Assert.Equal(minimized, change.IsMinimized);
        Assert.Equal(40, change.Left);
        Assert.Equal(50, change.Top);
        Assert.Equal(minimized ? 360 : 400, change.Width);
        Assert.Equal(minimized ? 480 : 600, change.Height);
    }

    [Fact]
    public async Task Hidden_window_ignores_delayed_geometry() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var changes = new List<OverlayWindowState>();
        var window = context.Render<OverlayWindow>(parameters => parameters
            .Add(component => component.State, new() { IsVisible = false })
            .Add(component => component.StateChanged, changes.Add));

        await window.InvokeAsync(() => window.Instance.OnGeometryChanged(40, 50, 400, 600));

        Assert.Empty(changes);
    }

    [Fact]
    public async Task Geometry_cannot_reopen_a_window_while_hide_is_pending() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var changes = new List<OverlayWindowState>();
        var window = context.Render<OverlayWindow>(parameters => parameters
            .Add(component => component.StateChanged, async state => {
                changes.Add(state);
                entered.TrySetResult();
                await release.Task;
            }));
        var hiding = window.InvokeAsync(() => window.Find("[aria-label='Hide window']").ClickAsync(new MouseEventArgs()));
        try {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await window.InvokeAsync(() => window.Instance.OnGeometryChanged(40, 50, 400, 600)).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(Assert.Single(changes).IsVisible);
        } finally {
            release.TrySetResult();
            await hiding;
        }
    }

    [Fact]
    public async Task Geometry_is_retired_before_asynchronous_disposal_finishes() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cleanup = context.JSInterop.SetupVoid("CanDoItAll.overlayWindow.dispose", _ => true);
        var changes = new List<OverlayWindowState>();
        var window = context.Render<OverlayWindow>(parameters => parameters
            .Add(component => component.StateChanged, changes.Add));
        var disposing = window.InvokeAsync(() => window.Instance.DisposeAsync().AsTask());
        try {
            window.WaitForAssertion(() => Assert.Single(cleanup.Invocations));
            Assert.False(disposing.IsCompleted);
            await window.InvokeAsync(() => window.Instance.OnGeometryChanged(40, 50, 400, 600));
            Assert.Empty(changes);
        } finally {
            cleanup.SetVoidResult();
            await disposing;
        }
    }

    [Fact]
    public async Task Rejected_hide_remains_visible_and_allows_geometry_changes() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var failure = new InvalidOperationException("Synthetic host rejection");
        var changes = new List<OverlayWindowState>();
        var window = context.Render<OverlayWindow>(parameters => parameters
            .Add(component => component.StateChanged, state => {
                if (!state.IsVisible) {
                    throw failure;
                }
                changes.Add(state);
            }));

        var observed = await Assert.ThrowsAsync<InvalidOperationException>(() => window.InvokeAsync(
            () => window.Find("[aria-label='Hide window']").ClickAsync(new MouseEventArgs())));
        Assert.Same(failure, observed);
        await window.InvokeAsync(() => window.Instance.OnGeometryChanged(40, 50, 400, 600));
        Assert.True(Assert.Single(changes).IsVisible);
    }
}
