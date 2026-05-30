# SB06 — Components: scene runtime performance hardening

## Bottlenecks to inspect
- replacing full object groups for symbol-only changes;
- updating links by scanning all links;
- rebuilding all indexes on every patch;
- animated symbols keeping render loop active;
- repeated GLB clones and material clones;
- proof snapshot building full arrays too often.

## Required work
- Add symbol-only update path.
- Use `linkGroupsByObjectId` for link updates and validate it remains correct.
- Add diagnostics:
  - patched object count;
  - replaced object group count;
  - symbol-only update count;
  - link geometry rebuild count;
  - render frame reason histogram.
- Add a desktop-only performance proof:
  - at least 100 actors;
  - at least 50 links;
  - at least 100 motions through one command batch;
  - no mobile/tablet proof.
