# SB03 Semantic Invariants

Subbundle: `SB03-patch-transactions-and-revisions`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB03-TXN-001 | REQ-003 | A patch that fails preflight validation must leave object state, link state, layer membership, affected-id lists, and scene/UI revisions unchanged. | Returning `success=false` after already applying earlier object mutations. | `bundle://proof/SB03/transcripts/failing-first-browser-bad-link-partial-commit.json` records a failed bad-link patch that still moved `agent.runner` and advanced revision before the fix; `bundle://proof/SB03/transcripts/failing-first-dotnet-patch-document-revision.txt` records reducer failures before production changes. | `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json` records `badHashUnchanged=true`, no runner movement, no revision change, and empty affected ids for the same failure mode. |
| SB03-REV-001 | REQ-004 | `WebGlSceneModel.Revision` is canonical. Successful mutating patches advance it exactly once and mirror the same value to `UiState.Revision`; command/export results report the canonical value. | Continuing to increment only `UiState.Revision`, or using whichever revision happens to be larger. | Failing-first dotnet transcript shows canonical revision tests failing with wrong result revisions. | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` passes `Apply_uses_scene_revision_as_canonical_and_mirrors_ui_revision`; browser proof shows positive patch revision and UI revision both advance from 0 to 1. |
| SB03-HASH-001 | REQ-004, REQ-008 | Scene content hashing includes `Scene.Revision` and ignores UI-only revision, hover, and selection state; full document hashing still reflects included UI state. | Hashing `UiState.Revision` as scene content, or ignoring canonical scene revision. | Failing-first dotnet transcript shows `Scene_content_hash_uses_scene_revision_and_ignores_ui_revision` failing before the hasher change. | Passing dotnet transcript covers `Scene_content_hash_uses_scene_revision_and_ignores_ui_revision` and existing hover/selection hash tests. |
| SB03-CONSISTENCY-001 | REQ-003, REQ-008 | Removing an object removes links that reference it, removes the object id from layers, records affected link ids, and advances revision once. | Removing only the object while leaving dangling links or stale layer membership. | Failing-first dotnet transcript shows the remove/layer/revision test failing before reducer changes. | Passing dotnet transcript covers `Apply_remove_object_cleans_links_layers_and_increments_revision_once`. |
| SB03-PARITY-001 | REQ-003, REQ-004 | C# and JavaScript patch paths apply the same preflight rules for scene id, strict base revision, object patch availability after add/remove resolution, and missing link endpoints. | Fixing only C# tests while the browser runtime can still partially mutate a failed patch. | Browser failing-first transcript catches JS partial mutation; dotnet failing-first transcript catches C# reducer/hash failures. | Passing dotnet, browser, import-audit, runtime-audit, and sandbox-build transcripts together cover the C# and JS paths. |
| SB03-BOUNDARY-001 | REQ-001, REQ-015 | SB03 keeps `WebGlLib` generic and does not introduce run-layer or Economy/domain semantics. | Adding domain-specific branch logic to make the tycoon-village proof pass. | `bundle://proof/SB03/transcripts/sb03-anti-stub-and-boundary-scan.txt` would surface forbidden domain/run-layer terms in touched production files. | The same scan passes; ADR places only generic scene mutation and hash policy in `WebGlLib`. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Canonical revision helper | `WebGlSceneRevisionPolicy.cs`, `34-webgl-scene-revisions.js` | C# reducer, document normalizer, JS patch/result/export runtime | Resolve legacy payloads, compute next revision, and commit successful patch revisions. | Failing-first dotnet transcript shows wrong revision behavior before helper usage. |
| Patch preflight validation | `WebGlScenePatchReducer.Validate`, `validatePatchForApply` | `Apply` and `applyPatchDetailed` | Validates whole patch before mutating state for structural failure modes. | Browser failing-first transcript shows the exact shallow failure: failed result after mutation. |
| Link/layer cleanup on object removal | C# reducer and JS patch runtime | Scene model, runtime link groups, layers, diagnostics/proof readers | Removing an object also removes dependent links and layer membership. | Failing-first dotnet transcript shows cleanup/revision expectations failing before the reducer change. |
| Scene document hash policy | `WebGlSceneDocumentHasher`, `WebGlSceneDocumentNormalizer` | Save/load contract and consumers comparing scene content | Normalizes documents, hashes scene content separately from full saved document state. | Failing-first dotnet transcript shows UI revision affecting scene content hash before the fix. |
| Browser transaction proof | `/tycoon-village` sandbox route | Bundle progression gate and later SB13 comparison | Executes bad and good patches through public runtime APIs with diagnostics and screenshot. | Bad-link negative patch must fail with unchanged snapshot and no affected ids. |

## Reopen Triggers

- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Patch|Document|Revision"` fails.
- Browser proof shows a failed patch mutating object positions, link count, layer state, affected ids, or revision.
- A patch result reports `UiState.Revision` instead of canonical `Scene.Revision`.
- Scene content hash changes when only UI revision, hover, or selection changes.
- Touched `WebGlLib` patch/revision files gain run-layer or Economy/domain concepts.
