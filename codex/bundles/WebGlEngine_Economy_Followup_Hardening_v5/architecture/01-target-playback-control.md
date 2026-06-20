# Target playback control architecture

Playback control must be a layered contract:

```text
Blazor UI command -> WebGlRun host controller -> WebGlRun runner -> WebGlLib runtime stop/apply APIs -> JS stage runner + motion queues
```

Pause/Cancel must stop both C# scheduling and browser runtime activity. A UI flag alone is not sufficient.

Target APIs:

- `WebGlSceneView.StopRuntimeActivityAsync(reason)`
- `window.CanDoItAll.webglScene.stopRuntimeActivity(host, reason)`
- `WebGlRunDocumentRunner.StopAsync(reason)` or equivalent host-level command
- browser proof diagnostics: `lastRuntimeStopReason`, `runtimeStopCount`, `clearedMotionCount`, `cancelledCommandStageCount`
