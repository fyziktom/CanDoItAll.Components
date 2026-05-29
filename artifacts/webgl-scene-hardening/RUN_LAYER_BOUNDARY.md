# WebGlLib vs Future WebGlRunLib Boundary

Large-screen-only hard rule: this bundle does not tune or validate small-screen layouts.

## Stays In WebGlLib

- Declarative scene contracts: `WebGlSceneModel`, objects, links, camera, environment, UI state.
- Asset catalogs, variants, performance hints, quality profiles, GLB loading, and primitive fallback.
- Generic interaction: hover, selection, drag-on-ground-plane, camera controls, overlays.
- Generic render-layer commands: object transform, scene patch, export/import, proof snapshots.
- Generic motion primitive: object transform interpolation without simulation semantics.
- Runtime diagnostics and render loop policy.

## Moves To Future WebGlRunLib

- Simulation clock semantics and run lifecycle.
- Scenario playback, save slots, replay logs, and persistence providers.
- Path planning, physics, collision, and domain event orchestration.
- Economy, process, game, or agent-rule semantics.
- Network synchronization and domain-specific symbol policies.

Practical rule: if it can render or interact with any 3D scene, it belongs in `WebGlLib`; if it decides how a world evolves over time, it belongs above the renderer.
