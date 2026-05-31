# SB06 Semantic Invariants

## INV-SB06-001 Backwards Seek Replays Deterministically

- Expected behavior: seeking backwards marks scene reset and returns replay frames from the initial frame to the target.
- Shallow-pass trap: only setting `CurrentFrameIndex` to the target.
- Positive proof: `bundle://proof/SB06/transcripts/webglrunlib-tests.txt`.

## INV-SB06-002 Playback Result Is Bridge-Ready

- Expected behavior: result exposes requested command, target frame, frames applied, stages queued, errors/warnings, and run-source provenance.
- Negative proof: tests assert concrete values for command, target, frame count, stage count, and three provenance hashes.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Playback result/provenance state | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackResult.cs` | Apply, replay, play-to-end | `bundle://proof/SB06/transcripts/webglrunlib-tests.txt` |

