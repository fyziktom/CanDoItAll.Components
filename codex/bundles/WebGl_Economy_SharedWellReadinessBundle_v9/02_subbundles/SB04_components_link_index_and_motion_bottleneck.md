# SB04 - Components: link index and motion bottleneck

## Problem
Moving many objects can become O(movingObjects * links) if every motion scans all link groups.

## Tasks
- Ensure `linkGroupsByObjectId` is populated whenever links are added or removed.
- Update `syncLinksForObject` to use `linkGroupsByObjectId`.
- Add diagnostics:
  - linksUpdatedLastFrame
  - linkSyncScanCount
  - linkSyncIndexedHitCount
- Add tests/proofs that moving one object in a dense graph updates only related links.

## Performance target
With 100 objects and 300 links, moving one actor should update related links, not scan all 300 every frame.
