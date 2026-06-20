# SB14: Statistical comparison and non-comparability gates

## Goal

Make multi-run experiment comparisons safe.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Design harness must require explicit design matrix for comparisons unless single-run exploratory is declared.
- Stochastic mode must require seeds/repetitions and report variance/sample count.
- Deterministic mode must fail if repeated same configuration has different run hash.
- Comparison must fail if design matrix hash, policy, behavior profile, metric registry, or oracle corpus differs.

## Required proof artifacts

- `proof/SB14/design-comparison-tests.txt`

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
