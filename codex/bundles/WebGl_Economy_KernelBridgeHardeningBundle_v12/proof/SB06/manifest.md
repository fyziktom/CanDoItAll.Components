# SB06 Proof Manifest

Status: Completed

## Scope

Run playback reports requested command, target frame, frames applied, stages queued, reset/replay state, errors/warnings, cancellation propagation, and source provenance hashes.

## Changed File Hashes

| Reference | SHA-256 |
|---|---|
| `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackResult.cs` | `A5C0F71E6CFD8256F156E3E02DC07998A838AEBCEC24FB44CE7B7E81512DC745` |
| `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` | `43B2A3D8D5612C5D8C729C48DC4342BE560277E2C15F79211F7324E388B13660` |
| `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs` | `82F568BEA79004BE0FEACCF1B243F90AD9C984EBC2EFFDE25A2A14FC77421DAC` |

## Command Transcripts

| Artifact | SHA-256 |
|---|---|
| `bundle://proof/SB06/transcripts/webglrunlib-tests.txt` | `9DCAF001BBC7870ECE41ECA6EEED2C224F211FA88D447719A028103FE9F2E6C8` |
| `bundle://proof/SB06/transcripts/source-assertions.txt` | `18DB7C6A26AB7F9FA8E9D37944D74AD82E953D16972BC5C456B8F027249577AB` |
| `bundle://proof/SB06/transcripts/components-anti-stub-audit.txt` | `15CECDD0D0E38DB696745090F7A277B41017E3A987050C9DFA6B8B77332086A9` |

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Playback result state and run-source provenance | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackResult.cs` and run playback UI/bridge consumers | Apply command, resolve frames, replay on backwards seek, return provenance | `bundle://proof/SB06/transcripts/webglrunlib-tests.txt` asserts reset/replay and provenance fields. |

