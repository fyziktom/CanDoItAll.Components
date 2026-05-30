# Components review findings

## Current positive state

- `CanDoItAll.Components.WebGlRunLib` is now present in the solution and references `CanDoItAll.Components.WebGlLib`.
- `WebGlLib` now has a richer runtime API including scene import/export, patching, command batches, motion commands, diagnostics, model diagnostics, scene indexes, render scheduler, and resource ownership helpers.
- `WebGlRunLib` now contains action-oriented contracts: `WebGlRunEvent`, `WebGlRunAction`, target anchors, visual state catalog, pose definitions, symbols, action bindings, and common action kinds.
- The sandbox has a generic run playback page and tests exist for run action planning.

## Main concerns

1. `WebGlRunLib` still looks contract-heavy and may not yet contain a robust reusable playback controller/service that the sandbox consumes. If the sandbox still orchestrates playback directly, move that to `WebGlRunLib`.
2. `WebGlRunActions.cs` mixes event contracts, action contracts, target models, visual state catalogs, action kind constants, anchor constants, and pose/symbol definitions. Split it before it becomes a long-term dumping ground.
3. There is duplicated batch normalization/coalescing logic between C# and JavaScript. This is a drift risk.
4. Command batching currently drops duplicate motions per object unless explicitly allowed. That is dangerous for ordered paths and multi-leg actions such as "go to well -> perform admin -> return home".
5. Patch coalescing can be unsafe when patches contain add/remove operations or order-dependent changes.
6. `21-webgl-scene-asset-cache.js` provides `disposeAssetCache`, but lifecycle code must explicitly call it during `dispose`; verify and patch if missing.
7. Link updates still risk O(moving objects × linkGroups) behavior if many agents move at once. Create link-group indexes by object id.
8. The WebGL repo must not spend time on small/medium screen optimization. Desktop/large-screen only.

## Recommended action

Make the next wave a hardening wave, not a feature wave. The only new feature surface should be the generic action planning/sequence model needed for scenarios such as shared well community and small entrepreneur market.
