# SB08 — Components: playback controller refactor

## Problem
The controller exists, but sandbox and library responsibilities need tightening.

## Required work
- Sandbox pages must use `WebGlRunPlaybackController`, not own playback loops.
- Add an applier that converts `WebGlRunFrameApplyResult` to `WebGlSceneView` API calls.
- Ensure replay/seek can reset/import initial scene and apply frames deterministically.
- Cache timeline max frame index instead of recomputing repeatedly.
- Add tests for:
  - seek backward replay;
  - play to end;
  - pause;
  - reset;
  - missing frame diagnostics.
