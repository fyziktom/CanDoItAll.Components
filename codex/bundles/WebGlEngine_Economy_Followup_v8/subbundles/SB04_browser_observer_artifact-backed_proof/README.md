# SB04: Browser observer artifact-backed proof

## Goal

Convert browser observer proof from smoke/demo to artifact-backed evidence.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Create an observer proof artifact schema with route, viewport, WebGlRunDocument hash, browser-loaded hash, runtime diagnostics, console errors, idle result, and screenshot path.
- Add Components proof route that can load a generated WebGlRunDocument artifact, not only a hard-coded demo document.
- Browser proof must compare expected completed stage ids and final object positions.
- Failures block browser-observer band only.

## Required proof artifacts

- `proof/SB04/browser-observer-proof.json`

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
