# SB02 — Runtime idle and stop semantics

## Purpose

Add a first-class idle/settled-state contract to WebGlLib.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Implement JS idle predicate over motions, motion queues, stage runner, active barrier, asset cache pending disposal and render loop.
- Expose `waitForRuntimeIdle` through `window.CanDoItAll.webglScene`.
- Add `WebGlSceneView.WaitForRuntimeIdleAsync` with timeout and typed diagnostics.
- Make `StopRuntimeActivityAsync` optionally wait for idle.

## Required proof

- Unit/harness proof for active motions, queued motions and queued stages.
- Browser proof that stop+idle completes within configured timeout.
- Timeout proof with a deliberately blocked barrier.

## Refactor gate

Before closing this subbundle, Codex must add a short self-review covering:

- API compatibility,
- generic/domain boundary,
- deterministic behavior,
- performance risk,
- proof adequacy,
- remaining open risks.

## Stop conditions

Do not continue to the next subbundle if a critical proof is browser-screenshot-only, placeholder-only, warning-only where a hard gate is required, or not tied to a source invariant.
