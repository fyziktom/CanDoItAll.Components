using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TooltipLifecycleBehaviorTests {
    private static readonly Type InteropType = typeof(Tooltip).Assembly.GetType(
        "CanDoItAll.Components.BaseLib.TooltipInterop",
        throwOnError: true)!;

    [Fact]
    public async Task ConcurrentCallsImportOneSharedModule() {
        var module = new RecordingModule();
        var runtime = new RecordingRuntime(module, delayImport: false);
        var interop = CreateInterop(runtime);

        await Task.WhenAll(
            InvokeAnchorAsync(interop, "tooltip-a"),
            InvokeAnchorAsync(interop, "tooltip-b"));
        await DisposeAsync(interop);
        await DisposeAsync(interop);

        Assert.Equal(1, runtime.ImportCount);
        Assert.Equal(2, module.InvocationCount);
        Assert.Equal(1, module.DisposeCount);
    }

    [Fact]
    public async Task DisposeDuringImportDoesNotPublishOrLeakModule() {
        var module = new RecordingModule();
        var runtime = new RecordingRuntime(module, delayImport: true);
        var interop = CreateInterop(runtime);

        var anchorTask = InvokeAnchorAsync(interop, "tooltip-race");
        await runtime.ImportStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var disposeTask = DisposeAsync(interop);
        runtime.CompleteImport();

        var anchor = await anchorTask;
        await disposeTask;

        Assert.Null(anchor);
        Assert.Equal(1, runtime.ImportCount);
        Assert.Equal(0, module.InvocationCount);
        Assert.Equal(1, module.DisposeCount);
    }

    [Fact]
    public async Task CanceledModuleDisposalReleasesTheOwnedReferenceOnce() {
        var module = new RecordingModule { DisposalFailure = new TaskCanceledException() };
        var runtime = new RecordingRuntime(module, delayImport: false);
        var interop = CreateInterop(runtime);
        await InvokeAnchorAsync(interop, "retiring");

        await DisposeAsync(interop);
        await DisposeAsync(interop);
        Assert.Null(await InvokeAnchorAsync(interop, "retired"));
        Assert.Equal(1, module.DisposeCount);
        Assert.Equal(1, module.InvocationCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RetiredImportReleasesTheGateAfterSuccessOrCancellation(bool cancel) {
        var module = new RecordingModule { DisposalFailure = new TaskCanceledException() };
        var runtime = new RecordingRuntime(module, delayImport: true);
        var interop = CreateInterop(runtime);
        var anchor = InvokeAnchorAsync(interop, "retiring");
        await runtime.ImportStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var queued = InvokeVoidAsync(interop, "ClearFocusedTargetAsync", default(ElementReference), "retiring");
        var firstDisposal = DisposeAsync(interop);
        var secondDisposal = DisposeAsync(interop);

        runtime.CompleteImport(cancel ? new TaskCanceledException() : null);
        await Task.WhenAll(anchor, queued, firstDisposal, secondDisposal).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Null(await anchor);
        Assert.Equal(cancel ? 0 : 1, module.DisposeCount);
        Assert.Equal(0, module.InvocationCount);
        Assert.Equal(1, runtime.ImportCount);
    }

    [Fact]
    public async Task FailedRetiredImportStaysObservableAndReleasesTheGate() {
        var runtime = new RecordingRuntime(new RecordingModule(), delayImport: true);
        var interop = CreateInterop(runtime);
        var anchor = InvokeAnchorAsync(interop, "retiring");
        await runtime.ImportStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var disposal = DisposeAsync(interop);
        var failure = new JSException("Synthetic import defect");
        runtime.CompleteImport(failure);

        Assert.Same(failure, await Assert.ThrowsAsync<JSException>(() => anchor));
        await disposal.WaitAsync(TimeSpan.FromSeconds(5));
        await DisposeAsync(interop);
    }

    [Theory]
    [InlineData("GetAnchorPointAsync", false)]
    [InlineData("GetAnchorPointAsync", true)]
    [InlineData("ClearFocusedTargetAsync", false)]
    [InlineData("ClearFocusedTargetAsync", true)]
    [InlineData("ClampToViewportAsync", false)]
    [InlineData("ClampToViewportAsync", true)]
    public async Task RetiringWithQueuedCallsDrainsOnlyTheAcceptedOperation(string method, bool cancel) {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var module = new RecordingModule { InvocationRelease = release.Task };
        var interop = CreateInterop(new RecordingRuntime(module, delayImport: false));
        var active = InvokeOperationAsync(interop, method);
        await module.InvocationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var queuedAnchor = InvokeAnchorAsync(interop, "queued");
        var queuedClear = InvokeVoidAsync(interop, "ClearFocusedTargetAsync", default(ElementReference), "queued");
        var queuedClamp = InvokeVoidAsync(interop, "ClampToViewportAsync", default(ElementReference));
        var disposal = DisposeAsync(interop);
        if (cancel) {
            release.SetException(new TaskCanceledException());
        } else {
            release.SetResult();
        }

        await Task.WhenAll(active, queuedAnchor, queuedClear, queuedClamp, disposal).WaitAsync(TimeSpan.FromSeconds(5));
        await DisposeAsync(interop);
        Assert.Null(await queuedAnchor);
        Assert.Null(await InvokeAnchorAsync(interop, "after-disposal"));
        Assert.Equal(1, module.InvocationCount);
        Assert.Equal(1, module.DisposeCount);
    }

    [Theory]
    [InlineData("GetAnchorPointAsync")]
    [InlineData("ClearFocusedTargetAsync")]
    [InlineData("ClampToViewportAsync")]
    public async Task ActiveCancellationAndJavaScriptFailuresAreObservable(string method) {
        var module = new RecordingModule { InvocationRelease = Task.FromException(new TaskCanceledException()) };
        var interop = CreateInterop(new RecordingRuntime(module, delayImport: false));

        await Assert.ThrowsAsync<TaskCanceledException>(() => InvokeOperationAsync(interop, method));
        module.InvocationRelease = Task.FromException(new JSException("Synthetic active defect"));
        await Assert.ThrowsAsync<JSException>(() => InvokeOperationAsync(interop, method));
        module.InvocationRelease = Task.FromException(new InvalidOperationException("Synthetic programming defect"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => InvokeOperationAsync(interop, method));
        await DisposeAsync(interop);
        Assert.Equal(3, module.InvocationCount);
        Assert.Equal(1, module.DisposeCount);
    }

    [Fact]
    public async Task UnrelatedDisposalFailureRemainsObservableWithoutReusingTheModule() {
        var failure = new JSException("Synthetic cleanup defect");
        var module = new RecordingModule { DisposalFailure = failure };
        var interop = CreateInterop(new RecordingRuntime(module, delayImport: false));
        await InvokeAnchorAsync(interop, "active");

        Assert.Same(failure, await Assert.ThrowsAsync<JSException>(() => DisposeAsync(interop)));
        await DisposeAsync(interop);
        Assert.Null(await InvokeAnchorAsync(interop, "retired"));
        Assert.Equal(1, module.DisposeCount);
    }

    [Fact]
    public async Task LateFocusCleanupAndDisposalOfOneTargetPreserveTheOtherTarget() {
        await using var context = new BunitContext();
        var module = new RecordingModule { DisposalFailure = new TaskCanceledException() };
        context.Services.AddSingleton<IJSRuntime>(new RecordingRuntime(module, delayImport: false));
        context.Services.AddSingleton<TooltipService>();
        var first = context.Render<TooltipTarget>(parameters => parameters
            .Add(component => component.Text, "First")
            .Add(component => component.Duration, null));
        var second = context.Render<TooltipTarget>(parameters => parameters
            .Add(component => component.Text, "Second")
            .Add(component => component.Duration, null));
        var service = context.Services.GetRequiredService<TooltipService>();
        await first.InvokeAsync(() => first.Find("span").FocusInAsync(new FocusEventArgs()));
        Assert.Equal("First", service.Current?.Text);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        module.InvocationRelease = release.Task;
        var blur = first.InvokeAsync(() => first.Find("span").FocusOutAsync(new FocusEventArgs()));
        await second.InvokeAsync(() => second.Find("span").MouseEnterAsync(new MouseEventArgs()));
        Assert.Equal("Second", service.Current?.Text);

        release.SetResult();
        await blur;
        Assert.Equal("Second", service.Current?.Text);
        await first.InvokeAsync(async () => await first.Instance.DisposeAsync());
        Assert.Equal("Second", service.Current?.Text);
        await second.InvokeAsync(() => second.Find("span").MouseLeaveAsync(new MouseEventArgs()));
        Assert.Null(service.Current);
        Assert.Equal(1, module.DisposeCount);
    }

    private static Task InvokeOperationAsync(object interop, string method)
        => method == "GetAnchorPointAsync"
            ? InvokeAnchorAsync(interop, "active")
            : method == "ClearFocusedTargetAsync"
                ? InvokeVoidAsync(interop, method, default(ElementReference), "active")
                : InvokeVoidAsync(interop, method, default(ElementReference));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LeavingOrDisposingADelayedTargetCancelsItsPendingPresentation(bool dispose) {
        await using var context = new BunitContext();
        context.Services.AddSingleton<TooltipService>();
        var target = context.Render<TooltipTarget>(parameters => parameters
            .Add(component => component.Text, "Pending")
            .Add(component => component.Delay, TimeSpan.FromDays(1)));
        await target.InvokeAsync(() => target.Find("span").MouseEnterAsync(new MouseEventArgs()));
        var service = context.Services.GetRequiredService<TooltipService>();
        var lifetime = (CancellationTokenSource)typeof(TooltipService)
            .GetField("activeLifetime", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(service)!;
        var cancellation = lifetime.Token;
        Assert.Null(service.Current);

        if (dispose) {
            await target.InvokeAsync(async () => await target.Instance.DisposeAsync());
        } else {
            await target.InvokeAsync(() => target.Find("span").MouseLeaveAsync(new MouseEventArgs()));
        }

        Assert.True(cancellation.IsCancellationRequested);
        Assert.Null(service.Current);
    }

    [Fact]
    public async Task RetiringATargetWithAPendingAnchorCannotReplaceItsSuccessor() {
        await using var context = new BunitContext();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var module = new RecordingModule { InvocationRelease = release.Task };
        context.Services.AddSingleton<IJSRuntime>(new RecordingRuntime(module, delayImport: false));
        context.Services.AddSingleton<TooltipService>();
        var first = context.Render<TooltipTarget>(parameters => parameters.Add(component => component.Text, "First"));
        var second = context.Render<TooltipTarget>(parameters => parameters.Add(component => component.Text, "Second"));
        var focus = first.InvokeAsync(() => first.Find("span").FocusInAsync(new FocusEventArgs()));
        await module.InvocationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var disposal = first.InvokeAsync(async () => await first.Instance.DisposeAsync());
        await second.InvokeAsync(() => second.Find("span").MouseEnterAsync(new MouseEventArgs()));
        release.SetResult();
        await Task.WhenAll(focus, disposal).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal("Second", context.Services.GetRequiredService<TooltipService>().Current?.Text);
        Assert.Equal(1, module.DisposeCount);
        Assert.Equal(1, module.InvocationCount);
    }

    [Fact]
    public async Task TooltipHostDisposalReleasesTheModuleAndSubscriptionAfterCancellation() {
        await using var context = new BunitContext();
        var module = new RecordingModule { DisposalFailure = new TaskCanceledException() };
        context.Services.AddSingleton<IJSRuntime>(new RecordingRuntime(module, delayImport: false));
        context.Services.AddSingleton<TooltipService>();
        var host = context.Render<Tooltip>();
        var service = context.Services.GetRequiredService<TooltipService>();
        await host.InvokeAsync(() => service.Open("Before disposal", 10, 20));
        await module.InvocationStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await host.InvokeAsync(async () => await host.Instance.DisposeAsync());
        await host.InvokeAsync(() => service.Open("After disposal", 30, 40));

        Assert.Equal(1, module.InvocationCount);
        Assert.Equal(1, module.DisposeCount);
        service.Dispose();
        Assert.Null(service.Current);
    }

    private static Task InvokeVoidAsync(object interop, string method, params object?[] arguments)
        => ((ValueTask)InteropType.GetMethod(method)!.Invoke(interop, arguments)!).AsTask();

    private static object CreateInterop(IJSRuntime runtime)
        => Activator.CreateInstance(
            InteropType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: [runtime],
            culture: null)!;

    private static async Task<object?> InvokeAnchorAsync(object interop, string tooltipId) {
        var result = InteropType
            .GetMethod("GetAnchorPointAsync", BindingFlags.Instance | BindingFlags.Public)!
            .Invoke(interop, [default(ElementReference), tooltipId])!;
        var task = (Task)result.GetType().GetMethod("AsTask")!.Invoke(result, null)!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task);
    }

    private static Task DisposeAsync(object interop) {
        var result = (ValueTask)InteropType
            .GetMethod(nameof(IAsyncDisposable.DisposeAsync), BindingFlags.Instance | BindingFlags.Public)!
            .Invoke(interop, null)!;
        return result.AsTask();
    }

    private sealed class RecordingRuntime(RecordingModule module, bool delayImport) : IJSRuntime {
        private readonly TaskCompletionSource importRelease = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource ImportStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int ImportCount { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
            => new(ImportAsync<TValue>(identifier, cancellationToken));

        public void CompleteImport(Exception? failure = null) {
            if (failure is null) {
                importRelease.TrySetResult();
            } else {
                importRelease.TrySetException(failure);
            }
        }

        private async Task<TValue> ImportAsync<TValue>(string identifier, CancellationToken cancellationToken) {
            Assert.Equal("import", identifier);
            ImportCount++;
            ImportStarted.TrySetResult();
            if (delayImport) {
                await importRelease.Task.WaitAsync(cancellationToken);
            }

            return (TValue)(object)module;
        }
    }

    private sealed class RecordingModule : IJSObjectReference {
        public TaskCompletionSource InvocationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task? InvocationRelease { get; set; }

        public Exception? DisposalFailure { get; init; }

        public int InvocationCount { get; private set; }

        public int DisposeCount { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args) {
            InvocationCount++;
            InvocationStarted.TrySetResult();
            if (InvocationRelease is { } release) {
                await release;
            }
            object? value = identifier == "getAnchorPoint"
                ? Activator.CreateInstance(
                    typeof(TValue),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    args: [12d, 24d],
                    culture: null)
                : default(TValue);
            return (TValue)value!;
        }

        public ValueTask DisposeAsync() {
            DisposeCount++;
            return DisposalFailure is { } failure ? ValueTask.FromException(failure) : ValueTask.CompletedTask;
        }
    }
}
