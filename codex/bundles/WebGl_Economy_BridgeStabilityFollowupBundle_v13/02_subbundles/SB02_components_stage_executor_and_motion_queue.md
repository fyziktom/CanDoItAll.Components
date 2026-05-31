# SB02 — Components stage executor and per-object motion queue

## Problem
Stage-aware batch structures exist, but JS stage execution currently applies all stages immediately. `waitSeconds` does not act as a temporal barrier. Motion append mode is not a true ordered per-object queue.

## Tasks
- Add a generic stage executor in WebGlRunLib/WebGlLib boundary.
- Ensure stages can be applied sequentially.
- Implement per-object motion queue semantics:
  - one active motion per object
  - queued motions start after prior completion
  - replacing vs appending is explicit
  - cancellation is deterministic
- Ensure `queueMode=append` does not create simultaneous competing transforms.

## Tests
- same object: move A -> B -> C -> Home results in ordered completion.
- duplicate motions are preserved in sequential mode.
- `waitSeconds` delays the next stage in deterministic playback tests.

## Do not
- Add Economy or scenario-specific logic to Components.
