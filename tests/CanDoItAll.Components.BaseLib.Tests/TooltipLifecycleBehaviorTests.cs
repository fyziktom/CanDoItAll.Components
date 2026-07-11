using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class TooltipLifecycleBehaviorTests
{
    private static readonly Type InteropType = typeof(Tooltip).Assembly.GetType(
        "CanDoItAll.Components.BaseLib.TooltipInterop",
        throwOnError: true)!;

    [Fact]
    public async Task ConcurrentCallsImportOneSharedModule()
    {
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
    public async Task DisposeDuringImportDoesNotPublishOrLeakModule()
    {
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

    private static object CreateInterop(IJSRuntime runtime)
        => Activator.CreateInstance(
            InteropType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: [runtime],
            culture: null)!;

    private static async Task<object?> InvokeAnchorAsync(object interop, string tooltipId)
    {
        var result = InteropType
            .GetMethod("GetAnchorPointAsync", BindingFlags.Instance | BindingFlags.Public)!
            .Invoke(interop, [default(ElementReference), tooltipId])!;
        var task = (Task)result.GetType().GetMethod("AsTask")!.Invoke(result, null)!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task);
    }

    private static Task DisposeAsync(object interop)
    {
        var result = (ValueTask)InteropType
            .GetMethod(nameof(IAsyncDisposable.DisposeAsync), BindingFlags.Instance | BindingFlags.Public)!
            .Invoke(interop, null)!;
        return result.AsTask();
    }

    private sealed class RecordingRuntime(RecordingModule module, bool delayImport) : IJSRuntime
    {
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

        public void CompleteImport() => importRelease.TrySetResult();

        private async Task<TValue> ImportAsync<TValue>(string identifier, CancellationToken cancellationToken)
        {
            Assert.Equal("import", identifier);
            ImportCount++;
            ImportStarted.TrySetResult();
            if (delayImport)
            {
                await importRelease.Task.WaitAsync(cancellationToken);
            }

            return (TValue)(object)module;
        }
    }

    private sealed class RecordingModule : IJSObjectReference
    {
        public int InvocationCount { get; private set; }

        public int DisposeCount { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            InvocationCount++;
            object? value = identifier == "getAnchorPoint"
                ? Activator.CreateInstance(
                    typeof(TValue),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    args: [12d, 24d],
                    culture: null)
                : default(TValue);
            return ValueTask.FromResult((TValue)value!);
        }

        public ValueTask DisposeAsync()
        {
            DisposeCount++;
            return ValueTask.CompletedTask;
        }
    }
}
