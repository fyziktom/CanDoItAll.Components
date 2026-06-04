# SB12: Behavior profile lockfile and diff tooling

## Goal

Make event expansion drift explicit and comparable.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Emit behavior profile lockfile in headless manifest and scenario pack validation.
- Add diff tool section for behavior profile id/version/hash/ruleset/rule fingerprints.
- Add test that changing a profile changes run hash and diff category is `behavior-profile-drift`.
- Add docs: expansion profile is economic policy, not UI detail.

## Required proof artifacts

- `proof/SB12/behavior-profile-diff-tests.txt`

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
