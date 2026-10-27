using Bunit;
using CanDoItAll.Components.CanvasLib;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasCalendarSynchronizationTests {
    private const string Create = "CanDoItAll.canvasCalendar.create";
    private const string Update = "CanDoItAll.canvasCalendar.update";
    private const string Dispose = "CanDoItAll.canvasCalendar.dispose";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Pending_interop_is_serialized_and_applies_the_latest_surface(bool creating) {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var pending = context.JSInterop.SetupVoid(creating ? Create : Update, _ => true);
        var calendar = context.Render<CanvasCalendar>(parameters => parameters.Add(component => component.Surface, Surface("week")));
        try {
            calendar.Render(parameters => parameters.Add(component => component.Surface, Surface("month")));
            calendar.Render(parameters => parameters.Add(component => component.Surface, Surface("list")));

            Assert.Single(context.JSInterop.Invocations, call => call.Identifier == Create);
            Assert.Equal(creating ? 0 : 1, context.JSInterop.Invocations.Count(call => call.Identifier == Update));
            pending.SetVoidResult();

            calendar.WaitForAssertion(() => {
                var updates = context.JSInterop.Invocations.Where(call => call.Identifier == Update).ToArray();
                Assert.Equal(creating ? 1 : 2, updates.Length);
                Assert.Equal("list", Assert.IsType<CanvasCalendarSurface>(updates[^1].Arguments[1]).InitialView);
                Assert.Single(context.JSInterop.Invocations, call => call.Identifier == Create);
            });
        } finally {
            pending.SetVoidResult();
        }
    }

    [Fact]
    public async Task Disposal_waits_for_creation_and_releases_the_created_controller_once() {
        await using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var creation = context.JSInterop.SetupVoid(Create, _ => true);
        var calendar = context.Render<CanvasCalendar>(parameters => parameters.Add(component => component.Surface, Surface("week")));
        Task cleanup = Task.CompletedTask;
        try {
            await calendar.InvokeAsync(() => {
                cleanup = calendar.Instance.DisposeAsync().AsTask();
            });
            Assert.False(cleanup.IsCompleted);
            Assert.DoesNotContain(context.JSInterop.Invocations, call => call.Identifier == Dispose);

            creation.SetVoidResult();
            await cleanup.WaitAsync(TimeSpan.FromSeconds(5));
            await calendar.Instance.DisposeAsync();

            Assert.Single(context.JSInterop.Invocations, call => call.Identifier == Dispose);
            Assert.DoesNotContain(context.JSInterop.Invocations, call => call.Identifier == Update);
        } finally {
            creation.SetVoidResult();
        }
    }

    private static CanvasCalendarSurface Surface(string view) => new() { InitialView = view };
}
