using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// The histogram of a <see cref="WebGlFloatImageView"/>'s displayed values with its display window, on a scale that
/// stays put while the window changes (so a narrowed window can be widened again). The window's edges and the window
/// itself are dragged; the linked views follow at once in the browser and <see cref="WindowChanged"/> reports the
/// window when the drag ends. A focused edge or window steps with the arrow keys (Shift: ten steps), Home and End; Esc
/// cancels a drag; a double-click fits the scale to the values and the window. Display only: no value changes.
/// </summary>
public partial class WebGlImageWindowHistogram : IAsyncDisposable
{
    private ElementReference svg;
    private IJSObjectReference? module;
    private IJSObjectReference? instance;
    private DotNetObjectReference<WebGlImageWindowHistogram>? reference;
    private string? linkedKey;
    private WebGlImageWindow? appliedWindow;
    private int appliedScaleRevision;
    private bool scaleFitPending;
    private bool disposed;

    /// <summary>The id of the view whose displayed values are binned (see <see cref="WebGlFloatImageView.Id"/>).</summary>
    [Parameter, EditorRequired]
    public string ViewId { get; set; } = "";

    /// <summary>Ids of further views whose window follows this one while it moves (a comparison under one window).</summary>
    [Parameter]
    public IReadOnlyList<string>? LinkedViewIds { get; set; }

    /// <summary>The display window shown on the histogram.</summary>
    [Parameter]
    public WebGlImageWindow? Window { get; set; }

    /// <summary>The window after a drag or a key on the histogram.</summary>
    [Parameter]
    public EventCallback<WebGlImageWindow?> WindowChanged { get; set; }

    /// <summary>The window while it is dragged (at most once per frame).</summary>
    [Parameter]
    public EventCallback<WebGlImageWindow> WindowInput { get; set; }

    /// <summary>The values against the window, whenever either changes (at most once per frame).</summary>
    [Parameter]
    public EventCallback<WebGlImageWindowStats> StatsChanged { get; set; }

    /// <summary>A change fits the scale to the values and the window again (for example after a "fit window" command).</summary>
    [Parameter]
    public int ScaleRevision { get; set; }

    /// <summary>Bins over the values' own range (up to four times more when the scale is wider).</summary>
    [Parameter]
    public int Bins { get; set; } = 64;

    /// <summary>Axis label when the view's source gives none.</summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>What is counted on the vertical axis.</summary>
    [Parameter]
    public string CountLabel { get; set; } = "pixels";

    [Parameter]
    public string AriaLabel { get; set; } = "Histogram of the displayed values with the display window: drag an edge of the window or the window itself";

    [Parameter]
    public int ViewBoxWidth { get; set; } = 400;

    [Parameter]
    public int ViewBoxHeight { get; set; } = 80;

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => string.IsNullOrWhiteSpace(Class) ? "ui-webgl-histogram" : "ui-webgl-histogram " + Class;

    private string ViewBox => string.Create(CultureInfo.InvariantCulture, $"0 0 {ViewBoxWidth} {ViewBoxHeight}");

    /// <summary>The values against the window now (null before the histogram runs in the browser).</summary>
    public async ValueTask<WebGlImageWindowStats?> GetStatsAsync() =>
        instance is null || disposed ? null : await instance.InvokeAsync<WebGlImageWindowStats>("stats");

    protected override void OnParametersSet()
    {
        if (ScaleRevision != appliedScaleRevision)
        {
            appliedScaleRevision = ScaleRevision;
            scaleFitPending = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (disposed) return;
        if (firstRender)
        {
            module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", WebGlFloatImageView.ModulePath);
            reference = DotNetObjectReference.Create(this);
            instance = await module.InvokeAsync<IJSObjectReference>("createWindowHistogram", svg, reference, new
            {
                bins = Bins,
                label = Label,
                countLabel = CountLabel,
                emitInput = WindowInput.HasDelegate,
                emitStats = StatsChanged.HasDelegate,
            });
        }

        if (instance is null) return;
        string[] ids = [ViewId, .. LinkedViewIds ?? []];
        string key = string.Join('\n', ids);
        if (key != linkedKey)
        {
            linkedKey = key;
            await instance.InvokeVoidAsync("link", (object)ids);   // one argument, not params
        }

        if (Window != appliedWindow || scaleFitPending)
        {
            bool fit = scaleFitPending;
            scaleFitPending = false;
            appliedWindow = Window;
            await instance.InvokeVoidAsync("setWindow", Window?.Low, Window?.High, fit);
        }
    }

    /// <summary>Called by the browser module when a drag or a key ends on a new window.</summary>
    [JSInvokable]
    public Task NotifyWindowChanged(double low, double high)
    {
        var window = new WebGlImageWindow(low, high);
        appliedWindow = window;   // the browser shows it already
        return WindowChanged.InvokeAsync(window);
    }

    /// <summary>Called by the browser module while the window moves.</summary>
    [JSInvokable]
    public Task NotifyWindowInput(double low, double high) => WindowInput.InvokeAsync(new WebGlImageWindow(low, high));

    /// <summary>Called by the browser module when the values or the window change.</summary>
    [JSInvokable]
    public Task NotifyStats(WebGlImageWindowStats stats) => StatsChanged.InvokeAsync(stats);

    public async ValueTask DisposeAsync()
    {
        if (disposed) return;
        disposed = true;
        try
        {
            if (instance is not null)
            {
                await instance.InvokeVoidAsync("dispose");
                await instance.DisposeAsync();
            }

            if (module is not null) await module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone: the browser released the histogram with the page.
        }
        catch (TaskCanceledException)
        {
        }

        reference?.Dispose();
        GC.SuppressFinalize(this);
    }
}
