# SB06: Unclassified diagnostics closure

## Goal

Ensure every diagnostic contributes to a band or explicit unclassified failure.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add diagnostic classifier that classifies input, backend, projection, metrics, oracle, performance, runtime, and UI messages.
- Any unknown backend/input diagnostic becomes `unclassified-diagnostic` error in research mode.
- Add tests with synthetic unknown diagnostics from input/backend/projection.
- Expose diagnostic classification table in readiness report.

## Required proof artifacts

- `proof/SB06/diagnostic-classifier-tests.txt`

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
