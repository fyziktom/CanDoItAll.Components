# Current `webgl-engine` Review Summary

## Positive findings

The implementation is moving in the right direction.

- `01-webgl-scene.js` is now a thin public façade that delegates lifecycle, patching, motion, proof snapshots, diagnostics, and commands to dedicated modules.
- `10-webgl-scene-lifecycle.js` owns state creation, disposal, runtime notification, and handler registration.
- `11-webgl-scene-graph.js` owns object/link runtime graph operations.
- `12-webgl-scene-drag.js` separates drag-on-ground-plane behavior.
- `13-webgl-scene-patching.js` adds patch and detailed command result support.
- `14-webgl-scene-motion.js` adds a generic target-position interpolation primitive.
- `15-webgl-scene-render-loop.js` introduces render reasons and render modes.
- `16-webgl-scene-models.js`, `17-webgl-scene-resources.js`, and `18-webgl-scene-model-diagnostics.js` separate model loading, resource ownership, and visibility diagnostics.
- `19-webgl-scene-shell.js` separates DOM shell construction.
- `WebGlSceneDocument` and serializer exist.
- `ModelLab` exists for per-model diagnostics.
- `tools/webgllib/audit-scene-runtime.cjs` provides a good first JS hygiene gate.

## Important remaining concerns

### 1. Runtime command results are duplicated and fragile

Both patching and motion currently implement their own command result creation. This creates drift risk. There is also at least one fragile call site in patching where `failPatch` can be called with the wrong argument shape when adding an invalid object.

Fix by extracting a small shared JS command-result module and adding tests for malformed patch commands.

### 2. Render loop still keeps a permanent requestAnimationFrame chain

The loop can skip rendering when idle, but it still schedules `requestAnimationFrame` continuously. This is better than rendering continuously, but not ideal for large scenes, laptops, and long-running dashboards.

Fix with a true idle scheduler:
- one rAF while active,
- no rAF while idle,
- restart on invalidation/motion/camera/symbol animation,
- optional continuous mode.

### 3. Model import diagnostics are useful but not complete

The Model Lab is a good diagnostic surface, but invisible GLB issues need a more complete pipeline:
- import recipe presets per asset/variant,
- material/opacity/side normalization,
- automatic camera/bounds framing,
- model axes/bounds helper,
- batch diagnostics for all registered external models,
- proof artifact that flags invisible/near-zero/extreme/far-from-origin models.

### 4. WebGlSceneDocument needs stronger deterministic persistence semantics

The current document serializer is useful but should distinguish:
- `SceneContentHash`: stable hash of the scene payload, excluding volatile state such as `SavedAtUtc`, hover, selection, and document identity.
- `DocumentHash`: optional hash of the complete stored document.
- Migration path for future schema versions.
- Recursive validation against run/economy metadata leaking into generic scene documents.

### 5. WebGlLib is not a run engine

Keep WebGlLib as a generic rendering/interaction engine. A new layer above it should own run clocks, scenario playback, save slots, deterministic replay, persistence providers, path planning, and domain event orchestration.

### 6. Economy simulation must avoid reference mixing

Future economy scenarios such as a shared well or small entrepreneur community must not mix the ledger simulator and simple-account simulator accidentally. Shared abstractions belong in a dependency-light project, with separate adapters for ledger and simple accounts.
