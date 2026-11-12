using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class NotificationModuleLoadRaceTests {
    [Fact]
    public async Task Retired_root_never_connects_a_late_import_and_releases_it_once() {
        using var context = new BunitContext();
        using var notifications = new NotificationService();
        var runtime = new PendingRuntime();
        context.Services.AddSingleton(notifications);
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var host = context.Render<OutletHost>();
        Assert.NotNull(notifications.ActiveOutlet);
        host.Render(parameters => parameters.Add(value => value.Visible, false));
        Assert.Null(notifications.ActiveOutlet);
        var module = new Module();
        await host.InvokeAsync(() => runtime.Import.SetResult(module));
        await host.Instance.Outlet!.Settled.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(1, module.Disposals);
        Assert.Null(host.Instance.Outlet.Failure);
        Assert.Equal(0, module.Connects);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Retired_root_accepts_expected_import_cancellation_without_a_late_callback(bool disconnected) {
        using var context = new BunitContext();
        using var notifications = new NotificationService();
        var runtime = new PendingRuntime();
        context.Services.AddSingleton(notifications);
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var host = context.Render<OutletHost>();
        host.Render(parameters => parameters.Add(value => value.Visible, false));
        await host.InvokeAsync(() => runtime.Import.SetException(disconnected
            ? new JSDisconnectedException("Retired circuit") : new TaskCanceledException("Retired import")));
        await host.Instance.Outlet!.Settled.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Null(host.Instance.Outlet.Failure);
        Assert.Null(notifications.ActiveOutlet);
    }

    public sealed class OutletHost : ComponentBase {
        [Parameter]
        public bool Visible { get; set; } = true;

        public TrackingNotification? Outlet { get; private set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder) {
            if (Visible) {
                builder.OpenComponent<TrackingNotification>(0);
                builder.AddComponentReferenceCapture(1, value => Outlet = (TrackingNotification)value);
                builder.CloseComponent();
            }
        }
    }

    public sealed class TrackingNotification : Notification {
        public TaskCompletionSource Settled { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Exception? Failure { get; private set; }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            try {
                await base.OnAfterRenderAsync(firstRender);
            } catch (Exception exception) {
                Failure = exception;
                throw;
            } finally {
                if (firstRender) {
                    Settled.TrySetResult();
                }
            }
        }
    }

    private sealed class PendingRuntime : IJSRuntime {
        public TaskCompletionSource<IJSObjectReference> Import { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            Assert.Equal("import", identifier);
            return (TValue)(object)await Import.Task;
        }
    }

    private sealed class Module : IJSObjectReference {
        public int Disposals { get; private set; }
        public int Connects { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            if (identifier == "connect") {
                Connects++;
            }
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask DisposeAsync() {
            Disposals++;
            return ValueTask.CompletedTask;
        }
    }
}
