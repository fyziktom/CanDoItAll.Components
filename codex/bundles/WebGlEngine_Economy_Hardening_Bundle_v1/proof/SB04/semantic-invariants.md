# SB04 Semantic Invariants

Subbundle: `SB04-incremental-render-performance`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB04-TRANSFORM-001 | REQ-005 | Transform-only patches update object transforms, dependent link geometry, and symbol positions without incrementing `fullSceneRebuildCount` or `sceneIndexSyncCount`. | Incrementing a new `transformOnlyPatchCount` while still calling `rebuildScene`. | `bundle://proof/SB04/transcripts/failing-first-transform-patches-rebuild.json` records 100 transform patches causing `sceneIndexSyncCount` delta 100 and missing required counters. | `bundle://proof/SB04/transcripts/passing-transform-patches-no-rebuild.json` records 100 transform patches with `fullSceneRebuildCount` delta 0, `sceneIndexSyncCount` delta 0, and `transformOnlyPatchCount` delta 100. |
| SB04-SYMBOL-001 | REQ-005 | Symbol-only patches rebuild only the target object's symbol groups and do not trigger a full scene rebuild. | Rebuilding the whole scene to update one object's status symbols. | The failing-first transform proof names the pre-SB04 rebuild pattern; symbol-only was previously covered only by `symbolOnlyUpdateCount` without full-rebuild diagnostics. | `bundle://proof/SB04/transcripts/passing-symbol-link-only-no-rebuild.json` records `symbolOnlyPatchCount` delta 1, `symbolOnlyUpdateCount` delta 1, and `fullSceneRebuildCount` delta 0. |
| SB04-LINK-001 | REQ-005 | Link-only add/remove patches update link groups and visibility indexes without a full scene rebuild. | Adding/removing links by rebuilding all objects, symbols, and links. | The old generic patch tail called `rebuildScene` for every changed patch; transform failing-first proof captures that common failure path. | `bundle://proof/SB04/transcripts/passing-symbol-link-only-no-rebuild.json` records `linkOnlyPatchCount` delta 2, final `linkCount` delta 0, and `fullSceneRebuildCount` delta 0. |
| SB04-DIAG-001 | REQ-005, REQ-015 | Runtime diagnostics and proof snapshots expose `fullSceneRebuildCount`, patch classification counters, `lastPatchClassification`, and `linkGeometryUpdateCount` through JS and C# interop DTOs. | Browser-only counters that C# callers cannot deserialize, or command results without classification metadata. | Missing counters are recorded as `null` in the failing-first browser proof. | Focused diagnostics test transcript passes; passing browser proof records command-result diagnostics and proof-snapshot metadata for the new counters. |
| SB04-BOUNDARY-001 | REQ-001, REQ-015 | Incremental render behavior stays generic and belongs to `WebGlLib`; no run-layer or Economy/domain concepts are introduced. | Special-casing the generated stress scene or tycoon-village route inside production runtime code. | `bundle://proof/SB04/transcripts/sb04-anti-stub-and-boundary-scan.txt` would surface forbidden terms or placeholders. | The same scan passes; browser proof uses public generic import/patch APIs and generated object ids only in proof scripts. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Patch classification | `36-webgl-scene-patch-classification.js` | `applyPatchDetailed`, command results, diagnostics/proof snapshots | Classifies normalized patches as transform-only, symbol-only, link-only, visual-replace, mixed-incremental, graph-structure, scene-rebuild, or no-op. | Failing-first proof shows no required classification counters before SB04. |
| Full scene rebuild counter | `rebuildScene` in `11-webgl-scene-graph.js` | Browser proof, C# diagnostics DTO, proof snapshots | Increments only when the runtime actually rebuilds the full dynamic scene. | Passing transform proof requires delta 0 across 100 transform patches. |
| Incremental patch counters | `recordPatchClassificationDiagnostics` | Browser diagnostics, command results, C# DTOs | Increment after successful mutating patches according to classification. | Failing-first proof records `transformOnlyPatchCount=null` before the fix. |
| Link geometry update counter | `syncLinksForObject` in `27-webgl-scene-links.js` | Browser proof and C# diagnostics DTO | Increments when dependent link geometry is updated because an object moved. | Passing transform proof requires 199 link geometry updates for 100 moved objects in a linked chain. |
| Browser stress transcript/screenshot | `/tycoon-village` route plus public scene import/patch APIs | Bundle progression gate and later SB13 performance comparison | Imports 250 primitive objects and 249 links, applies 100 transform patches, records diagnostics and screenshot. | Failing-first proof records the pre-fix rebuild-equivalent behavior. |

## Reopen Triggers

- `fullSceneRebuildCount` or `sceneIndexSyncCount` increments during transform-only stress patches.
- `transformOnlyPatchCount`, `symbolOnlyPatchCount`, or `linkOnlyPatchCount` stops moving for the matching patch class.
- Command results or C# diagnostics lose the SB04 counters.
- Link geometry no longer updates when transformed objects have connected links.
- Touched `WebGlLib` runtime/DTO files gain run-layer or domain-specific concepts.
