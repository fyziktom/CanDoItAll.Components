# WebGlSceneView component skeleton

```razor
@namespace CanDoItAll.Components.WebGlLib
@implements IAsyncDisposable
@inject IJSRuntime JsRuntime

<div class="wgl-scene-shell"
     data-testid="webgl-scene-shell"
     data-webgl-scene-id="@Scene.SceneId">
    <div @ref="host"
         class="wgl-scene-host"
         data-testid="webgl-scene-host"
         tabindex="0"
         aria-label="@AriaLabel">
    </div>
</div>

@code {
    private static readonly System.Text.Json.JsonSerializerOptions SerializerOptions = new(System.Text.Json.JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private ElementReference host;
    private DotNetObjectReference<WebGlSceneView>? objectReference;
    private string? pendingSceneKey;
    private string? appliedSceneKey;

    [Parameter, EditorRequired]
    public WebGlSceneModel Scene { get; set; } = default!;

    [Parameter]
    public WebGlRuntimeOptions Options { get; set; } = new();

    [Parameter]
    public string AriaLabel { get; set; } = "WebGL scene";

    [Parameter]
    public EventCallback<WebGlSceneSelectionChangedEventArgs> SelectionChanged { get; set; }

    [Parameter]
    public EventCallback<WebGlSceneHoverChangedEventArgs> HoverChanged { get; set; }

    [Parameter]
    public EventCallback<string> StateChanged { get; set; }

    [Parameter]
    public EventCallback<WebGlRuntimeReadyEventArgs> RuntimeReady { get; set; }

    [Parameter]
    public EventCallback<WebGlRuntimeErrorEventArgs> RuntimeError { get; set; }

    protected override void OnParametersSet()
    {
        pendingSceneKey = SerializePayload(Scene, Options);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        objectReference ??= DotNetObjectReference.Create(this);

        if (string.IsNullOrWhiteSpace(appliedSceneKey))
        {
            var created = await JsRuntime.InvokeAsync<bool>(
                "CanDoItAll.webglScene.create",
                host,
                objectReference,
                Scene,
                Options);

            if (created)
            {
                appliedSceneKey = pendingSceneKey;
            }

            return;
        }

        if (string.Equals(appliedSceneKey, pendingSceneKey, StringComparison.Ordinal))
        {
            return;
        }

        var updated = await JsRuntime.InvokeAsync<bool>(
            "CanDoItAll.webglScene.update",
            host,
            Scene,
            Options);

        if (updated)
        {
            appliedSceneKey = pendingSceneKey;
        }
    }

    [JSInvokable]
    public Task OnSceneSelectionChanged(string eventJson)
    {
        var args = Deserialize<WebGlSceneSelectionChangedEventArgs>(eventJson);
        return args is null ? Task.CompletedTask : SelectionChanged.InvokeAsync(args);
    }

    public Task FitViewAsync()
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.fitView", host).AsTask();

    public Task ResetCameraAsync()
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.resetCamera", host).AsTask();

    public Task FocusObjectAsync(string objectId)
        => JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.focusObject", host, objectId).AsTask();

    public Task<WebGlSceneProofSnapshot?> GetProofSnapshotAsync()
        => JsRuntime.InvokeAsync<WebGlSceneProofSnapshot?>("CanDoItAll.webglScene.getProofSnapshot", host).AsTask();

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("CanDoItAll.webglScene.dispose", host);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
        }

        objectReference?.Dispose();
    }

    private static string SerializePayload(WebGlSceneModel scene, WebGlRuntimeOptions options)
        => System.Text.Json.JsonSerializer.Serialize(new { scene, options }, SerializerOptions);

    private static T? Deserialize<T>(string json)
        => System.Text.Json.JsonSerializer.Deserialize<T>(json, SerializerOptions);
}
```
