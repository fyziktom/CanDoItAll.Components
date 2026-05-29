# SB05 - Scene patch, export, and import

## Goal

Prepare the renderer for simulations/runs without adding run semantics.

## Tasks

1. Add `WebGlScenePatch` contracts.
2. Add object patch support:
   - position,
   - rotation,
   - scale,
   - color,
   - symbols,
   - metadata.
3. Implement `applyPatch(host, patch)` in JS without full rebuild for transform-only changes.
4. Add `exportScene(host)` JS API.
5. Add `WebGlSceneView.ExportSceneAsync()`.
6. Add sandbox proof:
   - move object,
   - export scene,
   - reset page state,
   - import/apply exported scene,
   - verify position persists.

## Acceptance criteria

- Simple object transform patch does not rebuild all objects.
- Add/remove object patch works.
- Add/remove link patch works.
- Exported scene can be serialized to JSON and reloaded.
