# Current-state analysis after v5

## Executive verdict

The v5 implementation moved the system in the right direction. The explicit pause bug is addressed at the surface level: the Blazor playback loop now has generation-based cancellation, the stop path calls `WebGlSceneView.StopRuntimeActivityAsync`, and the JS runtime exposes a stop operation that cancels command-stage work and clears active/queued motions.

However, the system is **not yet ready for serious economic conclusions**. It is ready for exploratory scenario authoring, UI proof, visualization proof, and simulator plumbing tests. It is not yet safe to treat observed economic outcomes as model evidence, because several non-model sources of noise can still affect results:

- runtime replay/visualization timing,
- warning-level semantic errors,
- heuristic store resolution,
- implicit behavior expansion,
- weak metric/invariant semantics for unknown metric kinds,
- missing oracle/golden scenario coverage,
- insufficient separation of economic validity from visualization validity.

The next hardening pass must therefore shift focus from "does the UI/run layer work?" to "can a failed experiment be trusted as a model failure rather than simulator noise?"

## What improved in v5

1. Playback stop is no longer only a C# boolean.
2. The JS runtime now has a public stop/cancel surface.
3. Economy scenario packs now have manifests and runtime scenario descriptors.
4. Economy sandbox can use scenario sources rather than directly searching test fixtures.
5. Session export/import is closer to portable pack semantics.
6. Stage ordering parity has improved.
7. Performance probes cover larger headless and WebGL-projection sizes.

## Remaining critical weaknesses

### W01 — Runtime idle is not a scientific barrier

`applyCommandBatch` can queue command stages and return before all barriers and motions have completed. A C# apply success can therefore mean "commands were accepted" rather than "the visual runtime reached the intended frame state."

For UI animation this can be acceptable. For experiment proof it is not enough.

Required fix: add a runtime idle/settled-state await protocol with explicit timeout, diagnostics and browser proof.

### W02 — Pause is fixed as an interaction, not yet as an invariant

`StopRuntimeActivityAsync` cancels stages and clears motions. But the system still needs a proof contract:

- active motions after pause: 0
- queued motions after pause: 0
- queued stages after pause: 0
- active barrier after pause: none
- no delayed `MotionCompleted` callback mutates status after pause
- repeated Play/Pause/Play does not resume stale generation

Required fix: add a browser-level pause-stress proof, not just unit tests.

### W03 — Economy replay is correct-looking but can be expensive and misleading

The Economy sandbox constructs deterministic replay by reapplying all run frames up to the selected frame. That is valid for seek correctness, but can become O(n²) for interactive stepping and can blur whether an error belongs to the model, the projection layer, or the replay path.

Required fix: split replay modes:
- `absolute-replay` for seek/proof,
- `incremental-apply` for normal step/play,
- `snapshot-anchor-replay` for large timelines.

### W04 — Strict economic mode is not yet the default

The SimpleAccounts engine can treat unknown events and insufficient stock as warnings depending on options. That is useful for demos, but dangerous for experiments: a scenario can "finish" while silently rejecting flows or applying partial effects.

Required fix: introduce `SimulationExperimentMode.Strict` where warnings in semantic categories fail the run unless explicitly allowed.

### W05 — Store resolution contains hidden policy

Duplicate actor/resource stores are collapsed to the first store, and shared store resolution falls back through several heuristics. This is the largest economics-noise risk in the current model. An experiment may fail or pass because the simulator chose a store implicitly, not because the economic design was good.

Required fix: require explicit store resolution policy and emit hard errors for ambiguous stores under strict mode.

### W06 — Behavior expansion injects economics

The behavior expander automatically turns events like need/use/trade into multi-step sequences. This is a domain policy, not a neutral simulator primitive. It must be explicit in the scenario profile and included in provenance.

Required fix: make expansion profiles explicit, versioned and auditable.

### W07 — Metrics and invariants can silently lie

Unknown metric kinds and missing metric IDs can produce zero-like results. Unknown invariant kinds fall back to metric thresholds. For experiments, unknown metric/invariant semantics must be hard errors.

Required fix: typed metric/invariant registry with strict validation before run.

### W08 — No golden oracle suite yet

There are tests and performance probes, but not enough "known answer" economics scenarios where expected frame-by-frame resource stores, flows, issues and metrics are asserted.

Required fix: add a golden oracle suite.

### W09 — Visualization validity is still mixed with economic validity

A failed WebGL apply should not automatically mean the economic model failed. Conversely, a successful WebGL proof should not imply economic correctness.

Required fix: produce separate validity bands: scenario, simulation, projection, runtime, UI.

### W10 — Performance thresholds are still partly observational

Current performance proof records many useful values, but warning-only thresholds are not a quality gate. For experiments, resource budgets need hard pass/fail thresholds on deterministic paths, and visual/browser budgets should be clearly non-model gates.

Required fix: add hard budgets for headless deterministic paths and separate visual budgets.
