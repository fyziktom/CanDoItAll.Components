# SB07: Design matrix factor materialization

## Goal

Ensure experiment factors actually mutate scenario input/configuration or are rejected.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Implement factor bindings: parameters, policy fields, scenario metadata, resource quantities, event magnitudes, behavior profile, seed, or run-plan options.
- If a factor has no binding, fail with `design-factor-no-effect`.
- Effective configuration hash must include resolved factor effects, not only labels.
- Add test where two factor levels produce different frame hash chains; add test where labels-only matrix is rejected.

## Required proof artifacts

- `proof/SB07/design-factor-tests.txt`

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
