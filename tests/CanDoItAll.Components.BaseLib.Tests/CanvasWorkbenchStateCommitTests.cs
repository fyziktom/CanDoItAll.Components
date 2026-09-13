using Bunit;
using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchStateCommitTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Normalized_parent_state_reaches_the_browser(bool fromToolbar) {
        await using var harness = new Harness();
        harness.Render(json => {
            harness.Accept(json);
            return Task.CompletedTask;
        });

        if (fromToolbar) {
            harness.Context.JSInterop.Setup<string>("CanDoItAll.canvasWorkbench.getState", _ => true)
                .SetResult(harness.ClientState(1.25));
            await harness.Canvas.Find("[aria-label='Toggle maximize']").ClickAsync(new MouseEventArgs());
        } else {
            await harness.DispatchAsync(1.25, 1);
        }

        var applied = Assert.Single(harness.Updates);
        Assert.Equal(1.25, applied.Zoom);
        Assert.Equal(fromToolbar, applied.IsMaximized);
        Assert.Contains("adopted", Assert.Single(applied.GroupFrames).AnchorNodeIds);
    }

    [Fact]
    public async Task Exact_parent_echo_avoids_an_update_and_allows_later_owner_changes() {
        await using var harness = new Harness();
        harness.Render(json => {
            harness.Accept(json, normalize: false);
            return Task.CompletedTask;
        });

        await harness.DispatchAsync(1.25, 1);

        Assert.Empty(harness.Updates);
        harness.Accept(harness.ClientState(1.5));
        Assert.Equal(1.5, Assert.Single(harness.Updates).Zoom);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Only_the_latest_pending_callback_can_release_owner_synchronization(bool latestCompletesFirst) {
        await using var harness = new Harness();
        var entered = new[] { Signal(), Signal() };
        var release = new[] { Signal(), Signal() };
        var calls = 0;
        harness.Render(async json => {
            var index = calls++;
            harness.Accept(json);
            entered[index].SetResult();
            await release[index].Task;
        });

        var firstState = harness.ClientState(1.25);
        var secondState = harness.ClientState(1.5);
        var first = harness.Canvas.InvokeAsync(() => harness.Canvas.Instance.OnStateChanged(firstState, 1));
        Task second = Task.CompletedTask;
        try {
            await entered[0].Task.WaitAsync(TimeSpan.FromSeconds(5));
            second = harness.Canvas.InvokeAsync(() => harness.Canvas.Instance.OnStateChanged(secondState, 2));
            await entered[1].Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Empty(harness.Updates);

            var firstToFinish = latestCompletesFirst ? 1 : 0;
            release[firstToFinish].SetResult();
            await (latestCompletesFirst ? second : first);
            if (latestCompletesFirst) {
                Assert.Equal(1.5, Assert.Single(harness.Updates).Zoom);
            } else {
                Assert.Empty(harness.Updates);
            }
        } finally {
            foreach (var signal in release) {
                signal.TrySetResult();
            }
            await Task.WhenAll(first, second);
        }

        Assert.Equal(2, calls);
        Assert.Equal(1.5, Assert.Single(harness.Updates).Zoom);
    }

    [Fact]
    public async Task Callback_failure_preserves_the_exception_and_allows_a_later_owner_update() {
        await using var harness = new Harness();
        var failure = new InvalidOperationException("Owner rejected the canvas update.");
        harness.Render(_ => Task.FromException(failure));

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => harness.DispatchAsync(1.25, 1));

        Assert.Same(failure, actual);
        Assert.Empty(harness.Updates);
        harness.Accept(harness.ClientState(1.5));
        Assert.Equal(1.5, Assert.Single(harness.Updates).Zoom);
    }

    [Fact]
    public async Task Removing_the_handler_does_not_abandon_an_admitted_commit() {
        await using var harness = new Harness();
        var entered = Signal();
        var release = Signal();
        harness.Render(async json => {
            harness.Accept(json);
            entered.SetResult();
            await release.Task;
        });

        var pending = harness.DispatchAsync(1.25, 1);
        try {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            harness.Canvas.Render(parameters => parameters
                .Add(component => component.StateChanged, default(EventCallback<string>)));
            Assert.Empty(harness.Updates);
        } finally {
            release.TrySetResult();
            await pending;
        }

        Assert.Equal(1.25, Assert.Single(harness.Updates).Zoom);
    }

    [Fact]
    public async Task Unbound_client_changes_do_not_replay_an_uncommitted_parent_state() {
        await using var harness = new Harness();
        harness.Render();

        await harness.DispatchAsync(1.25, 1);

        Assert.Empty(harness.Updates);
    }

    [Fact]
    public async Task Stale_dispatch_is_rejected_without_another_callback_or_browser_update() {
        await using var harness = new Harness();
        var calls = 0;
        harness.Render(json => {
            calls++;
            harness.Accept(json);
            return Task.CompletedTask;
        });
        await harness.DispatchAsync(1.25, 2);
        Assert.Single(harness.Updates);
        harness.Updates.Clear();

        await harness.DispatchAsync(1.5, 1);

        Assert.Equal(1, calls);
        Assert.Equal(1.25, harness.Surface.UiState.Zoom);
        Assert.Empty(harness.Updates);
    }

    private static TaskCompletionSource Signal()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class Harness : IAsyncDisposable {
        public BunitContext Context { get; } = new();
        public List<CanvasWorkbenchUiState> Updates { get; } = [];
        public IRenderedComponent<CanvasWorkbench> Canvas { get; private set; } = null!;
        public CanvasWorkbenchSurface Surface { get; } = new() {
            SurfaceId = "owner-state",
            Nodes = [new() { Id = "anchor" }, new() { Id = "adopted" }],
            UiState = new() { GroupFrames = [new() { Id = "frame", AnchorNodeIds = ["anchor"] }] }
        };

        public Harness() {
            Context.JSInterop.Mode = JSRuntimeMode.Loose;
            Context.JSInterop.Setup<bool>("CanDoItAll.canvasWorkbench.create", _ => true).SetResult(true);
            Context.JSInterop.Setup<bool>("CanDoItAll.canvasWorkbench.update", invocation => {
                var surface = Assert.IsType<CanvasWorkbenchSurface>(invocation.Arguments[1]);
                Updates.Add(CanvasWorkbenchUiState.Parse(surface.UiState.ToJson()));
                return true;
            }).SetResult(true);
        }

        public void Render(Func<string, Task>? callback = null) {
            Canvas = Context.Render<CanvasWorkbench>(parameters => {
                parameters.Add(component => component.Surface, Surface);
                if (callback is not null) {
                    parameters.Add(component => component.StateChanged, callback);
                }
            });
            Assert.Single(Context.JSInterop.Invocations["CanDoItAll.canvasWorkbench.create"]);
        }

        public string ClientState(double zoom) {
            var state = CanvasWorkbenchUiState.Parse(Surface.UiState.ToJson());
            state.Zoom = zoom;
            return state.ToJson();
        }

        public Task DispatchAsync(double zoom, long dispatchId)
            => Canvas.InvokeAsync(() => Canvas.Instance.OnStateChanged(ClientState(zoom), dispatchId));

        public void Accept(string json, bool normalize = true) {
            Surface.UiState = CanvasWorkbenchUiState.Parse(json);
            if (normalize) {
                Surface.UiState.GroupFrames = [new() { Id = "frame", AnchorNodeIds = ["anchor", "adopted"] }];
            }
            Canvas.Render(parameters => parameters.Add(component => component.Surface, Surface));
        }

        public ValueTask DisposeAsync()
            => Context.DisposeAsync();
    }
}
