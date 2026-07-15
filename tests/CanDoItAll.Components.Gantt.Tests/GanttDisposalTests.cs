using System.Reflection;
using Bunit;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttDisposalTests
{
    [Fact]
    public async Task Dispose_releases_the_callback_reference_when_javascript_cleanup_fails()
    {
        using var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.JSInterop
            .SetupVoid("CanDoItAll.ganttChart.dispose", static _ => true)
            .SetException(new JSException("Synthetic disposal failure."));
        var task = new GanttTask(
            new GanttTaskId("task"),
            "Task",
            DateTimeOffset.UnixEpoch,
            DateTimeOffset.UnixEpoch.AddHours(1));
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { task })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>()));
        var callbackReference = typeof(GanttChart).GetField(
            "dotNetReference",
            BindingFlags.Instance | BindingFlags.NonPublic)!;

        Assert.NotNull(callbackReference.GetValue(cut.Instance));
        await Assert.ThrowsAsync<JSException>(() => cut.Instance.DisposeAsync().AsTask());
        Assert.Null(callbackReference.GetValue(cut.Instance));
    }
}
