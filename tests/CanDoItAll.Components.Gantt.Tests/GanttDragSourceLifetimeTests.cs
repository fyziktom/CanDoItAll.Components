using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttDragSourceLifetimeTests {
    [Fact]
    public async Task Disposal_waits_for_pending_registration_and_releases_it_once() {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { HoldRegistration = true };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        await runtime.RegistrationEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));

        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        Assert.False(disposal.IsCompleted);
        runtime.RegistrationRelease.SetResult();
        await disposal.WaitAsync(TimeSpan.FromSeconds(3));

        AssertReleased(Assert.Single(runtime.Registrations));
        await cut.Instance.DisposeAsync();
        AssertReleased(Assert.Single(runtime.Registrations));
    }

    [Fact]
    public async Task Replacement_cleanup_and_disposal_share_one_registration_owner() {
        using var context = new TestContext();
        var runtime = new ControlledRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        var original = Assert.Single(runtime.Registrations);
        original.HoldCleanup = true;
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.Task, TaskModel("Replacement")));
        await original.CleanupEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));

        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        Assert.False(disposal.IsCompleted);
        Assert.Equal(1, original.CleanupCalls);
        original.CleanupRelease.SetResult();
        await disposal.WaitAsync(TimeSpan.FromSeconds(3));

        AssertReleased(Assert.Single(runtime.Registrations));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Changes_during_registration_apply_the_latest_parameters(bool disable) {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { HoldRegistration = true };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        await runtime.RegistrationEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        cut.SetParametersAndRender(parameters => parameters
            .Add(component => component.Task, TaskModel("Latest task"))
            .Add(component => component.Disabled, disable));
        runtime.RegistrationRelease.SetResult();
        var original = await runtime.FirstRegistered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        await original.ProxyReleased.Task.WaitAsync(TimeSpan.FromSeconds(3));
        if (!disable) {
            var latest = await runtime.SecondRegistered.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Contains("Latest task", latest.Payload, StringComparison.Ordinal);
            Assert.Equal(0, latest.CleanupCalls);
        }
        AssertReleased(original);
        Assert.Equal(disable ? 1 : 2, runtime.Registrations.Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Cleanup_releases_the_proxy_and_only_suppresses_a_disconnected_circuit(bool disconnected) {
        using var context = new TestContext();
        var runtime = new ControlledRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        var registration = Assert.Single(runtime.Registrations);
        registration.CleanupFailure = disconnected
            ? new JSDisconnectedException("Circuit closed.")
            : new JSException("Synthetic cleanup failure.");

        var error = await Record.ExceptionAsync(() => cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask()));

        if (disconnected) {
            Assert.Null(error);
        } else {
            Assert.Same(registration.CleanupFailure, error);
        }
        AssertReleased(registration);
        await cut.Instance.DisposeAsync();
        AssertReleased(registration);
    }

    private static IRenderedComponent<GanttTaskDragSource> Render(TestContext context)
        => context.RenderComponent<GanttTaskDragSource>(parameters => parameters
            .Add(component => component.Task, TaskModel("Original task")));

    private static GanttTask TaskModel(string title)
        => new(new GanttTaskId("task"), title, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(1));

    private static void AssertReleased(Registration registration) {
        Assert.Equal(1, registration.CleanupCalls);
        Assert.Equal(1, registration.ProxyDisposals);
    }

    private sealed class ControlledRuntime : IJSRuntime {
        public bool HoldRegistration { get; set; }
        public List<Registration> Registrations { get; } = [];
        public TaskCompletionSource RegistrationEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource RegistrationRelease { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<Registration> FirstRegistered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<Registration> SecondRegistered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            Assert.Equal("CanDoItAll.ganttChart.registerDragSource", identifier);
            if (HoldRegistration) {
                HoldRegistration = false;
                RegistrationEntered.SetResult();
                await RegistrationRelease.Task.WaitAsync(cancellationToken);
            }
            var registration = new Registration((string)args![1]!);
            Registrations.Add(registration);
            (Registrations.Count == 1 ? FirstRegistered : SecondRegistered).TrySetResult(registration);
            return (TValue)(object)registration;
        }
    }

    private sealed class Registration(string payload) : IJSObjectReference {
        public string Payload { get; } = payload;
        public bool HoldCleanup { get; set; }
        public Exception? CleanupFailure { get; set; }
        public int CleanupCalls { get; private set; }
        public int ProxyDisposals { get; private set; }
        public TaskCompletionSource CleanupEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource CleanupRelease { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ProxyReleased { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            Assert.Equal("dispose", identifier);
            CleanupCalls++;
            CleanupEntered.TrySetResult();
            if (HoldCleanup) {
                await CleanupRelease.Task.WaitAsync(cancellationToken);
            }
            if (CleanupFailure is not null) {
                throw CleanupFailure;
            }
            return default!;
        }

        public ValueTask DisposeAsync() {
            ProxyDisposals++;
            ProxyReleased.TrySetResult();
            return ValueTask.CompletedTask;
        }
    }
}
