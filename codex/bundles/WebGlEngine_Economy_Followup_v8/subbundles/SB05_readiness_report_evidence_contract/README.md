# SB05: Readiness report evidence contract

## Goal

Prevent research-ready status from boolean-only flags.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Replace/extend `BrowserRuntimeExercised`, `UIExercised`, and `OracleProofExercised` with evidence records or artifact citations.
- ResearchReady must require valid evidence artifacts with hashes and schemas.
- Add failing-first test: setting booleans without artifacts must not produce research-ready.
- Update docs and CLI output.

## Required proof artifacts

- `proof/SB05/readiness-evidence-tests.txt`

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
