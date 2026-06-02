# Current-state analysis after previous bundle

## What improved

- `WebGlRunBrowserApplyAdapter.ApplyAsync(WebGlRunFrameApplyResult)` now stops before applying a command batch when `frameApplyResult.Errors` are already present.
- Scene reset failure now stops the browser apply path instead of continuing to apply a frame on a stale scene.
- `WebGlSceneViewBrowserRuntime.ImportSceneAsync` now calls `ImportSceneDocumentDetailedAsync`, so document-based scene import can carry document runtime options.
- `WebGlRunDocumentValidator` now rejects mixed direct frame commands plus staged commands.
- Economy UI no longer searches under `tests/CanDoItAll.Economy.Tests/Fixtures` at runtime; it uses `IEconomySimulationScenarioCatalog`.
- Node copies runtime sample scenarios to output/publish through `SimulationScenarios/EconomySimulationSandbox/**`.
- Components package versioning now has `CanDoItAllPackageProofSuffix`.

## What remains weak

- Economy UI still applies only `Session.CurrentRunFrame` after `Step`, `First`, `Last`, or seek-like operations. If frames are deltas, this is not deterministic replay.
- `WebGlRunDocumentRunner` validates frame execution before `FromFrame`, but it does not explicitly fail if `FromFrame` itself returns errors. This leaves a gap for future `FromFrame` validation rules.
- `WebGlRunFrameExecutionValidator` iterates `frame.Stages` in insertion order, while `WebGlRunFrameApplyResult.FromFrame` sorts by `StartsAtSeconds`, `StageIndex`, `OrderIndex`, and `StageId`. Validation and runtime can disagree.
- `IEconomySimulationScenarioCatalog` has stream methods, but its descriptor and main session service API remain path-oriented through `ExperimentJsonPath`.
- `EconomySimulationSandboxSessionExport` stores absolute and relative paths, not a portable scenario source identity plus content hash.
- `PersistSessionArtifacts` blocks on async snapshot persistence by calling `.GetAwaiter().GetResult()`.
- The generic provenance boundary currently permits all `source.*` keys and values without a schema.
- `WebGlSceneView.ImportSceneDocumentDetailedAsync` can externally change runtime scene/options without updating the component's `appliedSceneKey` / lifecycle bookkeeping.
- Several proof transcript files are empty. Empty artifacts must not be accepted as completed semantic proof.
- Large-scene performance risks remain around full payload serialization in `OnParametersSet`, replay cost, and resource/cache pressure.
