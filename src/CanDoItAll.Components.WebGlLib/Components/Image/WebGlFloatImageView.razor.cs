using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.WebGlLib;

/// <summary>
/// A scientific float image drawn with WebGL2: raw samples (any <see cref="WebGlImageElementType"/>) are shown through
/// a display window, gamma, optional inversion and smoothing, as the samples themselves or as the negative log ratio
/// against a reference plane. Invalid pixels (masked or non-finite) are drawn in <see cref="InvalidColor"/>; a region
/// and a profile row can be outlined. The pointer pans, the wheel zooms, a double-click fits the image again, and
/// <see cref="OnProbe"/> reports the pixel under the pointer. Everything is display only: the samples never change.
/// Without WebGL2 the image is not drawn, but it is still loaded, so its statistics and a linked
/// <see cref="WebGlImageWindowHistogram"/> keep working.
/// </summary>
public partial class WebGlFloatImageView : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.WebGlLib/js/image/webgl-image-display.js";

    private readonly string generatedId = "cda-image-" + Guid.NewGuid().ToString("N");
    private ElementReference host;
    private ElementReference canvas;
    private IJSObjectReference? module;
    private IJSObjectReference? instance;
    private DotNetObjectReference<WebGlFloatImageView>? reference;
    private WebGlImageDisplayState? appliedDisplay;
    private WebGlImageDisplayMode? appliedMode;
    private string? requestedKey;
    private WebGlImageWindow? automaticWindow;
    private string? statusMessage;
    private bool disposed;

    /// <summary>
    /// The element id of the view; a <see cref="WebGlImageWindowHistogram"/> links to the view by it. A unique id is
    /// generated when none is given.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>The image to show; null shows none. A different key loads again.</summary>
    [Parameter]
    public WebGlImageSource? Source { get; set; }

    /// <summary>The display window; null uses the image's suggested window.</summary>
    [Parameter]
    public WebGlImageWindow? Window { get; set; }

    /// <summary>Gamma of the grey levels inside the window (1: linear).</summary>
    [Parameter]
    public double Gamma { get; set; } = 1;

    /// <summary>High values dark and low values light.</summary>
    [Parameter]
    public bool Invert { get; set; }

    [Parameter]
    public WebGlImageDisplayMode Mode { get; set; } = WebGlImageDisplayMode.Linear;

    /// <summary>Interpolates between pixel centres for display (the samples are unchanged).</summary>
    [Parameter]
    public bool Smooth { get; set; }

    /// <summary>A region outlined on the image, in image pixels.</summary>
    [Parameter]
    public WebGlImageRegion? Region { get; set; }

    /// <summary>A row marked on the image (for a profile along it).</summary>
    [Parameter]
    public double? ProfileRow { get; set; }

    [Parameter]
    public string AriaLabel { get; set; } = "Image";

    [Parameter]
    public string? Class { get; set; }

    /// <summary>Hex colour of invalid pixels (#ff00ff by default).</summary>
    [Parameter]
    public string? InvalidColor { get; set; }

    /// <summary>Hex colour of the region's outline (#ffbf00 by default).</summary>
    [Parameter]
    public string? RegionColor { get; set; }

    /// <summary>Hex colour of the profile row (#1a99ff by default).</summary>
    [Parameter]
    public string? ProfileColor { get; set; }

    /// <summary>The view writes what it cannot draw (no WebGL2, a lost context, a failed load) over the image.</summary>
    [Parameter]
    public bool ShowStatus { get; set; } = true;

    /// <summary>The pixel under the pointer; null when the pointer leaves the image. At most once per frame.</summary>
    [Parameter]
    public EventCallback<WebGlImageProbe?> OnProbe { get; set; }

    /// <summary>A source was loaded (or could not be).</summary>
    [Parameter]
    public EventCallback<WebGlImageLoadResult> OnLoaded { get; set; }

    [Parameter]
    public EventCallback<WebGlImageViewStatusEventArgs> OnStatusChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>The view's element id (see <see cref="Id"/>).</summary>
    public string ElementId => string.IsNullOrWhiteSpace(Id) ? generatedId : Id;

    /// <summary>What the view can draw.</summary>
    public WebGlImageViewStatus Status { get; private set; } = WebGlImageViewStatus.Ready;

    /// <summary>The last loaded image (null before the first load).</summary>
    public WebGlImageLoadResult? Loaded { get; private set; }

    /// <summary>The window drawn now: <see cref="Window"/>, or the image's suggested window when that is null.</summary>
    public WebGlImageWindow? EffectiveWindow => Window ?? automaticWindow;

    private string CssClass => string.IsNullOrWhiteSpace(Class) ? "ui-webgl-image" : "ui-webgl-image " + Class;

    /// <summary>
    /// A window over the displayed values in the current mode: the given quantiles of the valid pixels (0.5 % and
    /// 99.5 % by default). Null without an image or valid values.
    /// </summary>
    public async ValueTask<WebGlImageWindow?> SuggestWindowAsync(double lowQuantile = 0.005, double highQuantile = 0.995)
    {
        if (instance is null || disposed) return null;
        // The displayed quantity is the one the parameters name now, even before this view's next render.
        await ApplyDisplayAsync();
        var window = await instance.InvokeAsync<JsWindow?>("suggestWindow", lowQuantile, highQuantile);
        return window is { } w ? new WebGlImageWindow(w.Low, w.High) : null;
    }

    /// <summary>Fits the whole image into the view again (after panning or zooming).</summary>
    public async ValueTask ResetViewAsync()
    {
        if (instance is not null && !disposed) await instance.InvokeVoidAsync("resetView");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (disposed) return;
        if (firstRender)
        {
            module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
            reference = DotNetObjectReference.Create(this);
            instance = await module.InvokeAsync<IJSObjectReference>("createImageView", host, canvas, reference, new
            {
                id = ElementId,
                probe = OnProbe.HasDelegate,
                invalidColor = InvalidColor,
                regionColor = RegionColor,
                profileColor = ProfileColor,
            });
        }

        if (instance is null) return;
        await ApplyDisplayAsync();
        await LoadAsync();
    }

    private async Task ApplyDisplayAsync()
    {
        if (instance is null) return;
        if (Window is null && Loaded is not null && appliedMode is { } before && before != Mode)
        {
            // Another displayed quantity: the automatic window follows it.
            appliedMode = Mode;
            await instance.InvokeVoidAsync("setDisplay", new { mode = WebGlImageDisplayState.ModeName(Mode) });
            var suggested = await instance.InvokeAsync<JsWindow?>("suggestWindow", 0.005, 0.995);
            automaticWindow = suggested is { } s ? new WebGlImageWindow(s.Low, s.High) : null;
        }

        appliedMode = Mode;
        var display = WebGlImageDisplayState.From(EffectiveWindow, Gamma, Invert, Mode, Smooth, Region, ProfileRow);
        if (display == appliedDisplay) return;
        appliedDisplay = display;
        await instance.InvokeVoidAsync("setDisplay", display.ToJs());
    }

    private async Task LoadAsync()
    {
        string? key = Source?.Key;
        if (key == requestedKey || instance is null) return;
        requestedKey = key;
        if (Source is null)
        {
            await instance.InvokeVoidAsync("clear");
            Loaded = null;
            automaticWindow = null;
            return;
        }

        var result = await instance.InvokeAsync<WebGlImageLoadResult>("load", WebGlImageDisplayState.SourceToJs(Source));
        if (disposed || result.Superseded || result.Key != requestedKey) return;
        Loaded = result;
        automaticWindow = result.SuggestedWindow;
        if (Window is null) await ApplyDisplayAsync();
        await OnLoaded.InvokeAsync(result);
    }

    /// <summary>Called by the browser module with the pixel under the pointer.</summary>
    [JSInvokable]
    public Task NotifyProbe(WebGlImageProbe? probe) => OnProbe.InvokeAsync(probe);

    /// <summary>Called by the browser module when what the view can draw changes.</summary>
    [JSInvokable]
    public async Task NotifyStatus(string kind, string message)
    {
        Status = kind switch
        {
            "unavailable" => WebGlImageViewStatus.Unavailable,
            "context-lost" => WebGlImageViewStatus.ContextLost,
            "too-large" => WebGlImageViewStatus.TooLarge,
            "load-failed" => WebGlImageViewStatus.LoadFailed,
            _ => WebGlImageViewStatus.Ready,
        };
        statusMessage = string.IsNullOrWhiteSpace(message) ? null : message;
        await OnStatusChanged.InvokeAsync(new WebGlImageViewStatusEventArgs(Status, message ?? ""));
        StateHasChanged();
    }

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
            // The circuit is gone: the browser released the view with the page.
        }
        catch (TaskCanceledException)
        {
        }

        reference?.Dispose();
        GC.SuppressFinalize(this);
    }

    internal sealed record JsWindow(double Low, double High);
}
