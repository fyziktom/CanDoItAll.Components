# Current-state analysis after v6

## What improved

- Components now exposes runtime stop APIs from `WebGlSceneView`.
- JS runtime has `stopRuntimeActivity` and `cancelCommandStages`.
- `RunPlayback` now uses a generation counter and cancellation token source rather than a single mutable `isPlaying` flag.
- Economy has scenario sources, scenario manifests, pack/content hashes, service registration helpers, async export methods, and readiness/performance probes.
- Economy UI now uses a scenario selector and deterministic replay through `ApplyPlaybackAsync`.
- A real-scenario readiness reporter exists and can generate machine-readable artifacts.

## What still blocks research-grade experiments

### 1. Pause correctness still needs settled-state proof

The implementation now sends a stop command to the browser runtime, but the research-grade gate is not "StopRuntimeActivityAsync was called." The gate is:

- active motions = 0
- queued motions = 0
- queued command stages = 0
- active barrier = none
- no stale `MotionCompleted` callback mutates UI after pause
- no automatic stage runner work remains after a bounded wait
- subsequent play resumes from a consistent runner state

### 2. Batch apply can mean "scheduled", not "settled"

`applyCommandBatch` schedules stages via the stage runner. That means a C# apply result can be true while browser runtime work is still in progress. This is a serious proof-quality issue for performance tests and browser replay tests. The runtime needs an explicit idle/settled contract.

### 3. Readiness report is useful but not yet an experiment validity certificate

Current readiness answers key questions, but it still reports missing browser playback actions. It also currently relies on real scenario runner outputs and strict projection validation, not a complete oracle-driven economic correctness suite.

### 4. Strict mode must be end-to-end and default for research

The simulator has warning/error options, but research mode needs a single explicit policy that elevates all infrastructure ambiguity to errors:

- unknown event kind
- unknown handler
- insufficient stock when not explicitly allowed
- missing actor/resource/store references
- ambiguous store resolution
- fallback visual object
- no-op pose/symbol mapping
- unknown metric kind
- unknown invariant kind
- unresolved visual action mapping
- warnings above zero unless explicitly allowlisted

### 5. Store resolution can hide bugs

Current store resolution may select a store implicitly from multiple candidates. In demos that is convenient; in experiments it is noise. Research mode must require an explicit resolution policy and must record why a store was selected.

### 6. Metric/invariant fallbacks risk false positives

Metrics and invariants currently infer or fall back. Research mode needs registered metric/invariant kinds, unknown-kind errors, units, decimal precision policy, and expected metadata schema validation.

### 7. Behavior expansion is a hidden economic policy

Automatic expansion of `need`, `use`, `trade`, and rule events is economic semantics. It must be versioned, declared by the scenario pack, emitted into artifacts, and included in deterministic hashes.

### 8. Performance proof is not a hard gate yet

Performance probes exist and are valuable, but some thresholds are warning-only. Research-grade runs need budgets that can mark an experiment "not publishable" or "not comparable".

## Can simulations be run now?

Yes, for exploration and engineering feedback.

Do not yet treat their outputs as strong economic evidence. Use them for scenario development and pipeline validation, but gate any interpretation with the next research-readiness layer.

## Minimum criteria for trusted experiments

A scenario can be trusted only when all of the following pass:

1. scenario pack manifest hash validation;
2. strict event/reference validation;
3. deterministic headless run;
4. golden oracle comparison or declared no-oracle exploratory status;
5. metric/invariant registry validation;
6. behavior-expansion profile declared and hashed;
7. readiness report status = `research-ready`;
8. browser visualization status = observer-only, not source of truth;
9. repeat run hash-chain equality;
10. warnings budget = zero or explicitly allowlisted.
