# SB15: Browser performance observer budget

## Goal

Keep browser performance separate from economic truth but useful for visual exploration.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add browser observer performance proof: load, first frame, settled frame, pause idle, batch settle.
- Browser budget failures block `browser-observer-valid` but not headless validity.
- Large-screen only; no small/medium/mobile optimization.
- Record top bottlenecks and WebGL diagnostics.

## Required proof artifacts

- `proof/SB15/browser-performance-proof.json`

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
