# SB11: Metric/invariant evaluator no-fallback mode

## Goal

Ensure strict evaluator calls cannot silently return zero/fallback values.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add strict evaluator options or context to `SimulationFrameMetricEvaluator` and `SimulationInvariantEvaluator`.
- Unknown metric kind and unknown invariant kind must error at evaluation time, not only definition validation time.
- Metric outputs must include evaluator version, precision, source frame hash, and registered descriptor id.
- Add tests for direct evaluator misuse.

## Required proof artifacts

- `proof/SB11/metric-invariant-tests.txt`

## Semantic adequacy gate

This subbundle may be closed only when:

1. the implementation is not a stub,
2. at least one failing-first or explicit before/after proof exists,
3. the proof contains concrete assertions, not screenshots alone,
4. no research/economic claim depends on browser proof unless the browser-observer band is explicitly exercised,
5. and all changed public contracts are documented.

## Reopen triggers

- Any hidden warning path can reach `research-ready`.
- Any runtime/browsing failure can be mistaken for an economic model failure.
- Any factor/oracle/metric path can silently default.
- Any proof artifact is empty or only states success without evidence.
