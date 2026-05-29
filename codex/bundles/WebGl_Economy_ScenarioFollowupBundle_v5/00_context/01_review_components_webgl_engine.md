# Review findings: CanDoItAll.Components / branch `webgl-engine`

## What is good

- `CanDoItAll.Components.WebGlRunLib` now exists in the solution and references only `WebGlLib`.
- `01-webgl-scene.js` is now a public façade rather than a monolithic runtime file.
- WebGL runtime code has been split into lifecycle, graph, resources, diagnostics, scheduler, command results, scene indexes, and asset cache modules.
- `WebGlSceneView.razor` exposes generic scene APIs for import/export, patching, motion, diagnostics, and detailed command results.
- `WebGlRunLib` has a first DTO layer: run document, timeline, frames, frame patches, motions, playback state, frame source, scene projector, and snapshot store.
- The sandbox contains generic run playback proof code.

## Remaining risks

### C-WEBGL-001: WebGlRunLib is still mostly contracts

`WebGlRunLib` currently contains DTOs and interfaces, but it does not yet contain a reusable playback controller implementation, timeline validator, action compiler, event-to-command mapper, or replay-state reducer. The sandbox `RunPlayback.razor.cs` still performs playback imperatively in the UI.

Expected fix:
- Move reusable logic out of sandbox into `WebGlRunLib`.
- Keep the sandbox as a consumer/demo only.

### C-WEBGL-002: Runtime action/event layer is missing

The next scenario examples require a generic mapping from a simulation-like event to visual actions:

- actor goes to target object;
- actor returns home;
- actor changes pose/visual asset;
- actor shows symbol above head;
- actor performs admin/writing state;
- resource transfer is visualized as line/marker/symbol;
- multiple visual actions compose into one timeline segment.

This belongs to `WebGlRunLib`, not to `WebGlLib`.

### C-WEBGL-003: Scene indexes may become stale after patch commands

`23-webgl-scene-indexes.js` builds object/link/layer indexes during rebuild. Patch operations can add/remove objects and links without always rebuilding indexes. This can break layer visibility, linking, selection, or later action target resolution.

Expected fix:
- Introduce `syncSceneIndexes(state, reason)` and call it after add/remove/replace/link patch operations.
- Add tests/proofs for adding/removing an object with layer metadata and then selecting/moving it.

### C-WEBGL-004: Asset cache helper exists but must be fully integrated

`21-webgl-scene-asset-cache.js` introduces a state-local cache and `disposeAssetCache`, but lifecycle disposal must explicitly call it. Initial state should use the cache factory rather than a raw `Map`.

Expected fix:
- Initialize asset cache using `createAssetCache`.
- Call `disposeAssetCache(state)` from lifecycle dispose.
- Expose cache counters in diagnostics and browser proof.

### C-WEBGL-005: Scene document serializer is too large

`WebGlSceneDocumentSerializer.cs` has grown into serialization, normalization, validation, hashing, metadata filtering, sorting, and vector validation. This is now a maintenance risk.

Expected fix:
- Split into:
  - `WebGlSceneDocumentSerializer`
  - `WebGlSceneDocumentNormalizer`
  - `WebGlSceneDocumentValidator`
  - `WebGlSceneDocumentHasher`
  - `WebGlSceneDocumentMetadataPolicy`

### C-WEBGL-006: Command results need end-to-end consistency

JS has `20-webgl-scene-command-results.js`, C# has `WebGlSceneCommandResult`, and `WebGlSceneView` has detailed methods. Ensure all command paths use the same result builder:

- patch;
- motion enqueue;
- motion completed;
- import scene;
- layer visibility toggle;
- actor action compilation later.

### C-WEBGL-007: WebGlRunLib must define generic action semantics

The shared-well scenario requires action plans like:

- move object A to object B;
- return object A to home anchor;
- temporarily switch asset/pose;
- show symbol above A;
- then restore previous visual state.

These are generic rendering/playback concepts and should not be economy-specific.

## Guardrail

No economy words such as `water`, `well`, `citizen`, `ledger`, `account`, `market`, or `entrepreneur` should appear inside `WebGlLib` or `WebGlRunLib`, except inside neutral examples/docs in sandbox/test assets where clearly marked as sample content.
