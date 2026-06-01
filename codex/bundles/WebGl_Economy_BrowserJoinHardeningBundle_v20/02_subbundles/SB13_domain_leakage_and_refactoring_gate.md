# SB13 - Domain leakage and refactoring gate

## Status

Completed. Closure gate passed.

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

## Prerequisites

- Implementation subbundles SB02-SB12 completed or blockers recorded.

## Owned Requirements

- R13 Domain leakage/refactor gate.

## Dependency Impact

Final closure depends on proving generic layers remain generic and maintainable.

## Validation Depth

Run forbidden term scans, project reference scans, JS audit line-count gates, and source-size checks. Allowed fixture/test exceptions must be explicit.

## Proof Required

- Domain leakage scan transcript.
- File-size/JS audit transcript.
- Split follow-up list if thresholds are exceeded.
- `bundle://proof/SB13/manifest.md`

## Browser Validation Logging

N/A unless refactoring changes the browser page. If it does, cite SB11 browser proof rerun.

## Progression Gate

Pass only when generic production abstractions avoid probe-specific terms or every exception is fixture/test/scenario-scoped with a follow-up.
