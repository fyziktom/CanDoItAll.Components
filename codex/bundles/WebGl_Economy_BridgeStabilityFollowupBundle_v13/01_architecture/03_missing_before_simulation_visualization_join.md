# What is still missing before joining simulation and visualization

## Missing in bridge

1. Initial scene projector:
   - converts `EconomyVisualFrame` nodes/links into `WebGlSceneModel`.
   - creates stable object ids.
   - resolves asset ids and visual mappings.

2. Node-to-object index:
   - maps economy node ids to WebGL object ids.
   - fails loudly when required subject/target cannot resolve.
   - no silent fallback except explicit diagnostic placeholder.

3. Action-to-stage compiler:
   - maps `EconomyVisualAction` -> `WebGlRunAction`.
   - maps `WebGlRunActionPlan` -> `WebGlSceneCommandBatchStage`.
   - preserves order for sequences.
   - coalesces only independent stages.

4. WebGlRunDocument builder:
   - includes `InitialScene`.
   - includes timeline frames with actual patches/motions/batches.
   - includes provenance and source refs.
   - validates document before returning.

5. Visual mapping loader:
   - consumes `visual.mapping.json`.
   - resolves actor/resource/institution categories to assets, poses, symbols, and anchors.
   - keeps mapping outside hardcoded C# scenario logic.

## Missing in Components

1. Real stage executor semantics:
   - stage barriers.
   - wait/delay support.
   - per-object motion queue.
   - motion-complete continuation.

2. Generic action plan to command batch conversion:
   - deterministic stages.
   - no duplicate motion drop in ordered sequences.
   - C# and JS parity.

## Missing in Economy Sandbox

1. `CanDoItAll.Economy.SimulationSandbox` or equivalent example host.
2. It should live in Economy repo.
3. It can reference:
   - Economy simulation projects
   - Economy bridge
   - Components WebGL packages/project references
4. It must not move bridge responsibilities back into Components.
