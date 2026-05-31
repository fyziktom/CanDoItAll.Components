# Current Review Findings

## Components repo

Positive findings:

- `CanDoItAll.Components.WebGlRunLib` is part of the solution and references only the generic `WebGlLib`.
- `WebGlSceneCommandBatch` now supports `OrderingMode`, `BatchingPolicy`, `Stages`, and metrics such as command count, coalesced patch count, duplicate motion handling, and interop estimates.
- The JS command batch layer delegates normalization to `28-webgl-scene-command-batch-normalizer.js`, which improves maintainability over the earlier single-function approach.
- The WebGL runtime has a large-screen policy and audit script that checks unsafe JS patterns, import graph, domain neutrality, branch instructions, and small/medium/mobile drift.

Weaknesses / risks:

- JS `applyCommandBatch` applies all normalized top-level patches/motions and then all stage patches/motions immediately. `waitSeconds` is currently only metadata and is not an execution barrier.
- Motion runtime still stores active motions in a `Map` keyed by `motionId`. `queueMode=append` prevents deletion of existing motions for the same object, but multiple active motions on the same object can fight each other frame-by-frame.
- `WebGlRunActionPlanner` flattens `Sequence` steps into one plan with patches and motions, but it does not appear to emit explicit stage objects for each step. This means semantic order can be lost when the plan becomes a batch.
- C# and JS batch normalizers are similar but duplicated. This is acceptable for now, but parity tests must become stronger and fixture-driven.
- The bridge from generic run actions to runtime command batches is still incomplete: an action plan should become a staged command batch with deterministic stage IDs, not just flat `Patches` and `Motions`.

## Economy repo

Positive findings:

- Experiment input pack contracts exist.
- Placement and parameter documents exist.
- Random placement is handled as a deterministic pre-run generator that writes JSON output.
- Scenario definition normalization now resolves aliases between `Actors/Entities`, `Locations/Places`, `Stores/InitialStores`, and `ScheduledEvents/EventTemplates`.
- Event normalization supports typed refs and a canonical event kind registry.
- A simple state transition engine exists with indexed store lookups.
- Shared-well and farmer-land probes exist.

Weaknesses / risks:

- Input pack fixture hashes are placeholders (`sha256:scenario`, `sha256:placement`, etc.) rather than real SHA-256 hashes. This weakens the experiment determinism story unless strict fixtures exist elsewhere.
- `SimulationExperimentInputPackValidator` validates hash format loosely. It should require `sha256:` + 64 lowercase hex characters for strict mode.
- Core transition handling is still strongly centered around resource transfer/use/trade/tax/rule examples. It is better than before, but should move toward a pluggable event handler registry with isolated handlers.
- `SimulationScenarioPolicies.cs` mixes distance, capacity, surplus/shortage, trade, fee/tax/admin burden, and metric/invariant policy logic in one broad file. This is maintainability risk.
- Scenario definitions are serializable but need a full experiment pack loader that loads all referenced JSON documents, validates hashes, applies placement/parameters/rules, compiles events, materializes frames, evaluates metrics/invariants, and returns a traceable run result.
- There is no typed contract yet for the simulation-to-visualization bridge output that can be consumed by `WebGlRunLib` without Economy depending directly on WebGL unless a dedicated adapter project is introduced.
