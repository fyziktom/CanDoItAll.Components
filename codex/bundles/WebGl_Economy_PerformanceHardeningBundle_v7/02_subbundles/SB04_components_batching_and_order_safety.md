# SB04 - Components command batching and order safety

## Problem

Batching is essential for performance, but unsafe coalescing can break ordered behavior.

Example problematic sequence:

```text
move actor to well
change pose to writing
move actor home
```

Dropping duplicate motions per object or coalescing order-dependent patches can erase intermediate actions.

## Required changes

1. Add `BatchOrderingMode`:
   - `CoalesceIndependent`
   - `PreserveOrder`
   - `Sequential`
2. For `Sequence` actions, preserve ordered commands.
3. Only coalesce object patches when:
   - no add/remove operations are present,
   - patches target the same revision window,
   - no motion depends on intermediate pose/symbol state.
4. Do not drop duplicate motions per object when they are explicitly sequential waypoints.
5. Add tests:
   - duplicate independent motions are deduped
   - sequential motions are preserved
   - add/remove/patch ordering is preserved
   - patch after remove fails safely

## Bottleneck addressed

This keeps batching performance while preventing the "go there and back" path from being collapsed into one final target.
