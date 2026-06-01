# SB13 - Domain leakage and refactoring gate

## Goal

Keep core generic and maintainable.

## Tasks

- Scan generic projects for example-specific terms:
  - water,
  - well,
  - farmer,
  - land,
  - parcel,
  - oligarchy,
  - near-household,
  - far-household.
- Allow those terms only in:
  - fixtures,
  - tests named for probes,
  - scenario factories intentionally scoped to examples.
- Split files above agreed line-count thresholds.
- Keep JS modules under runtime line-count gates.

## Acceptance

- Generic abstractions do not leak example terms.
- Broad tests have a split plan if they exceed limits.
