# SB01: Current-state and proof integrity audit

## Goal

Establish exact post-v7 baseline and reject empty or stale proof artifacts.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Inventory changed files in both repos since previous bundle.
- Run focused builds/tests/audits or capture why impossible.
- Scan proof directories for empty transcripts, stale screenshots, missing manifests, and non-deterministic timestamps.
- Create a current-state report mapping each finding to this bundle.

## Required proof artifacts

- `proof/SB01/current-state.md`
- `proof/SB01/proof-integrity-scan.txt`

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
