# SB10: Store resolution policy completeness

## Goal

Eliminate implicit store-selection noise in research mode.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Require explicit store-resolution policy when candidate count > 1 for source, target, shared, and effect roles.
- Emit structured metadata for accepted and rejected flows for every role.
- Add matrix tests for actor-owned, location-owned, shared-pool, market-pool, store-id-exact, heuristic/exploratory fallback.
- Research mode must reject heuristic fallback.

## Required proof artifacts

- `proof/SB10/store-resolution-tests.txt`

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
