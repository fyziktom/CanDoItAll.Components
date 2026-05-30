# SB02 — Components: ordered action stages

## Problem
Current run frames can normalize patches/motions into a flat batch. This is unsafe for sequential actions.

## Required work
- Add `WebGlRunActionStage` or equivalent.
- Allow each frame to contain ordered stages.
- Each stage can contain patches, motions, waits, symbols, pose changes.
- Coalesce only within the same stage.
- Never deduplicate multiple motions of the same object across different stages.
- Add tests:
  - actor moves to target then returns home in one logical frame;
  - pose before movement is preserved;
  - admin-writing happens at target before return;
  - stage order survives batch normalization.

## Do not
- Add economy-specific terms.
- Add small/medium/mobile screen handling.
