using System.Collections.Concurrent;
using System.Reflection;
using CanDoItAll.Components.Mermaid;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class MermaidDiagramLifecycleTests
{
    [Fact]
    public async Task DisposeDuringImportDisposesTheLateModuleWithoutRendering()
    {
        var module = new RecordingModule(delayRender: false);
        var runtime = new RecordingRuntime(module, delayImport: true);
        var diagram = CreateDiagram(runtime);

        var renderTask = diagram.RunAfterRenderAsync();
        await runtime.ImportStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var disposeTask = diagram.DisposeAsync().AsTask();
        runtime.CompleteImport();

        await Task.WhenAll(renderTask, disposeTask).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(0, module.RenderCount);
        Assert.Equal(0, module.DestroyCount);
        Assert.Equal(1, module.DisposeCount);
    }

    [Fact]
    public async Task DisposeDuringRenderCleansUpWithoutPublishingCallbacks()
    {
        var module = new RecordingModule(delayRender: true);
        var runtime = new RecordingRuntime(module, delayImport: false);
        var diagram = CreateDiagram(runtime);
        var renderedCount = 0;
        var nodeClickedCount = 0;
        SetProperty(
            diagram,
            nameof(MermaidDiagram.Rendered),
            EventCallback.Factory.Create<MermaidRenderResult>(
                new object(),
                _ => renderedCount++));
        SetProperty(
            diagram,
            nameof(MermaidDiagram.NodeClicked),
            EventCallback.Factory.Create<MermaidNodeClickEventArgs>(
                new object(),
                _ => nodeClickedCount++));

        var renderTask = diagram.RunAfterRenderAsync();
        await module.RenderStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var disposeTask = diagram.DisposeAsync().AsTask();
        module.CompleteRender();

        await Task.WhenAll(renderTask, disposeTask).WaitAsync(TimeSpan.FromSeconds(5));
        await diagram.HandleNodeClickedAsync(new MermaidNodeClickEventArgs());

        Assert.Equal(1, module.RenderCount);
        Assert.Equal(1, module.DestroyCount);
        Assert.Equal(1, module.DisposeCount);
        Assert.Equal(0, renderedCount);
        Assert.Equal(0, nodeClickedCount);
        Assert.Equal(
            ["render-start", "render-end", "destroy", "module-dispose"],
            module.Operations.ToArray());
    }

    private static TestableMermaidDiagram CreateDiagram(IJSRuntime runtime)
    {
        var diagram = new TestableMermaidDiagram();
        SetProperty(diagram, "JS", runtime);
        SetProperty(diagram, nameof(MermaidDiagram.Source), "flowchart LR\nA --> B");
        return diagram;
    }

    private static void SetProperty(TestableMermaidDiagram diagram, string name, object value)
        => typeof(MermaidDiagram)
            .GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(diagram, value);

    private sealed class TestableMermaidDiagram : MermaidDiagram
    {
        public Task RunAfterRenderAsync()
            => base.OnAfterRenderAsync(firstRender: true);
    }

    private sealed class RecordingRuntime(RecordingModule module, bool delayImport) : IJSRuntime
    {
        private readonly TaskCompletionSource importRelease = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource ImportStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
            => new(ImportAsync<TValue>(identifier, cancellationToken));

        public void CompleteImport()
            => importRelease.TrySetResult();

        private async Task<TValue> ImportAsync<TValue>(string identifier, CancellationToken cancellationToken)
        {
            Assert.Equal("import", identifier);
            ImportStarted.TrySetResult();
            if (delayImport)
            {
                await importRelease.Task.WaitAsync(cancellationToken);
            }

            return (TValue)(object)module;
        }
    }

    private sealed class RecordingModule(bool delayRender) : IJSObjectReference
    {
        private readonly TaskCompletionSource renderRelease = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource RenderStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int RenderCount { get; private set; }

        public int DestroyCount { get; private set; }

        public int DisposeCount { get; private set; }

        public ConcurrentQueue<string> Operations { get; } = new();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
            => identifier switch
            {
                "render" => new(RenderAsync<TValue>(cancellationToken)),
                "destroy" => Destroy<TValue>(),
                _ => throw new InvalidOperationException($"Unexpected invocation '{identifier}'.")
            };

        public void CompleteRender()
            => renderRelease.TrySetResult();

        public ValueTask DisposeAsync()
        {
            DisposeCount++;
            Operations.Enqueue("module-dispose");
            return ValueTask.CompletedTask;
        }

        private async Task<TValue> RenderAsync<TValue>(CancellationToken cancellationToken)
        {
            RenderCount++;
            Operations.Enqueue("render-start");
            RenderStarted.TrySetResult();
            if (delayRender)
            {
                await renderRelease.Task.WaitAsync(cancellationToken);
            }

            Operations.Enqueue("render-end");
            return (TValue)(object)new MermaidRenderResult
            {
                Succeeded = true,
                DiagramId = "diagram"
            };
        }

        private ValueTask<TValue> Destroy<TValue>()
        {
            DestroyCount++;
            Operations.Enqueue("destroy");
            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
