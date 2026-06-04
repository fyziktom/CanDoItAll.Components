# SB09: Metamorphic and conservation property tests

## Goal

Add broad noise-detection tests beyond fixed golden cases.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add conservation tests: resource total conserved except explicit source/sink/rejection rules.
- Add order tests: reordering independent events should not change final hash.
- Add scaling tests: doubling initial resources and magnitudes should scale flows/quantities predictably for selected scenarios.
- Add no-op tests: adding zero-magnitude event should not change economic state except trace metadata when configured.

## Required proof artifacts

- `proof/SB09/metamorphic-tests.txt`

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
