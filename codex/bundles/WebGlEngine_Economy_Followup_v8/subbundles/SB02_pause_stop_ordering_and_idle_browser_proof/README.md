# SB02: Pause/stop ordering and idle browser proof

## Goal

Fix the observed Pause bug by stopping browser runtime immediately, then draining C# playback task.

## Scope

- Repositories: `CanDoItAll.Components` and/or `CanDoItAll.Economy` as required.
- Keep Components generic.
- Keep Economy semantics in Economy.
- Large-screen browser proof only when browser proof is required.

## Required implementation tasks

- Change `RunPlayback.StopPlaybackAsync` ordering: request cancellation, immediately call `StopRuntimeActivityAsync(waitForIdle:true)`, then wait for playback task drain.
- Ensure late callbacks cannot change UI after stop generation changes.
- Add large-screen Play->Pause browser proof during active motion/stage.
- Assert idle state: activeMotionCount=0, queuedMotionCount=0, queuedCommandStageCount=0, no active barrier, stable frame after pause.

## Required proof artifacts

- `proof/SB02/browser-pause-proof.json`
- `proof/SB02/console.log`
- `proof/SB02/screenshot.png`

## Semantic adequacy gate

This subbundle may be closed only when:

1. the implementation is not a stub,
2. at least one failing-first or explicit before/after proof exists,
3. the proof contains concrete assertions, not screenshots alone,
4. no research/economic claim depends on browser proof unless the browser-observer band is explicitly exercised,
5. and all changed public contracts are documented.

## Reopen triggers

- Any hidden warning path can reach `research-ready`.
- Any runtime/browsing failure can be mistaken for an economic model failure.
- Any factor/oracle/metric path can silently default.
- Any proof artifact is empty or only states success without evidence.
