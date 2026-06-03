# SB11 proof manifest

Status: completed

## Semantic assertion

Large WebGlLib scenes now have explicit generic runtime budgets and avoid using full scene/options JSON as the Blazor parameter lifecycle key. Scenes with at least 100 objects or 100 links use compact scene id, revision, count, catalog, and runtime-key data; callers opt into deterministic large-scene updates by bumping scene/UI revisions and changing `WebGlRuntimeOptions.RuntimeKey` when runtime configuration changes. Browser and JS audit proof show primitive runtime stress, staged motion replay, resource ownership/disposal, and practical GLB diagnostics without introducing Economy semantics into Components.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-tests.txt`
- `transcripts/sample-nonregression.txt`
- `transcripts/benchmark-resource-proof.txt`
- `transcripts/model-diagnostics-proof.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt`
- `transcripts/browser-proof.txt`
- `transcripts/validator-audits.txt`
- `changed-file-hashes.md`
- `browser/performance-proof-diagnostics.json`
- `browser/performance-proof-browser.png`

## Results

- Failing-first proof: `transcripts/failing-first.txt` records the pre-fix 1000-object lifecycle test issuing an unwanted `CanDoItAll.webglScene.update` when a large scene object changed without a revision bump.
- Passing tests: `transcripts/passing-tests.txt` records 21 focused WebGlLib tests passing, including large-scene compact lifecycle, runtime budget diagnostics, and a 1000 patch plus 1000 motion CPU budget assertion.
- Benchmark/resource proof: `transcripts/benchmark-resource-proof.txt` records `webgllib:audit-sharedwell-performance` and `webgllib:test-resource-ownership` passing. The performance artifact records 1000 motions in 4.77 ms, 1000 indexed-link sync in 49.885 ms, and 500 staged commands with a bounded 200-entry journal.
- Browser proof: `transcripts/browser-proof.txt`, `browser/performance-proof-diagnostics.json`, and `browser/performance-proof-browser.png` record a 1280x800 in-app browser run of `/performance-proof`: 100 objects, 202 commands, 1 stage, 100 coalesced patches, 100 dropped duplicate motions, and all `scene-100` budget assertions true.
- Resource ownership/disposal proof: `transcripts/benchmark-resource-proof.txt` records retained shared texture, template/instance ownership separation, deduped disposal, pending promise disposal, and template cache disposal all passing with no disposal errors.
- High-GLB practical proof: `transcripts/model-diagnostics-proof.txt` records `webgllib:model-diagnostics` passing for 43 model assets; `artifacts/webgl-engine-prep-v4/model-diagnostics.md` shows model-high/model-medium/model-low load diagnostics with no load failures.
- WebGlLib-only sample proof: `transcripts/sample-nonregression.txt` records restore plus `dotnet build --no-restore` passing for `samples/CanDoItAll.Components.WebGlLibOnlyViewer`.
- Source assertions: `transcripts/source-assertions.txt` proves the compact lifecycle key, public budget profiles, diagnostics counter tests, sandbox budget wiring, and sample restore-mode documentation.
- Boundary audit: `transcripts/boundary-audit.txt` records `webgllib:audit-boundary` passing and no forbidden Economy coupling in scoped WebGlLib implementation/test files.
- Validator audits: `transcripts/validator-audits.txt` records the prepared-stage bundle validator and proof-integrity audit passing after SB11 proof was added.
- Changed hashes: `changed-file-hashes.md` records hashes for SB11 source, test, docs, generated proof, browser screenshot, and transcripts.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| Large-scene compact lifecycle key | `WebGlSceneView.CreateSceneLifecycleKey` | `OnParametersSet`, `OnAfterRenderAsync`, external import lifecycle | Scenes with at least 100 objects or links use scene id, scene/UI revisions, object/link/layer counts, asset catalog id/version, and runtime key/options instead of full scene serialization | `transcripts/failing-first.txt`, `transcripts/passing-tests.txt`, `transcripts/source-assertions.txt` |
| Public generic budget profiles | `WebGlRuntimeBudgetProfiles.Scene100`, `Scene500`, `Scene1000Plus` | WebGlLib callers and sandbox `PerformanceProof` | Provides generic object, asset-cache, active/queued motion, queued-stage, and triangle budgets for staged-motion replay | `transcripts/passing-tests.txt`, `transcripts/source-assertions.txt`, `browser/performance-proof-diagnostics.json` |
| Runtime diagnostics budget counters | WebGL runtime diagnostics and `WebGlRuntimeDiagnostics` DTO | Sandbox metrics panel, C# tests, browser proof JSON | Full scene rebuild, transform-only patch, batch duration, asset cache, disposal, queued motion, max motion queue, and queued command stage counters round-trip and render into proof metrics | `transcripts/passing-tests.txt`, `transcripts/browser-proof.txt` |
| Resource ownership/disposal | WebGlLib JS resource/cache modules | Runtime diagnostics, resource ownership audit | Shared template resources are retained for tinted instances, instance resources dispose separately, duplicates dedupe, and pending promises dispose after resolution | `transcripts/benchmark-resource-proof.txt` |
| WebGlLib-only sample mode guidance | Sample README | Package-mode and project-reference sample proof runs | Project-reference restore is refreshed after package-mode proof so static web asset metadata does not mix package and project assets | `transcripts/sample-nonregression.txt`, `transcripts/source-assertions.txt` |

## Refactor Gate

- Changed Components files: `WebGlSceneView.razor`, `WebGlRuntimeBudgetProfiles.cs`, `src/CanDoItAll.Components.WebGlLib/README.md`, `PerformanceProof.razor.cs`, `samples/CanDoItAll.Components.WebGlLibOnlyViewer/README.md`, and WebGlLib test files.
- Changed Economy files: none.
- Public API changed: additive `WebGlRuntimeBudgetProfiles` helper class. No existing signatures changed. Large-scene callers should bump `WebGlSceneModel.Revision` or `WebGlSceneUiState.Revision` for content changes and set `WebGlRuntimeOptions.RuntimeKey` for runtime option changes.
- Test/build/audit commands: see `transcripts/failing-first.txt`, `transcripts/passing-tests.txt`, `transcripts/sample-nonregression.txt`, `transcripts/benchmark-resource-proof.txt`, `transcripts/model-diagnostics-proof.txt`, `transcripts/source-assertions.txt`, `transcripts/boundary-audit.txt`, `transcripts/browser-proof.txt`, and `transcripts/validator-audits.txt`.
- Proof artifact paths: this manifest, `semantic-invariants.md`, `changed-file-hashes.md`, `transcripts/`, `browser/performance-proof-diagnostics.json`, and `browser/performance-proof-browser.png`.
- Open risks: no known SB11 blocker. High-GLB proof is a practical model diagnostics run rather than a separate high-GLB browser stress route; primitive browser stress covers live rendering and diagnostics.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
