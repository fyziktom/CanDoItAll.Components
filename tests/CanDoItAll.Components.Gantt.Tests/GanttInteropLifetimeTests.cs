using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttInteropLifetimeTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Slow_interop_cannot_replace_a_newer_controlled_model(bool holdCreate) {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { HoldNext = holdCreate };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = context.RenderComponent<GanttChart>(parameters => parameters.Add(component => component.Tasks, Tasks(1)));
        if (!holdCreate) {
            runtime.HoldNext = true;
            cut.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(2)));
        }
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, Tasks(3)));
        runtime.Release.TrySetResult();
        await runtime.Finished.Task.WaitAsync(TimeSpan.FromSeconds(3));
        cut.WaitForAssertion(() => Assert.Equal(3, runtime.DisplayedTaskCount));
        Assert.Equal(1, runtime.CreateCount);
        Assert.Equal(3, cut.FindAll(".cda-gantt__table-row").Count - 1);
    }

    [Fact]
    public async Task Disposal_waits_for_original_create_and_releases_its_javascript_instance() {
        using var context = new TestContext();
        var runtime = new ControlledRuntime { HoldNext = true };
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var cut = context.RenderComponent<GanttChart>(parameters => parameters.Add(component => component.Tasks, Tasks(1)));
        await runtime.Entered.Task.WaitAsync(TimeSpan.FromSeconds(3));
        var disposal = cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());
        runtime.Release.TrySetResult();
        await disposal.WaitAsync(TimeSpan.FromSeconds(3));
        await runtime.Finished.Task.WaitAsync(TimeSpan.FromSeconds(3));
        Assert.Equal(1, runtime.DisposeCount);
        Assert.False(runtime.Exists);
    }

    private static GanttTask[] Tasks(int count) => Enumerable.Range(1, count)
        .Select(index => new GanttTask(new($"task-{index}"), $"Task {index}", DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddHours(index))).ToArray();

    private sealed class ControlledRuntime : IJSRuntime {
        public bool HoldNext { get; set; }
        public int DisplayedTaskCount { get; private set; }
        public int CreateCount { get; private set; }
        public int DisposeCount { get; private set; }
        public bool Exists { get; private set; }
        public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Finished { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
            if (identifier == "CanDoItAll.ganttChart.dispose") {
                DisposeCount++;
                Exists = false;
                return default!;
            }
            if (identifier is not ("CanDoItAll.ganttChart.create" or "CanDoItAll.ganttChart.update")) {
                return default!;
            }
            var held = HoldNext;
            HoldNext = false;
            if (held) {
                Entered.TrySetResult();
                await Release.Task.WaitAsync(cancellationToken);
            }
            if (identifier == "CanDoItAll.ganttChart.create") {
                CreateCount++;
                Exists = true;
            }
            DisplayedTaskCount = JsonSerializer.SerializeToElement(args![^1]).GetProperty("Tasks").GetArrayLength();
            if (held) {
                Finished.TrySetResult();
            }
            return default!;
        }
    }
}
