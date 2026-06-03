# SB03 proof manifest

Status: Completed.

Owned requirements: R02 multi-frame playback must have explicit semantics; R01 fail-closed browser apply support.

Raw notes: `bundle://analysis/01-current-state-after-v3.md`, `bundle://analysis/02-main-weaknesses-and-repair-strategy.md`, `bundle://requirements/01-normalized-requirements.md`.

Semantic invariant contract: `bundle://proof/SB03/semantic-invariants.md`.

## Changed file hashes

See `bundle://proof/SB03/changed-file-hashes.md`.

## Command transcripts

| Command / action | Result | Transcript |
| --- | --- | --- |
| `dotnet test ... --filter FullyQualifiedName~Adapter_rejects_legacy_playback_apply_for_multiframe_results --no-restore` | Failing-first, old overload returned success for multi-frame playback | `bundle://proof/SB03/transcripts/failing-first.txt` |
| `dotnet test ... --filter FullyQualifiedName~Adapter_rejects_legacy_playback_apply_for_multiframe_results|...ApplyPlayback... --no-restore` | Pass, explicit API applies frames in order and stops on first failed frame | `bundle://proof/SB03/transcripts/passing-tests.txt` |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore` | Pass, 49 WebGlRunLib tests | `bundle://proof/SB03/transcripts/webglrunlib-full-tests.txt` |
| Source assertion scan | Pass, SB03-INV-001 and SB03-INV-002 source lines present | `bundle://proof/SB03/transcripts/source-assertions.txt` |
| Components boundary audit | Pass, no forbidden Economy/domain terms in touched production code | `bundle://proof/SB03/transcripts/boundary-audit.txt` |
| Anti-stub audit | Pass, no TODO/NotImplemented/placeholder production paths in touched files | `bundle://proof/SB03/transcripts/anti-stub-audit.txt` |

## API compatibility note

`IWebGlRunBrowserApplyAdapter.ApplyAsync(WebGlRunFrameApplyResult)` remains unchanged. `ApplyAsync(WebGlRunPlaybackResult)` remains available for single-frame compatibility, but fails closed for multi-frame playback results. New callers that need replay semantics should use `ApplyPlaybackAsync(WebGlRunPlaybackResult)`, which returns `WebGlRunBrowserPlaybackApplyResult` with per-frame outcomes.

## Source assertions

| Assertion | Evidence |
| --- | --- |
| Legacy playback overload rejects multi-frame playback instead of applying only current/last frame. | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/source-assertions.txt` |
| Explicit playback API applies reset once, then applies each frame in the playback result sequence. | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` |
| Explicit playback API stops on first failed frame and records per-frame results. | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` |

## Anti-stub audit

`bundle://proof/SB03/transcripts/anti-stub-audit.txt` passed for touched production files.

## Browser artifacts

No real browser artifact is claimed. SB03 uses fake runtime proof that records imported scenes and applied command batches.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunBrowserPlaybackApplyResult` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/source-assertions.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` | `ApplyPlaybackAsync(WebGlRunPlaybackResult)` creates one result per playback apply and appends per-frame results as frames are applied. | Stop-on-first-failed-frame test proves later frames are not applied after a failed frame. |
| `MultiFramePlaybackRequiresExplicitApply` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB03/transcripts/source-assertions.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB03/transcripts/passing-tests.txt` | Legacy `ApplyAsync(WebGlRunPlaybackResult)` sets the failure reason and returns before runtime import/apply calls. | `bundle://proof/SB03/transcripts/failing-first.txt` proves the previous shallow implementation succeeded and applied one frame. |
