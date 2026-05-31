# SB02 - Components Stage Execution Runtime

## Problem

`WebGlSceneCommandBatch` supports stages and `waitSeconds`, but the JS executor applies stage commands immediately. This is not enough for ordered visual sequences.

## Goal

Add real stage execution semantics in WebGL runtime without adding domain knowledge.

## Required behavior

- `applyCommandBatch` should apply top-level commands as before for non-staged batches.
- Staged batches should execute stages through a stage runner.
- Preserve order for `OrderingMode.Sequential` and `BatchingPolicy=preserve-order`.
- `waitSeconds` must be respected as a stage delay.
- Stage runner must not block the UI thread.
- Stage progress must expose diagnostics:
  - current batch id
  - current stage id
  - completed stage count
  - failed stage count
  - queued stage count
- Cancellation must be possible when importing a new scene or disposing the runtime.

## Suggested JS split

```text
29-webgl-scene-stage-runner.js
30-webgl-scene-stage-queue.js
```

## Tests

Add JS audit fixtures or browser proof that a staged batch with:

```text
move A -> target
wait
change pose
wait
move A -> home
```

keeps the command order.
