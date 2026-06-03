# SB03 semantic invariants

## SB03-INV-001 Legacy playback overload cannot silently choose one frame

- Source raw note: R02 says no public helper may silently apply only the last/current frame of a multi-frame playback result.
- Expected behavior: `ApplyAsync(WebGlRunPlaybackResult)` fails closed when `FramesToApply.Count > 1`.
- Disallowed shallow implementation: converting only `CurrentFrame` or `FramesToApply[^1]` and returning success.
- Failing-first test and transcript: `Adapter_rejects_legacy_playback_apply_for_multiframe_results`, `bundle://proof/SB03/transcripts/failing-first.txt`.
- Passing test and transcript: same test in `bundle://proof/SB03/transcripts/passing-tests.txt`.
- Changed source files and hashes: `bundle://proof/SB03/changed-file-hashes.md`.
- Production assertions: `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` checks `FramesToApply.Count > 1` and returns `MultiFramePlaybackRequiresExplicitApply`.
- Red-team negative case: fake runtime records no scene imports or command batches for the rejected legacy multi-frame call.
- Downstream dependency check: SB04 Economy UI must call deterministic runner or explicit playback apply instead of direct single-frame conversion.

## SB03-INV-002 Explicit playback apply preserves multi-frame sequence and stop-on-failure

- Source raw note: R02 requires explicit multi-frame semantics, per-frame results, and stop on first failed frame.
- Expected behavior: `ApplyPlaybackAsync` imports the initial scene once when reset is required, applies every frame in `FramesToApply` order, returns per-frame results, and stops before later frames when one frame fails.
- Disallowed shallow implementation: wrapping a loop around the old single-frame helper without per-frame results or failure stop.
- Failing-first proof: the legacy overload failing-first transcript proves why explicit multi-frame semantics were necessary.
- Passing proof: `bundle://proof/SB03/transcripts/passing-tests.txt`.
- Changed source files and hashes: `bundle://proof/SB03/changed-file-hashes.md`.
- Production assertions: `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`.
- Red-team negative case: second-frame batch failure prevents the third frame from being applied.
- Downstream dependency check: full WebGlRunLib tests pass in `bundle://proof/SB03/transcripts/webglrunlib-full-tests.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunBrowserPlaybackApplyResult` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/source-assertions.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` | Created and returned by every `ApplyPlaybackAsync` call. | Stop-on-first-failed-frame test proves the result is behavior-backed, not just a DTO. |
| `MultiFramePlaybackRequiresExplicitApply` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/source-assertions.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` | Returned by legacy playback apply before runtime mutation. | Failing-first transcript shows the old helper mutated runtime state and returned success. |
