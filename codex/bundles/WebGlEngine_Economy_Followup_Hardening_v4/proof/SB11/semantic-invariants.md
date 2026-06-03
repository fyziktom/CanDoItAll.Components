# SB11 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.
- Large scenes must not require full scene/options JSON serialization on every `OnParametersSet` call; compact lifecycle keys must use revisions/runtime keys and stable counts.
- Runtime budgets must be generic WebGlLib contracts for 100, 500, and 1000+ object scenes and staged motion replay.
- Diagnostics proof must include rebuild, patch, batch duration, resource disposal, asset cache, queued motion, and queued stage counters.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| Large-scene compact lifecycle key | `WebGlSceneView.CreateSceneLifecycleKey` | Blazor render lifecycle and external import guard | Scenes with at least 100 objects or links match by compact revision/runtime metadata; small scenes keep the existing full-payload key | `transcripts/failing-first.txt`, `transcripts/passing-tests.txt` |
| Revision/runtime-key update contract | `WebGlSceneModel.Revision`, `WebGlSceneUiState.Revision`, `WebGlRuntimeOptions.RuntimeKey` | Large-scene callers and `WebGlSceneView.OnParametersSet` | In-place large-scene data changes do not trigger runtime updates until a caller bumps revisions or changes runtime key/options | `transcripts/passing-tests.txt`, `transcripts/source-assertions.txt` |
| Budget profiles | `WebGlRuntimeBudgetProfiles` | WebGlLib callers, sandbox `PerformanceProof`, diagnostics DTO tests | Scene100, Scene500, and Scene1000Plus provide bounded object, asset, motion, stage, and triangle budgets | `transcripts/passing-tests.txt`, `browser/performance-proof-diagnostics.json` |
| Browser runtime performance diagnostics | `/performance-proof` through `WebGlSceneView` public APIs | Browser diagnostics JSON and screenshot | Live browser applies the 202-command primitive batch, refreshes diagnostics, and displays the full scene and metrics panel at 1280x800 | `transcripts/browser-proof.txt`, `browser/performance-proof-browser.png` |
| Resource ownership and disposal | JS resource/cache modules | Runtime diagnostics and resource ownership audit | Tinted instances retain shared textures, owned instance resources dispose separately, duplicate disposals dedupe, and pending templates dispose after promise resolution | `transcripts/benchmark-resource-proof.txt` |
| Practical GLB diagnostics | `tools/webgllib/export-model-diagnostics.cjs` | Model diagnostics JSON/markdown artifacts | Repository GLB/GLTF metadata is parsed for model-high/model-medium/model-low assets without domain coupling | `transcripts/model-diagnostics-proof.txt`, `changed-file-hashes.md` |
| WebGlLib-only sample non-regression | Sample project and README | Package-mode/project-reference proof runs | Project-reference restore/build remains valid and package-mode restore metadata is documented as mode-specific | `transcripts/sample-nonregression.txt` |

## Completion assertions

- `Large_scene_parameter_lifecycle_uses_revision_and_runtime_key_instead_of_full_payload` passes and suppresses stale large-scene updates until revision changes.
- `Runtime_budget_profiles_define_100_500_and_1000_plus_scene_budgets` passes with exact budget values.
- Runtime diagnostics round-trip batch duration, full rebuild, transform-only patch, asset cache, disposal, queued motion, max queue, and queued stage counters.
- `Batch_normalizer_handles_1000_patch_motion_items_with_deterministic_metrics` passes the 2000 ms CPU budget while preserving deterministic normalization metrics.
- Browser proof records 100 objects, 202 commands, 1 stage, 100 coalesced patches, 100 dropped motions, and all `scene-100` budget assertions true.
- Resource ownership proof reports overall `"pass": true`.
- WebGlLib-only sample restore/build succeeds.
- Boundary audit passes and no Economy implementation dependency is introduced.
