# Pause bug hypothesis and validation plan

## Observed symptom

During a performance/playback run, the user clicked Pause, but the scene continued to run.

## Most likely root causes

1. `RunPlayback.PlayAsync` is a long-running event handler. Even though it awaits internally, the component event pipeline can make a second UI event such as Pause arrive late or be effectively serialized behind the running Play handler.
2. `PauseAsync` only sets `isPlaying = false` and cancels the C# `CancellationTokenSource`. It does not cancel active JS/WebGL motions and does not cancel queued command-stage runner work.
3. The JS runtime has internal `cancelCommandStageRunner(state, reason)` and public `clearMotions`, but the public `window.CanDoItAll.webglScene` API does not expose a single runtime-level stop operation that cancels both command stages and motions.
4. `HandleMotionCompleted` can still update UI status after pause/cancel, producing stale success/status messages.

## Required proof

The failing-first proof must show at least one of these before the fix:

- After Pause, `isPlaying` is false but `activeMotionCount` or `queuedCommandStageCount` remains non-zero.
- The frame index, render count, motion count, or object position keeps changing after Pause.
- The status is overwritten by a delayed motion completion after Pause.

Passing proof must assert:

- Pause returns control quickly.
- Active motions, queued motions, and queued command stages are zero after the stop deadline.
- No frame/stage/motion progress occurs after the stop deadline.
- Cancel is idempotent.
