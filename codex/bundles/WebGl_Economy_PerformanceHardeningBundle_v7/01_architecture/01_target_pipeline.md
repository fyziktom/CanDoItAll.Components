# Target pipeline

## Components side

```text
WebGlSceneDocument
    -> WebGlSceneView
    -> JS WebGL runtime

WebGlRunDocument
    -> WebGlRunPlaybackController
    -> WebGlRunActionPlanner
    -> WebGlSceneCommandBatch
    -> WebGlSceneView.ApplyCommandBatchAsync(...)
```

`WebGlLib` stays a generic scene renderer.

`WebGlRunLib` becomes a generic run/action/playback layer over `WebGlLib`.

`WebGlSandbox` demonstrates generic scenarios only. It must not contain Economy semantics.

## Economy side

```text
SimulationScenarioDefinition
    -> SimulationScenarioLoader
    -> SimulationEventStream
    -> ISimulationBackend materialization
    -> SimulationFrame / SimulationFrameDelta
    -> EconomyVisualFrame / EconomyVisualAction
```

No WebGL types in Economy.

## Future bridge, not now

```text
EconomyVisualAction
    -> EconomyWebGlActionMapper
    -> WebGlRunAction
    -> WebGlRunActionPlanner
    -> WebGlSceneCommandBatch
```

The future bridge belongs in a separate integration layer/package, not in the current wave.
