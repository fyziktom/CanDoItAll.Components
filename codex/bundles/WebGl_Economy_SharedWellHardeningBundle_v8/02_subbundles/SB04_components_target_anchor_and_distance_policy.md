# SB04 — Components: target/anchor and distance policy

## Goal
Make generic target resolution reliable for scenarios like actors moving from home to a resource.

## Required work
- Add reusable target resolver service:
  - object by id;
  - anchor by key;
  - fallback to object center/base/top;
  - offset application;
  - diagnostics for unresolved targets.
- Add optional distance estimate output for planned motions.
- Add home/work/use/admin anchors as generic anchor keys, not economy semantics.
- Add tests for:
  - target object missing;
  - anchor missing fallback;
  - offset applied;
  - distance calculation deterministic.
