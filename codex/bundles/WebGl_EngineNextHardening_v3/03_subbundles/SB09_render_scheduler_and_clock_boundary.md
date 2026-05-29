# SB09 — Render Scheduler and Clock Boundary

## Goal

Keep render scheduling efficient while preserving a clear boundary between rendering and future run clocks.

## Current issue

The render loop has render reasons and can skip rendering while idle, but it still schedules `requestAnimationFrame` continuously. That is acceptable for early proof but not ideal for long-running visualizations.

## Tasks

1. Add a scheduler module:
   ```text
   22-webgl-scene-scheduler.js
   ```

2. Scheduler behavior:
   - `continuous`: rAF loop always active.
   - `auto`: rAF active only while motions, animated symbols, camera damping, resize, or pending invalidation exist.
   - `on-demand`: render only when explicitly invalidated, no continuous symbol animation.
   - idle state must not keep scheduling rAF forever.

3. Keep clock semantics generic:
   - WebGlLib may compute frame delta for rendering and interpolation.
   - WebGlLib must not own simulation time, scenario time, business step time, or deterministic replay time.
   - Future `WebGlRunLib` will provide run time/frame cursor.

4. Add diagnostics:
   - `isRenderLoopActive`
   - `renderSchedulerMode`
   - `lastScheduledReason`
   - `idleSinceMs`
   - `lastDeltaSeconds`

5. Browser proof:
   - with primitive static scene, render count stops increasing after idle.
   - with animated symbols, render count increases while animation active.
   - with motion, render count increases until motion completes and then idles.
   - with continuous mode, render count always increases.

## Done criteria

- Idle static scenes do not keep rAF alive.
- Animated and moving scenes still update.
- No run-specific clock semantics are introduced.
