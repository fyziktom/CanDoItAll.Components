# SB13: Headless runner manifest v3

## Goal

Harden reproducibility manifests and artifact validation.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Add manifest schema v3 if needed with repository commits, package versions, policy, scenario/source hashes, design matrix hash, oracle corpus id, metric registry version, invariant registry version.
- Manifest diff must categorize environment-only vs model-affecting changes.
- Artifact validation must reject missing schemaVersion and empty files.
- CLI should print status, confidence, run hash, and manifest path.

## Required proof artifacts

- `proof/SB13/headless-manifest-tests.txt`

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
