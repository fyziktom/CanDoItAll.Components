# SB05 - Components WebGlRunLib playback controller

Repository: `CanDoItAll.Components`

## Problem

`WebGlRunLib` currently defines contracts but sandbox still performs playback logic directly in `RunPlayback.razor.cs`.

## Tasks

1. Add generic `WebGlRunPlaybackController`.
2. Add `WebGlRunTimelineValidator`.
3. Add `WebGlRunFrameResolver`.
4. Add `WebGlRunPlaybackClock`.
5. Add `WebGlRunPlaybackResult`.
6. Move reusable frame-selection/seek/next/play/pause logic out of sandbox.
7. Keep the sandbox as a thin demo consumer.

## Required semantics

- No economy terms.
- Deterministic frame selection.
- Seek to any frame by replaying patches/motions from initial scene or using snapshots later.
- Playback speed controls delay only, not run identity.
- `FrameRate`, `TimeSeconds`, and `Index` have clear precedence.

## Tests

Add `CanDoItAll.Components.WebGlRunLib.Tests` or extend existing tests if preferred:
- timeline order validation;
- duplicate frame index rejection;
- seek-to-frame emits expected frame list;
- playback speed excluded from deterministic identity.
