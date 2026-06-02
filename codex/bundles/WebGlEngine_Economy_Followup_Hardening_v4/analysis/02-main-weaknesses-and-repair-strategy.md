# Main weaknesses and repair strategy

## W01 Replay is not yet deterministic enough

`WebGlRunDocumentRunner` is the right abstraction for reset/seek/replay, but `EconomySimulationSandboxPage` bypasses it and applies `WebGlRunFrameApplyResult.FromFrame(Session.CurrentRunFrame)` directly. This is only safe if every frame is absolute. Current frame/stage/patch naming strongly suggests delta-style playback is intended.

Repair: move browser application in Economy UI to a `WebGlRunDocumentRunner` instance with a browser frame applier. Step, First, Last, and future seek operations should ask the runner/controller for the frames to apply and apply the required sequence.

## W02 Runtime validation and application ordering are not the same

`WebGlRunFrameExecutionValidator` validates stages in stored order. `FromFrame` applies stages in sorted playback order. A frame with out-of-order add-object/move-object stages can pass/fail differently from how it runs.

Repair: create one shared stage ordering policy and use it in `WebGlRunFrameExecutionValidator`, `WebGlRunFrameApplyResult`, `EconomyWebGlRunValidator`, and any browser proof helpers.

## W03 Scenario/session APIs remain path-centric

`IEconomySimulationScenarioCatalog` has `OpenExperiment`, but descriptors and services still expose/consume `ExperimentJsonPath`. This makes tests, package consumers, Node, Docker, IPFS, embedded scenarios, and future cloud scenario sources harder.

Repair: add `EconomySimulationScenarioSource` or equivalent source abstraction with scenario id, source kind, experiment stream, companion resolver, content root identity, and deterministic source hash. Keep path APIs as legacy adapter methods only.

## W04 Portable session export/import is incomplete

`EconomySimulationSandboxSessionExport` currently stores `ExperimentJsonPath`, `BaseDirectory`, `RelativeExperimentPath`, and `SnapshotDirectory`. This couples export to the machine layout.

Repair: export scenario id, scenario source kind, input pack hash, scenario pack manifest hash, snapshot ids/hashes, and optionally an embedded/minimal scenario pack. Import must validate the scenario source by hash before restoring.

## W05 Async boundaries are not clean

Snapshot save/load is async at the store layer, but session service uses synchronous APIs and blocks on async persistence.

Repair: introduce async first methods (`LoadAsync`, `ProjectAsync`, `ExportSessionAsync`, `ImportSessionAsync`, `SnapshotAsync`) and keep sync wrappers only for tests/simple hosts. Avoid `.GetAwaiter().GetResult()` inside library logic.

## W06 Provenance is too broad

`source.*` is skipped wholesale by generic validators. This avoids false domain-leak errors but allows arbitrary domain-laden metadata into generic packages without shape guarantees.

Repair: define a typed/allowlisted provenance model. Generic Components can treat it as opaque but should validate allowed keys, size limits, casing, and required fields. Economy bridge can perform domain-specific validation outside Components.

## W07 Proof quality is inconsistent

The completed bundle reports success but many transcript files are empty. Some empty transcripts may be harmless, but critical proof must include command output, assertions, or a structured proof JSON.

Repair: add proof-integrity validator that fails completed bundles if required transcript files are empty, screenshots lack matching assertion JSON, or source assertions are not tied to file hashes.

## W08 Performance budgets are not contractual

There are stress proofs, but there is no durable budget for object counts, stage counts, replay durations, render rebuild counts, or memory/resource counters.

Repair: add benchmark-like tests and browser proof assertions for representative scenarios: 100, 500, 1000+ objects; many staged motions; seek-to-last replay; high GLB cache; repeated recreate/dispose.
