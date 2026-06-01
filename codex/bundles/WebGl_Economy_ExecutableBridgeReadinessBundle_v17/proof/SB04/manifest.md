# SB04 Proof Manifest

Status: Completed

## Owned Requirements

- R04 - Make `WebGlRunDocument` executable by a generic runner/controller and expose current frame, stage, action, diagnostics, and runtime snapshot state.

## Semantic Invariant Contract

- `bundle://proof/SB04/semantic-invariants.md`

## Changed Files

- `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackCommandKinds.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackState.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeSnapshot.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs`
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs`

Before/after SHA-256 hashes are in `bundle://proof/SB04/transcripts/source-assertions-and-hashes.txt`.

## Command Transcripts

- WebGlRunLib tests: `bundle://proof/SB04/transcripts/webglrunlib-tests.txt`
- WebGlLib tests: `bundle://proof/SB04/transcripts/webgllib-tests.txt`
- Source assertions and hashes: `bundle://proof/SB04/transcripts/source-assertions-and-hashes.txt`
- Anti-stub audit: `bundle://proof/SB04/transcripts/anti-stub-audit.txt`

## Source Assertions

- `WebGlRunPlaybackController` accepts `WebGlRunDocument`, resets initial scene state, seeks, steps, pauses, resumes, and exports `WebGlRunRuntimeSnapshot`.
- `WebGlRunFrameApplyResult.FromFrame` converts stages into generic `WebGlSceneCommandBatch` stages.
- The executable contract test applies a generic two-stage frame through an `IWebGlRunFrameApplier` and asserts current frame/stage/action state.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunRuntimeSnapshot` | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs`; `bundle://proof/SB04/transcripts/source-assertions-and-hashes.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs`; `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` | Produced by `ExportRuntimeSnapshot()` from controller state after reset/resume/seek/step | Test asserts stage/action IDs and provenance, so a frame-only or empty snapshot would fail. |
| `CurrentStageIds` / `CurrentActionIds` playback state | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs` | Updated by `ApplyDetailedAsync()` after frame resolution | Test requires two ordered stage ids and `action.move`, rejecting DTO-only playback. |

## Closure Gate

Passed. The controller remains generic and Economy-free while making run documents executable and observable.
