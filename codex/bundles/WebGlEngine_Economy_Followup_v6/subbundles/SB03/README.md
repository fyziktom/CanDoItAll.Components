# SB03 — Playback state machine and replay modes

## Purpose

Prevent simulation UI and playback from mixing logical frame progress with unsettled runtime animation.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add explicit playback states: Idle, Loading, Playing, Pausing, Paused, Stopping, Failed.
- Add `PlaybackGeneration`/`CancellationToken` ownership tests.
- Split browser playback modes: incremental, absolute replay, snapshot-anchor replay.
- Ensure ApplyPlayback can wait for idle after each frame or at the end, based on policy.

## Required proof

- Play/Pause/Play browser proof passes.
- Seek/Last uses absolute replay or snapshot-anchor replay.
- Incremental Step does not replay the entire timeline.

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
