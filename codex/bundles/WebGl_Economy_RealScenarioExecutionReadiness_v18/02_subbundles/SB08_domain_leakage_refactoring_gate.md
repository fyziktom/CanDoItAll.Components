# SB08 — Domain Leakage Refactoring Gate

## Goal

Ensure generic kernel and bridge remain scenario-independent.

## Forbidden in generic production code

```text
water, well, farmer, land, parcel, oligarchy, near-household, far-household, village
```

Allowed only in:

- fixture JSON
- test data/probe files
- human documentation
- scenario factory examples clearly under example/simple fixture code

## Required actions

- Extend source scan to include new files added by recent bundles.
- Avoid false positives in comments only if the file is a documented probe/test file.
- Ensure generic policies use terms like resource, actor, store, rule, institution, finite-resource, shared-resource.

## Acceptance

Boundary audit passes and produces a transcript.
