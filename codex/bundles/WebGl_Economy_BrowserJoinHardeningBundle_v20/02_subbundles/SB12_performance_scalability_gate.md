# SB12 - Performance and scalability gate

## Status

Completed. Closure gate passed.

## Goal

Prevent the first demo from hiding O(n^2) issues.

## Tasks

- Add performance probes for:
  - many actors,
  - many resource stores,
  - many visual actions,
  - many stage barriers,
  - many snapshots.
- Record:
  - projection time,
  - export time,
  - snapshot serialization time,
  - run document size,
  - artifact total size.
- Add thresholds as warnings first, then hard gates later.

## Acceptance

- No excessive growth for moderate desktop scenario sizes.
- Large screen only.

## Prerequisites

- SB11 browser smoke artifacts or blocker recorded.

## Owned Requirements

- R12 Performance gate.

## Dependency Impact

Performance proof supports final closure and determines whether further browser smoke expansion is safe.

## Validation Depth

Focused performance tests/audits must record counts, elapsed times, JSON/run-document sizes, snapshot serialization time, artifact sizes, and warning thresholds.

## Proof Required

- Performance probe transcript.
- Metrics JSON or summary.
- `bundle://proof/SB12/manifest.md`

## Browser Validation Logging

Desktop/large-screen only. Browser performance is supporting proof if available; no mobile/tablet proof.

## Progression Gate

Pass only when moderate scenario sizes have bounded metrics and threshold warnings are explicit.
