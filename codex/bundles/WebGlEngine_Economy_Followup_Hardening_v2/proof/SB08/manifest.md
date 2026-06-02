# SB08 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

Pre-SB08 hashes were not captured because the worktree was already intentionally dirty from SB01-SB07. The after-hash transcript is `proof/SB08/transcripts/changed-file-after-hashes.txt`.

| File | SHA-256 after | Reason |
| --- | --- | --- |
| `tools/webgllib/test-resource-ownership.mjs` | `0e26198d0f132135de9e2b886679c881f7cd04a3899936c304eefa8e0c07db0c` | Enhanced resource ownership harness with pending-template promise disposal diagnostics and template/instance ownership separation. |
| `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js` | `3da41645c9684d0b920aad4b72ecbb8306df64d663f47a249ae390c0c8bd595c` | Added pending disposal, settled promise, and disposal error counters for async cache cleanup. |
| `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/25-webgl-scene-diagnostics.js` | `0dca685962580b29c8898e370d96a4cd04250a7a3df9ea531210e61691228fff` | Initialized new cache lifecycle diagnostic counters. |
| `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | `eb58918c3388e7f11fb0640c98da153d5f0b69769afae34c1d039f139b756a49` | Exposed cache lifecycle counters through `getDiagnostics`. |
| `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `e799ced355efc3b88024972be0fc00f3993645220c8c7d794bc3f28e0bbf6d67` | Exposed cache lifecycle and disposal counters in proof snapshots. |
| `src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` | `369e443b171d4710723a1dc57f52870513e154edefec47f0ced45d0a42c0fd65` | Added typed Blazor diagnostics for new cache lifecycle counters. |
| `src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | `cd11d091baf9f39e588c578ce2ef461f9826f255bdfc9ff7e276f0f25d361788` | Added proof snapshot cache/disposal counters for browser artifacts. |
| `tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs` | `784a526dc0e213698ca8c901612404fd154902109f62ec6f869b9101fad3d94c` | Added DTO round-trip assertions for cache lifecycle fields. |
| `src/CanDoItAll.Components.WebGlLib/README.md` | `85316a5846cc8bf4159e725dea6d248bacf4298be28839b553d3dc7170e28473` | Documented cache diagnostics and future global/shared cache ownership rule. |

## Command transcripts

| Command | Transcript | Result |
| --- | --- | --- |
| `npm run webgllib:test-resource-ownership` before production fix | `proof/SB08/transcripts/failing-first-resource-ownership.txt` | Failed as expected: pending-template and cache-disposal diagnostic fields were absent. |
| `npm run webgllib:test-resource-ownership` | `proof/SB08/transcripts/passing-resource-ownership-final.txt` | Passed; pending disposal scheduled/drained, template disposal counted, tinted instance retains shared texture. |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore` | `proof/SB08/transcripts/components-webgllib-tests.txt` | Passed 48 tests. |
| `npm run webgllib:audit-scene-runtime-imports` | `proof/SB08/transcripts/components-webgllib-scene-runtime-imports-audit.txt` | Passed for 37 modules. |
| `npm run webgllib:audit-scene-runtime` | `proof/SB08/transcripts/components-webgllib-scene-runtime-audit-final.txt` | Passed with existing line-count warnings; no hard failures. |
| `npm run webgllib:audit-boundary` | `proof/SB08/transcripts/components-webgllib-boundary-audit.txt` | Passed; WebGlLib remains generic. |
| `dotnet build CanDoItAll.Components.slnx --no-restore` | `proof/SB08/transcripts/components-build.txt` | Passed with 0 warnings and 0 errors. |
| Anti-stub/source-placeholder scan | `proof/SB08/transcripts/anti-stub-placeholder-scan.txt` | Passed; no fixture/stub/proof-only placeholder matches in SB08 paths. |
| Source assertions scan | `proof/SB08/transcripts/source-policy-assertions.txt` | Passed; new counters/tests/docs are present in production and test paths. |
| `git diff --check` | `proof/SB08/transcripts/git-diff-check.txt` | Only repository LF-to-CRLF warnings; no whitespace error findings were introduced. |

