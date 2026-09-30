using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class DialogRetirementTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Active_close_fault_is_observable_and_does_not_retire_the_dialog(bool disconnected) {
        using var context = new BunitContext();
        Exception failure = disconnected ? new JSDisconnectedException("Active circuit failure") : new JSException("Active close failure");
        var module = new ControlledModule { CloseFailure = failure };
        context.Services.AddSingleton<IJSRuntime>(new Runtime(module));
        var dialog = context.Render<Dialog>(parameters => parameters.Add(value => value.IsOpen, true));

        var observed = Record.Exception(() => dialog.Render(parameters => parameters.Add(value => value.IsOpen, false)));

        Assert.Same(failure, observed);
        Assert.Equal(0, module.Disposals);
        Assert.Same(dialog.Instance, module.Callback!.Value);
        module.CloseFailure = null;
        await dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync());
        Assert.Equal(1, module.Disposals);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Retired_close_always_releases_module_and_callback(bool disconnected) {
        using var context = new BunitContext();
        var module = new ControlledModule {
            CloseFailure = disconnected ? new JSDisconnectedException("Retired circuit") : new TaskCanceledException("Retired close")
        };
        context.Services.AddSingleton<IJSRuntime>(new Runtime(module));
        var dialog = context.Render<Dialog>(parameters => parameters.Add(value => value.IsOpen, true));
        var callback = module.Callback!;

        await dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync());
        await dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync());

        Assert.Equal(1, module.Closes);
        Assert.Equal(1, module.Disposals);
        Assert.Throws<ObjectDisposedException>(() => callback.Value);
    }

    [Fact]
    public async Task Unexpected_module_release_failure_still_releases_callback() {
        using var context = new BunitContext();
        var module = new ControlledModule { DisposeFailure = new JSException("Module release failed") };
        context.Services.AddSingleton<IJSRuntime>(new Runtime(module));
        var dialog = context.Render<Dialog>(parameters => parameters.Add(value => value.IsOpen, true));
        var callback = module.Callback!;

        await Assert.ThrowsAsync<JSException>(() => dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync()));

        Assert.Equal(1, module.Disposals);
        Assert.Throws<ObjectDisposedException>(() => callback.Value);
    }

    [Fact]
    public async Task Repeated_retirement_shares_pending_close_and_releases_once() {
        using var context = new BunitContext();
        var module = new ControlledModule { HoldClose = true };
        context.Services.AddSingleton<IJSRuntime>(new Runtime(module));
        var dialog = context.Render<Dialog>(parameters => parameters.Add(value => value.IsOpen, true));
        var first = dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync());
        await module.CloseEntered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        var second = dialog.InvokeAsync(async () => await dialog.Instance.DisposeAsync());
        module.ReleaseClose.TrySetResult();
        await Task.WhenAll(first, second).WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(1, module.Closes);
        Assert.Equal(1, module.Disposals);
        Assert.Throws<ObjectDisposedException>(() => module.Callback!.Value);
    }

    private sealed class Runtime(ControlledModule module) : IJSRuntime {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) {
            Assert.Equal("import", identifier);
            return ValueTask.FromResult((TValue)(object)module);
        }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }

    private sealed class ControlledModule : IJSObjectReference {
        public Exception? CloseFailure { get; set; }
        public Exception? DisposeFailure { get; init; }
        public bool HoldClose { get; init; }
        public int Closes { get; private set; }
        public int Disposals { get; private set; }
        public DotNetObjectReference<Dialog>? Callback { get; private set; }
        public TaskCompletionSource CloseEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ReleaseClose { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) {
            if (identifier == "openDialog") {
                Callback = (DotNetObjectReference<Dialog>)args![2]!;
            } else if (identifier == "closeDialog") {
                Closes++;
                CloseEntered.TrySetResult();
                if (HoldClose) {
                    await ReleaseClose.Task;
                }
                if (CloseFailure is { } error) {
                    throw error;
                }
            } else {
                throw new InvalidOperationException("Unexpected dialog method.");
            }
            return default!;
        }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
        public ValueTask DisposeAsync() {
            Disposals++;
            return DisposeFailure is { } error ? ValueTask.FromException(error) : ValueTask.CompletedTask;
        }
    }
}
