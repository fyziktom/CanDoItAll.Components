# SB07 — Idle Render Scheduler

## Goal

Avoid a perpetual `requestAnimationFrame` loop in static scenes while preserving continuous rendering for motions, animated symbols, camera damping, and explicit continuous mode.

## Current risk

The render loop currently resolves a reason and only renders when needed, but still schedules `requestAnimationFrame` continuously. For tycoon/simulation views this can waste CPU/GPU when the scene is idle.

## Implementation tasks

Refactor render scheduling so that:

- `continuous` mode runs every frame;
- active motions keep the loop alive;
- animated symbols keep the loop alive only when visible and render mode is `auto`;
- camera damping keeps the loop alive for the remaining damping frames;
- static idle scenes do not schedule endless frames;
- any call to `scheduleRender(reason)` wakes the loop;
- resize/selection/patch/import/model-loaded events wake the loop;
- diagnostics expose scheduler state.

Add diagnostics:

```text
RenderMode
IsRenderLoopActive
LastFrameReason
RenderCount
FrameTimeMs
IdleSinceMs optional
ActiveMotionCount
AnimatedSymbolCount
```

## Acceptance criteria

- Browser proof captures render count, waits while idle, and verifies render count does not continuously increase in on-demand/auto mode without active effects.
- Continuous mode proof still increments render count.
- Motion proof shows render count increases while motion is active and stops after completion.

