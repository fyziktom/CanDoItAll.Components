# SB12 - Performance and scalability gate

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
