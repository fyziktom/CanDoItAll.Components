# SB06 - Components: command batching and performance hardening

Current risk: a run frame may apply many patches/motions one by one through JS interop.

Implement:

- `WebGlSceneCommandBatch`
- `ApplyCommandBatchAsync` on `WebGlSceneView`
- JS facade `applyCommandBatch(host, batch)`
- batch result with per-command results and one proof snapshot

Performance targets:

- one JS interop call per frame where possible;
- coalesce multiple object patches for same object;
- reject/diagnose duplicate motion commands for the same object if not explicitly allowed;
- track `batchCommandCount`, `batchDurationMs`, `coalescedPatchCount`, `droppedDuplicateMotionCount`.

Validation:

- 100 object motion commands in a batch should not trigger 100 interop calls from the Razor host.
