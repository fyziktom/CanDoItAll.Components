# Current review findings

## Components repo

Relevant observations:

- `CanDoItAll.Components.slnx` includes `CanDoItAll.Components.WebGlRunLib`, `WebGlLib`, `WebGlSandbox`, and test projects. This is the correct place for generic run/playback and renderer infrastructure.
- `WebGlSceneCommandBatch` now has `BatchOrderingMode`, `Stages`, `BatchingPolicy`, and metrics for coalescing/preserved duplicate motion count.
- JS runtime has `26-webgl-scene-command-batch.js` and `28-webgl-scene-command-batch-normalizer.js`.
- The JS batch executor iterates stages, but currently applies stage patches/motions immediately in a single pass. `waitSeconds` is preserved by normalization, but does not yet act as a real stage barrier.
- Motion enqueue still replaces existing per-object motions unless `queueMode=append`, and append is not yet a true ordered per-object queue.

## Economy repo

Relevant observations:

- `CanDoItAll.Economy.slnx` now includes `CanDoItAll.Economy.Simulation.WebGlBridge`.
- The bridge project references `Simulation.Abstractions`, `Simulation.Visualization`, and the sibling Components `WebGlRunLib`.
- `EconomyWebGlBridgeContracts.cs` maps `EconomyVisualAction` into `WebGlRunAction`.
- `EconomyWebGlRunProjector` creates `WebGlRunDocument` and frames/stages, but the stages currently contain metadata only. It does not yet emit command batches, run actions, scene patches, motions, initial scene objects, or node/object mappings.
- `EconomyWebGlRunProjector` uses `visualFrame.Actions.Concat(input.Actions)` inside every frame. That risks duplicating global actions into each frame.
- Strict input pack validation exists, including safe relative paths, hash format, file existence, content hash, and pack hash checks.
- Simulation policies and transition engine are now useful but still broad; continue splitting by responsibility.

## Main conclusion

The architecture direction is right, but the bridge is not yet a functional simulation-to-visualization pipeline. It is currently closer to a traceability skeleton. This bundle makes it a robust generic bridge without creating the final demo.
