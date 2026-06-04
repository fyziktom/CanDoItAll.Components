# SB02 — Runtime pause and idle settled-state contract

## Repository scope

Components

## Goal

Make pause/cancel a browser-runtime settled-state operation, not only a C# cancellation flag.

## Tasks

- Add or harden `WaitForRuntimeIdleAsync` on WebGlSceneView and JS runtime.
- Define idle as no active motions, no queued motions, no queued command stages, no active barrier, and no scheduled automatic stage work.
- Make `StopRuntimeActivityAsync` optionally wait for diagnostics to reflect idle.
- Ensure stale MotionCompleted callbacks are ignored after stop generation changes.
- Add timeout and diagnostic snapshot to stop result.

## Acceptance criteria

- Clicking Pause during active motion produces browser diagnostics with zero active/queued work.
- No stale completion callback changes status after pause.
- Cancel and Reset use the same runtime stop primitive.

## Required proof artifacts

- `proof/SB02/browser/pause-settled-after.json`
- `proof/SB02/browser/pause-settled-after.png`
- `proof/SB02/transcripts/components-webgllib-runtime-stop-tests.txt`

## Gate

Pause is not fixed until diagnostics prove settled browser runtime state.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
