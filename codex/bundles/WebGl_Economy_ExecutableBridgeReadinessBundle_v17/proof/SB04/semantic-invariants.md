# SB04 Semantic Invariants

## Invariant ID: SB04-executable-run-document

Raw note owned: `WebGlRunDocument` must be executable by a generic runner/controller, not only a DTO; it must support initial scene, selected frame application, stages as command batches, step/pause/resume/seek, diagnostics, current frame/stage/action IDs, and runtime snapshot export.

Expected behavior: the generic controller accepts a document, resolves frames, queues/applyable command batches, tracks current stage/action ids, handles reset/pause/resume/seek/step, and exports a runtime snapshot with provenance.

Shallow-pass trap: a DTO-only controller could return frames without converting stages to command batches or exposing current stage/action state.

Adversarial negative proof: `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` includes `Controller_exports_executable_state_snapshot_for_generic_document`, which would fail if resume did not advance, command batches were not generated, or stage/action IDs were missing.

Semantic positive proof: the same test builds a generic two-stage document, resets the initial scene, resumes to a frame, applies the generated command batch through an `IWebGlRunFrameApplier`, seeks, steps, and exports a snapshot.

Anti-stub audit: `bundle://proof/SB04/transcripts/anti-stub-audit.txt` reports no TODO, NotImplemented, template-only, or fixture-specific production markers in the changed playback surfaces.

Changed source files and hashes: `bundle://proof/SB04/transcripts/source-assertions-and-hashes.txt`.

Production assertions: `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs`, `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackState.cs`, and `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeSnapshot.cs`.

Red-team negative case: a controller that only stores the current frame index but leaves `CurrentStageIds`, `CurrentActionIds`, or `WebGlRunRuntimeSnapshot` empty would fail the executable contract test.

Downstream dependency check: SB10 may execute Economy-projected documents through this generic controller contract.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunRuntimeSnapshot` | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs`; `bundle://proof/SB04/transcripts/source-assertions-and-hashes.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs`; `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` | Produced by `ExportRuntimeSnapshot()` after playback commands update state | `Controller_exports_executable_state_snapshot_for_generic_document` rejects empty snapshot fields. |
| Playback current stage/action state | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs` | Updated inside `ApplyDetailedAsync()` for each resolved frame | The test rejects frame-only playback that omits stage/action IDs. |
