# SB05 Semantic Invariants

Subbundle: `SB05-resource-ownership-asset-cache`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB05-TEXTURE-001 | REQ-006 | Cloned/tinted GLB model instances own cloned material objects but do not dispose shared template texture maps unless the texture was explicitly cloned and marked owned. | Treating `ownsMaterial` as `ownsTexture`, or incrementing a retention counter while still disposing texture maps. | `bundle://proof/SB05/transcripts/failing-first-resource-ownership.json` records `sharedTextureDisposesWhenInstanceDisposed=1` before the fix. | `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` and `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` record retained shared textures with `disposedTextureCount` unchanged while one tinted instance is removed. |
| SB05-TEMPLATE-001 | REQ-006, REQ-007 | State-local cached GLB templates own their geometry, material, and texture disposal when the scene state/cache is disposed. | Incrementing `disposedTemplateCount` while leaving template geometry/material/texture undisposed. | `bundle://proof/SB05/transcripts/failing-first-resource-ownership.json` records template disposal count 1 but zero template geometry/material/texture disposals. | `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` records template geometry/material/texture disposal; browser proof records final `disposedTemplateCount=1` and texture disposal increasing from 0 to 2 at scene dispose. |
| SB05-CACHE-001 | REQ-007 | Asset cache remains state-local and deterministic across repeated model/primitive/high profile imports: same URI produces one miss and subsequent hits, and profile switching does not create unexpected missing assets. | A global cache without ownership proof, clearing cache on every profile switch, or hiding missing assets as silent primitive fallback. | Failing-first proof shows the previous cache disposal path did not actually release template resources, making cache diagnostics shallow. | Browser proof records `assetCacheMode=state-local`, one cache entry, `assetCacheMissCount=1`, increasing hit count after profile switches, and no unexpected missing assets until the explicit `asset.missing.intentional.sb05` patch. |
| SB05-DIAG-001 | REQ-006, REQ-007, REQ-015 | JS diagnostics, command results, C# runtime diagnostics, and proof snapshots expose cache mode plus retained shared texture count. | Browser-only counters that C# interop cannot deserialize, or command results that omit the ownership counters. | Failing-first proof shows no ownership split and no retained shared texture counter. | Focused diagnostics tests pass; source assertions cite JS/C# fields and browser command results include `assetCacheMode`, `retainedSharedTextureCount`, and `disposedTextureCount`. |
| SB05-BOUNDARY-001 | REQ-001, REQ-015 | Resource ownership and cache behavior stay inside generic `WebGlLib`; no run-layer, Economy, ledger, market, production-line, Vernon, or Smith concepts enter production code. | Special-casing the sandbox route, proof object ids, or a domain asset in production disposal/cache code. | `bundle://proof/SB05/transcripts/sb05-anti-stub-and-boundary-scan.txt` would surface forbidden production-code terms or placeholders. | The scan passes; browser proof uses public generic scene import/patch/dispose APIs and a repository GLB asset only as data. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Texture ownership flag | `17-webgl-scene-resources.js` | Model instance disposal, template cache disposal, diagnostics | Object trees and material clones carry separate `ownsGeometry`, `ownsMaterial`, and `ownsTexture` semantics. | Failing-first proof records shared texture disposal during instance removal. |
| Retained shared texture counter | `retainMaterialTextures` in `17-webgl-scene-resources.js` | Runtime diagnostics, command results, C# DTO, browser proof | Increments when a material clone is disposed while retaining shared texture maps. | Positive browser proof requires the counter to increase while `disposedTextureCount` remains unchanged. |
| State-local asset cache mode | `21-webgl-scene-asset-cache.js` | Runtime diagnostics, C# DTO, proof snapshots, browser proof | Reported as `state-local`; cache entries are per scene state and disposed when the state is disposed. | Browser proof rejects cache drift by requiring one miss and subsequent hits across profile switches. |
| Template resource disposal | `disposeAssetCache` plus `disposeSceneObjectTree` | Runtime diagnostics, final dispose proof | Cache disposal passes diagnostics into template tree disposal and releases template geometry/material/textures once. | Failing-first proof records `disposedTemplateCount=1` with zero resource disposals. |
| Textured browser proof transcript/screenshot | `/tycoon-village` route plus public scene import/patch/dispose APIs | Bundle progression gate and later SB13 memory proof | Imports two `question_box.glb` instances, removes one, switches profiles, applies intentional missing asset, and disposes state/cache. | Browser proof includes explicit assertions for no unexpected missing assets and final template texture disposal. |

## Reopen Triggers

- `disposedTextureCount` increments while removing only one tinted GLB instance that shares template textures.
- Cached templates stop disposing geometry, material, or texture resources at scene/cache disposal.
- `assetCacheMode`, `retainedSharedTextureCount`, or texture disposal counters disappear from JS or C# diagnostics.
- Profile switching creates unexpected missing asset diagnostics before an explicit bad asset id is supplied.
- Production `WebGlLib` resource/cache code gains WebGlRunLib or domain-specific terms.
