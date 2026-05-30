# SB02 — Components: ordered action stage contracts

## Problem

A shared-well visual sequence requires multiple motions for the same object in one logical event:

```text
move actor -> well
show/carry water
admin/write
return actor -> home
```

Current command batching and duplicate-motion dropping are useful for performance but dangerous without explicit stage boundaries.

## Tasks

1. Add first-class stage contracts to `WebGlRunLib`:
   - `WebGlRunActionStage`
   - `WebGlRunActionStagePolicy`
   - `WebGlRunActionStageBoundary`
   - `StageIndex`
   - `StageGroupId`
   - `CoalescingScope`: `StageOnly`, `Frame`, `None`
2. Extend `WebGlRunAction` or add a normalized action model that carries stage info.
3. Ensure batch normalizers never coalesce/deduplicate across stages unless explicitly allowed.
4. Add tests:
   - same object can move to target and then return home in the same frame when stages differ
   - same object duplicate motion in the same stage is deduplicated
   - sequence and parallel actions produce expected stage grouping

## Done criteria

- Ordered staged action plans are deterministic.
- Existing batching tests still pass.
- New shared-well-like sequence test fails before fix and passes after fix.
