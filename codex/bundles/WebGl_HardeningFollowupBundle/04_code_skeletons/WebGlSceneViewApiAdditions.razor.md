# WebGlSceneView API additions

Add methods to `WebGlSceneView.razor`:

```csharp
public Task<WebGlSceneModel?> ExportSceneAsync()
    => JsRuntime.InvokeAsync<WebGlSceneModel?>("CanDoItAll.webglScene.exportScene", host).AsTask();

public Task<bool> ApplyPatchAsync(WebGlScenePatch patch)
    => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.applyPatch", host, patch).AsTask();

public Task<bool> EnqueueMotionAsync(WebGlObjectMotionCommand command)
    => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.enqueueMotion", host, command).AsTask();

public Task<bool> ClearMotionsAsync(string? objectId = null)
    => JsRuntime.InvokeAsync<bool>("CanDoItAll.webglScene.clearMotions", host, objectId).AsTask();
```

Error behavior:
- Return `false` for invalid commands.
- Also update runtime diagnostics with the command failure.
- Do not throw JS exceptions for expected validation errors.
```
