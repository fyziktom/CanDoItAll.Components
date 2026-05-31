# SB06 - Components Run Playback Hardening

## Goal

Make `WebGlRunPlaybackController` ready for bridge-driven runs.

## Required improvements

- Add explicit playback result state:
  - requested command
  - target frame
  - frames applied
  - stages queued
  - errors/warnings
- Add reset/replay semantics:
  - seeking backwards replays from initial scene deterministically.
- Add cancellation token propagation for stage runners.
- Add run-source provenance:
  - input pack hash
  - run plan hash
  - visual mapping hash
- Do not add Economy references.

## Tests

- seek backwards uses initial scene and replay frames
- play-to-end stops exactly at last frame
- invalid timeline returns errors without throwing
