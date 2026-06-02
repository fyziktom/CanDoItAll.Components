# SB08 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB08-INV-001 | Disposing a state-local asset cache while GLB/GLTF template promises are pending clears entries immediately, exposes the pending cleanup count, disposes resolved templates, and drains the pending count to zero. | Only disposing already-resolved promises, or hiding late cleanup behind an unobservable continuation. | `proof/SB08/transcripts/failing-first-resource-ownership.txt` failed because pending-disposal diagnostics were absent. | `proof/SB08/transcripts/passing-resource-ownership-final.txt` and `proof/SB08/browser/high-glb-stress-diagnostics.json`. | `21-webgl-scene-asset-cache.js`, `25-webgl-scene-diagnostics.js`, `02-webgl-scene-core.js`, `08-webgl-scene-proof.js` | SB11 browser proof can trust resource diagnostics during route stress. |
| SB08-INV-002 | Tinted model instances dispose owned cloned material objects but retain shared template textures; template disposal remains responsible for shared template geometry/material/texture cleanup. | Marking all cloned instances as owning textures, causing shared template textures to be disposed early. | Existing test harness had no combined template/instance separation test before SB08. | `tinted-instance-template-ownership-separation` in `passing-resource-ownership-final.txt`. | `17-webgl-scene-resources.js`, `16-webgl-scene-models.js`, `tools/webgllib/test-resource-ownership.mjs` | High-GLB routes can tint instances without corrupting cached templates. |
| SB08-INV-003 | Repeated high-GLB create/dispose cycles do not monotonically accumulate object groups, hit meshes, label elements, or disposed cache entries after pending promises settle. | A screenshot of a loaded scene without forcing dispose while model loads are still in flight. | F08 finding documented that async load/dispose race proof was thin. | `high-glb-stress-diagnostics.json` shows six cycles with pending disposal scheduled, then drained; cache entries and scene collections are cleared after each dispose. | `10-webgl-scene-lifecycle.js`, `11-webgl-scene-graph.js`, `21-webgl-scene-asset-cache.js` | SB11 can run browser stress without validating stale scene-state leaks. |
| SB08-INV-004 | Any future shared/global asset cache must preserve ownership/ref-count boundaries and expose the same pending/settled cleanup diagnostics. | Documenting a shared cache as a performance optimization while making ownership invisible. | No previous extension rule existed in `WebGlLib/README.md`. | `source-policy-assertions.txt` and `src/CanDoItAll.Components.WebGlLib/README.md` document the rule. | `src/CanDoItAll.Components.WebGlLib/README.md` | SB09 package proof and future cache work have a stable extension contract. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| `assetCachePendingDisposalCount` | `disposeAssetCache` | `getDiagnostics`, `getProofSnapshot`, `WebGlRuntimeDiagnostics`, `WebGlSceneProofSnapshot` | Non-zero only while disposed cache promises are awaiting resolution/rejection. | Failing-first JS harness expected the field and failed before implementation. |
| `assetCacheDisposedPromiseCount` | `disposeAssetCache` | Browser/Blazor diagnostics | Settled cleanup promise count, independent of whether the template had already resolved before cache disposal. | Failing-first JS harness expected the field and failed before implementation. |
| `assetCacheDisposalErrorCount` | `disposeAssetCache` | Browser/Blazor diagnostics and browser proof assertions | Remains zero for clean high-GLB recreate/dispose cycles; increments on cleanup rejection. | Browser stress asserts zero cleanup errors. |
| `retainedSharedTextureCount` | `disposeOwnedMaterial` | Resource ownership tests and diagnostics | Increments when an owned cloned material is disposed but its shared texture is retained. | Tinted instance/template test fails if texture is disposed during instance cleanup. |

## Raw Requirement Closure

R09 is solved for SB08. Resource ownership and async asset load/dispose behavior are now covered by an enhanced Node harness, typed .NET diagnostics tests, high-GLB browser stress proof, console review, source assertions, boundary audits, and build proof.
