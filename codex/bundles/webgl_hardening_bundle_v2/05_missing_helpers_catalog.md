# Missing Generic Helpers / Services Catalog

This is the checklist of generic helper surfaces that are still missing or too weak for future run/game/simulation usage.

## JavaScript runtime helpers

### Runtime safety

- `webgl-scene-dom.js`: safe DOM creation helpers; no `innerHTML` for runtime UI.
- `webgl-scene-notifications.js`: all DotNet callback wrapping, JSON serialization, and callback error handling.
- `webgl-scene-disposal.js`: deterministic disposal helpers with resource ownership semantics.
- `webgl-scene-runtime-errors.js`: normalized error creation and diagnostics recording.

### Model import / GLB diagnostics

- `webgl-scene-model-diagnostics.js`: bounds, visibility, material, transform, mesh-count, and camera-fit diagnostics.
- `webgl-scene-model-normalization.js`: unit scale, center mode, axis correction, material normalization, and debug bounds.
- `webgl-scene-model-lab.js` or sandbox page support helpers for inspecting one asset at a time.

### Render loop / scheduling

- `webgl-scene-render-scheduler.js`: wake/sleep scheduler instead of perpetual requestAnimationFrame loop.
- `webgl-scene-frame-budget.js`: frame time and object/triangle budget warnings.

### Command API

- `webgl-scene-command-result.js`: normalized result object for patch/import/motion commands.
- `webgl-scene-patch-validation.js`: shared JS validation aligned with C# reducer semantics.
- `webgl-scene-motion-events.js`: accepted/completed/cancelled/failed events.

## C# contracts/helpers

- `WebGlSceneDocument`: generic serializable scene layout document.
- `WebGlSceneDocumentSerializer`: JSON serialize/deserialize with schema version and validation.
- `WebGlSceneCommandResult`: generic command result for JS interop operations.
- `WebGlModelImportOptions`: per asset/variant model normalization options.
- `WebGlModelDiagnostics`: diagnostics DTO for invisible/misaligned models.
- `WebGlSceneRuntimeCapabilities`: tells consumers which runtime features are available.
- `WebGlSceneValidationResult`: general scene validation result before render.

## Tooling helpers

- `tools/webgllib/audit-scene-runtime.cjs`: line counts, forbidden unsafe patterns, syntax, public namespace checks.
- model inventory should output both human markdown and machine JSON.
- browser proof should capture proof snapshot, diagnostics, and console logs.

