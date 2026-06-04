# SB16: Final cross-repo red-team closure

## Goal

Perform final adversarial review and produce next-step decision.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Run focused tests/builds in both repos.
- Run proof validator and fail on empty artifacts.
- Red-team: can any warning/diagnostic/replay/browser issue still masquerade as economic result?
- Produce final decision table: exploratory-ready, headless-valid-ready, oracle-valid-ready, browser-observer-ready, research-ready.

## Required proof artifacts

- `proof/SB16/red-team-final.md`

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
