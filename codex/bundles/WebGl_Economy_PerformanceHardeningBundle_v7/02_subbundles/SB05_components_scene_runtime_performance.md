# SB05 - Components scene runtime performance hardening

## Bottlenecks

1. Link updates: moving N objects with M links can become O(N*M).
2. Full scene rebuilds: import/update currently rebuilds dynamic scene; acceptable for load, not for live frames.
3. Asset cache disposal: verify `disposeAssetCache` is called in lifecycle dispose.
4. Model diagnostics: expensive bounds/material traversal should run once per cached asset template, not per clone.
5. Symbol animation can force continuous rendering; ensure only animated symbols trigger auto render.
6. Interop: applying many patch/motion commands one-by-one is expensive; use command batches.

## Required changes

- Build `linkGroupsByObjectId` and update only links connected to a moved object.
- Add `applyCommandBatchAsync` to sandbox playback if not already used.
- Ensure `disposeAssetCache(state)` is called during lifecycle dispose.
- Add diagnostics:
  - average frame time
  - peak frame time
  - rendered frame count
  - active motion count
  - link update count
  - batch command count
  - asset cache hit/miss count
- Add stress proof:
  - 100 agents
  - 200 links
  - 100 move actions in one batch
  - large desktop viewport only
