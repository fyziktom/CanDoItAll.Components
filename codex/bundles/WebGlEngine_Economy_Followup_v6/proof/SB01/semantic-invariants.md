# Semantic Invariants for SB01

## Invariant SB01-runtime-idle-after-pause

Source: Components WebGL runtime and sandbox playback page.

Expected behavior: after Pause, runtime activity stops and the public idle contract reports no active or queued motion/stage work before the proof completes.

Passing result: `browser-pause-idle-proof-passing.txt` reports zero active motions, queued motions, active stages, and pending stages after pause, with runtime stop count greater than zero.

Why this prevents simulator-noise contamination: visual animation work can no longer continue after the UI claims a paused state, so later economic or replay observations are not polluted by stale browser callbacks.

