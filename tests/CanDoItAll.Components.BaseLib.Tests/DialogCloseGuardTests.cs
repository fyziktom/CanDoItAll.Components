using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class DialogCloseGuardTests {
    [Fact]
    public async Task Programmatic_close_and_close_all_preserve_a_declined_dialog() {
        using var service = new DialogService(new TestNavigation());
        var result = service.OpenAsync("Editor", _ => builder => builder.AddContent(0, "Draft"));
        var dialog = Assert.Single(service.Dialogs);
        dialog.BeforeClose = () => Task.FromResult(false);
        await dialog.CloseAsync();
        await service.CloseAllAsync();
        Assert.Same(dialog, Assert.Single(service.Dialogs));
        Assert.False(result.IsCompleted);
        dialog.BeforeClose = () => Task.FromResult(true);
        await dialog.CloseAsync("discarded");
        Assert.Equal("discarded", await result);
    }

    [Fact]
    public async Task Repeated_close_requests_share_one_pending_decision() {
        using var service = new DialogService(new TestNavigation());
        var result = service.OpenAsync("Editor", _ => builder => builder.AddContent(0, "Draft"));
        var dialog = Assert.Single(service.Dialogs);
        var decision = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        dialog.BeforeClose = () => {
            calls++;
            return decision.Task;
        };
        var first = dialog.CloseAsync();
        var second = dialog.CloseAsync();
        Assert.Equal(1, calls);
        decision.SetResult(false);
        await Task.WhenAll(first, second);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task A_declined_inner_editor_stops_close_all_before_its_parent() {
        using var service = new DialogService(new TestNavigation());
        var parent = service.OpenAsync("Picker", _ => builder => builder.AddContent(0, "Parent"));
        var child = service.OpenAsync("Editor", _ => builder => builder.AddContent(0, "Draft"));
        service.Dialogs[^1].BeforeClose = () => Task.FromResult(false);
        await service.CloseAllAsync();
        Assert.Equal(2, service.Dialogs.Count);
        Assert.False(parent.IsCompleted);
        Assert.False(child.IsCompleted);
    }

    [Fact]
    public async Task Guard_failure_is_observable_and_never_removes_the_editor() {
        using var service = new DialogService(new TestNavigation());
        var result = service.OpenAsync("Editor", _ => builder => builder.AddContent(0, "Draft"));
        var dialog = Assert.Single(service.Dialogs);
        dialog.BeforeClose = () => throw new InvalidOperationException("Decision failed");
        await Assert.ThrowsAsync<InvalidOperationException>(() => dialog.CloseAsync());
        Assert.Single(service.Dialogs);
        Assert.False(result.IsCompleted);
    }

    private sealed class TestNavigation : NavigationManager {
        public TestNavigation() => Initialize("http://localhost/", "http://localhost/prompt-gallery");

        protected override void NavigateToCore(string uri, bool forceLoad) {
            Uri = ToAbsoluteUri(uri).AbsoluteUri;
            NotifyLocationChanged(false);
        }
    }
}
