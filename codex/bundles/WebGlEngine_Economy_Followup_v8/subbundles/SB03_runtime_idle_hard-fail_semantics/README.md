# SB03: Runtime idle hard-fail semantics

## Goal

Make idle timeout a hard failure in all proof/claim paths.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add explicit hard-fail option to `applyCommandBatchAndWait` or C# wrappers.
- Ensure `runtimeIdle=false` cannot appear in a successful proof result.
- Calibrate false blockers: scheduled render should settle after final frame; continuous render mode should be marked non-proof-compatible.
- Add tests for timeout, active motion, queued stage, and clean idle.

## Required proof artifacts

- `proof/SB03/runtime-idle-tests.txt`
- `proof/SB03/runtime-idle-contract.md`

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