## Browser proof

| Route | Viewport | Actions | Screenshot | Console | Result |
| --- | --- | --- | --- | --- | --- |
| `/tycoon-village` | 1600x900 | Clicked High GLB, waited for model load, ran six direct `CanDoItAll.webglScene.create`/`dispose` cycles against the page host, waited for pending disposal promises to drain, recreated final high-GLB state. | `proof/SB08/browser/tycoon-village-high-glb-stress.png` | `proof/SB08/browser/high-glb-console-warnings.txt`, `proof/SB08/browser/high-glb-console-errors.txt` | Pass; `high-glb-stress-diagnostics.json` assertions all true. Console had 0 errors and expected GLTF extension warnings only. |
| `/tycoon-village` | 1600x900 | Reloaded normal UI path, clicked High GLB, clicked Snapshot. | `proof/SB08/browser/tycoon-village-high-glb-ui-proof.png` | `proof/SB08/browser/high-glb-ui-console-warnings.txt`, `proof/SB08/browser/high-glb-ui-console-errors.txt` | Pass; UI proof reported 8 loaded assets, 17 model instances, 0 missing assets, and pending disposal count 0. |

Browser data artifacts:

- `proof/SB08/browser/high-glb-stress-diagnostics.json`
- `proof/SB08/browser/high-glb-ui-diagnostics.json`
- `proof/SB08/browser/initial-dom-inventory.json`
- `proof/SB08/browser/tycoon-village-initial-snapshot.md`

## Source assertions

- `21-webgl-scene-asset-cache.js` now tracks `pendingDisposalCount`, `disposedPromiseCount`, and `disposalErrorCount`, and mirrors them into runtime diagnostics.
- `02-webgl-scene-core.js` and `08-webgl-scene-proof.js` expose the counters through browser diagnostics and proof snapshots.
- `WebGlRuntimeDiagnostics` and `WebGlSceneProofSnapshot` deserialize the same fields for Blazor/.NET consumers.
- `test-resource-ownership.mjs` covers pending promise disposal, owned tinted material disposal, shared texture retention, and template disposal.
- `WebGlLib/README.md` documents the future shared/global cache extension rule.

## Anti-stub audit

`proof/SB08/transcripts/anti-stub-placeholder-scan.txt` reports no `TODO`, `NotImplemented`, placeholder, proof-only, fixture, or stub matches in the SB08 production/test resource paths.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `assetCachePendingDisposalCount` | `disposeAssetCache` | Browser diagnostics, proof snapshot, Blazor DTO | Increments when cache entries are cleared for async disposal; drains in `finally` after each promise settles. | Failing-first transcript shows the field was absent before the fix. |
| `assetCacheDisposedPromiseCount` | `disposeAssetCache` | Browser diagnostics, proof snapshot, Blazor DTO | Increments once per settled cache cleanup promise. | Failing-first transcript expected it and failed before production wiring. |
| `assetCacheDisposalErrorCount` | `disposeAssetCache` | Browser diagnostics, proof snapshot, Blazor DTO | Remains zero for clean pending GLB cleanup; increments if a disposed pending promise rejects. | Browser high-GLB stress proves zero cleanup errors over repeated cycles. |
| Tinted instance material ownership | `markOwnedMaterial` and `disposeSceneObjectTree` | Model instance lifecycle | Disposes owned cloned material while retaining shared template texture until template cleanup. | Enhanced JS test proves instance dispose does not dispose the shared texture. |

## Gate decision

Pass. SB08 objective is implemented; failing-first, passing tests, browser stress proof, source assertions, anti-stub scan, audits, build, and semantic invariants agree. No downstream phases were reopened.
