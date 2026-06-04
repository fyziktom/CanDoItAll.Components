# SB08: External golden oracle corpus

## Goal

Move oracle expected values from code-only helpers to data fixtures.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Create `tests/Fixtures/GoldenOracles/*.json` or equivalent scenario-pack oracle folder.
- Oracle fixture stores expected stores, flows, issues, relationships, metrics, invariant results, frame hash chain, and tolerated diagnostics.
- Broken expected fixture must fail with path-addressed diff.
- Keep code helpers only as runner/comparator, not source of expected truth.

## Required proof artifacts

- `proof/SB08/golden-oracle-fixtures.txt`

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
