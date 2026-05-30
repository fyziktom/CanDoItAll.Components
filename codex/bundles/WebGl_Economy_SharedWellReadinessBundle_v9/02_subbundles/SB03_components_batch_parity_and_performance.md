# SB03 - Components: C# / JS batch parity and performance

## Problem
C# `WebGlSceneCommandBatchNormalizer` and JS `26-webgl-scene-command-batch.js` can drift.

## Tasks
- Add shared JSON fixtures for batch normalization.
- Add test/proof comparing C# expected normalized output with JS output.
- Add `batchingPolicy` support.
- Add counters:
  - command count before/after normalization
  - dropped duplicate motions
  - preserved ordered duplicate motions
  - coalesced patches
  - interop calls avoided

## Performance proof
Use a desktop viewport only. Validate:
- 100 actors with one motion each;
- 25 actors with 4 staged motions each;
- 500 patch updates in one batch.
