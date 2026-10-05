using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttCleanupTests {
    public enum Completion { Success, TaskCanceled, OperationCanceled, Fault, JavaScriptFault }

    [Theory]
    [InlineData(Completion.TaskCanceled, false)]
    [InlineData(Completion.OperationCanceled, false)]
    [InlineData(Completion.Fault, false)]
    [InlineData(Completion.TaskCanceled, true)]
    [InlineData(Completion.OperationCanceled, true)]
    [InlineData(Completion.Fault, true)]
    [InlineData(Completion.Success, false)]
    [InlineData(Completion.JavaScriptFault, false)]
    public async Task Created_canvas_is_released_even_when_update_fails(Completion completion, bool alreadyCompleted) {
        using var context = new TestContext();
        var runtime = new ControlledRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        Assert.Single(runtime.Resources);
        runtime.HoldNext = true;
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(2)));
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        if (alreadyCompleted) {
            runtime.Release.SetResult(completion);
            await ObservePending(cut.Instance);
        }
        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        if (!alreadyCompleted) {
            Assert.False(disposal.IsCompleted);
            runtime.Release.SetResult(completion);
        }
        var error = await Record.ExceptionAsync(() => disposal.WaitAsync(TimeSpan.FromSeconds(3)));
        AssertCompletion(completion, error);
        Assert.Empty(runtime.Resources);
        Assert.Equal(1, runtime.DisposeCalls);
        Assert.Null(Callback(cut.Instance));
        await cut.Instance.DisposeAsync();
        Assert.Equal(1, runtime.DisposeCalls);
    }

    [Theory]
    [InlineData(Completion.TaskCanceled)]
    [InlineData(Completion.OperationCanceled)]
    [InlineData(Completion.Fault)]
    [InlineData(Completion.JavaScriptFault)]
    public async Task Failed_create_does_not_invent_a_resource_to_dispose(Completion completion) {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { HoldNext = true };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        runtime.Release.SetResult(completion);
        AssertCompletion(completion, await Record.ExceptionAsync(() => disposal.WaitAsync(TimeSpan.FromSeconds(3))));
        Assert.Empty(runtime.Resources);
        Assert.Equal(0, runtime.DisposeCalls);
        Assert.Null(Callback(cut.Instance));
    }

    [Fact]
    public async Task Both_failures_remain_observable_and_the_callback_is_released() {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { CleanupFailure = new JSException("cleanup failed") };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = Render(context);
        runtime.HoldNext = true;
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(2)));
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        runtime.Release.SetResult(Completion.Fault);
        var error = await Assert.ThrowsAsync<AggregateException>(() => disposal.WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.Collection(error.InnerExceptions,
            pending => Assert.Same(runtime.UpdateFailure, pending),
            cleanup => Assert.Same(runtime.CleanupFailure, cleanup));
        Assert.Equal(1, runtime.DisposeCalls);
        Assert.Null(Callback(cut.Instance));
    }

    [Fact]
    public async Task Disposing_one_chart_leaves_the_other_chart_and_its_callback_alive() {
        using var context = new TestContext();
        var runtime = new ControlledRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var first = Render(context);
        var second = Render(context);
        Assert.Equal(2, runtime.Resources.Count);
        runtime.HoldNext = true;
        first.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(2)));
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        var disposal = first.InvokeAsync(() => first.Instance.DisposeAsync().AsTask());
        runtime.Release.SetResult(Completion.OperationCanceled);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => disposal);
        Assert.Single(runtime.Resources);
        Assert.Null(Callback(first.Instance));
        Assert.NotNull(Callback(second.Instance));
        second.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(3)));
        Assert.Equal(3, runtime.LastTaskCount);
        await second.Instance.DisposeAsync();
        Assert.Empty(runtime.Resources);
    }

    private static IRenderedComponent<GanttChart> Render(TestContext context) => context.RenderComponent<GanttChart>(parameters => parameters
        .Add(component => component.Tasks, Tasks(1)));

    private static GanttTask[] Tasks(int count) => Enumerable.Range(1, count).Select(index => new GanttTask(new($"task-{index}"), $"Task {index}", DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(index))).ToArray();

    private static object? Callback(GanttChart chart) => typeof(GanttChart).GetField("dotNetReference", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(chart);

    private static async Task ObservePending(GanttChart chart) {
        var task = (Task)typeof(GanttChart).GetField("pendingInterop", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(chart)!;
        await Record.ExceptionAsync(() => task.WaitAsync(TimeSpan.FromSeconds(3)));
    }

    private static void AssertCompletion(Completion completion, Exception? error) {
        switch (completion) {
            case Completion.TaskCanceled:
                Assert.IsAssignableFrom<TaskCanceledException>(error);
                break;
            case Completion.OperationCanceled:
                Assert.IsAssignableFrom<OperationCanceledException>(error);
                break;
            case Completion.Fault:
                Assert.IsType<InvalidOperationException>(error);
                break;
            default:
                Assert.Null(error);
                break;
        }
    }

    private sealed class ControlledRuntime : IJSRuntime {
        public bool HoldNext { get; set; }
        public HashSet<string> Resources { get; } = [];
        private readonly Dictionary<string, string> owners = [];
        public int DisposeCalls { get; private set; }
        public int LastTaskCount { get; private set; }
        public Exception UpdateFailure { get; } = new InvalidOperationException("update failed");
        public Exception? CleanupFailure { get; init; }
        public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<Completion> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            if (identifier is not ("CanDoItAll.ganttChart.create" or "CanDoItAll.ganttChart.update" or "CanDoItAll.ganttChart.dispose")) {
                return default!;
            }
            var host = ((ElementReference)args![0]!).Id;
            if (identifier == "CanDoItAll.ganttChart.dispose") {
                DisposeCalls++;
                Assert.Equal(host, owners[(string)args[1]!]);
                if (CleanupFailure is not null) {
                    throw CleanupFailure;
                }
                Assert.True(Resources.Remove(host));
                owners.Remove((string)args[1]!);
                return default!;
            }
            if (HoldNext) {
                HoldNext = false;
                Entered.SetResult();
                switch (await Release.Task.WaitAsync(cancellationToken)) {
                    case Completion.TaskCanceled:
                        throw new TaskCanceledException("update canceled");
                    case Completion.OperationCanceled:
                        throw new OperationCanceledException("update canceled");
                    case Completion.Fault:
                        throw UpdateFailure;
                    case Completion.JavaScriptFault:
                        throw new JSException("interop failed");
                }
            }
            if (identifier == "CanDoItAll.ganttChart.create") {
                Assert.True(Resources.Add(host));
                owners.Add(System.Text.Json.JsonSerializer.SerializeToElement(args[^1]).GetProperty("OwnerId").GetString()!, host);
            }
            Assert.Contains(host, Resources);
            LastTaskCount = System.Text.Json.JsonSerializer.SerializeToElement(args[^1]).GetProperty("Tasks").GetArrayLength();
            return default!;
        }
    }
}
