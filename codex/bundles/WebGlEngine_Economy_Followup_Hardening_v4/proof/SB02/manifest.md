# SB02 proof manifest

Status: Completed.

Owned requirements: R01 browser/runner fail-closed semantics.

Raw notes: `bundle://analysis/01-current-state-after-v3.md`, `bundle://analysis/02-main-weaknesses-and-repair-strategy.md`, `bundle://requirements/01-normalized-requirements.md`.

Semantic invariant contract: `bundle://proof/SB02/semantic-invariants.md`.

## Changed file hashes

See `bundle://proof/SB02/changed-file-hashes.md`.

## Command transcripts

| Command / action | Result | Transcript |
| --- | --- | --- |
| `dotnet test ... --filter FullyQualifiedName~Runner_does_not_apply_when_frame_conversion_fails_after_execution_validation --no-restore` | Failing-first, 1 failed test proving old runner reported success after frame conversion error | `bundle://proof/SB02/transcripts/failing-first.txt` |
| `dotnet test ... --filter FullyQualifiedName~WebGlRunDocumentRunnerTests|FullyQualifiedName~WebGlRunBrowserApplyAdapterTests --no-restore` | Pass, 13 focused runner/adapter tests | `bundle://proof/SB02/transcripts/passing-tests.txt` |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore` | Pass, 46 WebGlRunLib tests | `bundle://proof/SB02/transcripts/webglrunlib-full-tests.txt` |
| Source assertion scan | Pass, SB02-INV-001 and SB02-INV-002 source lines present | `bundle://proof/SB02/transcripts/source-assertions.txt` |
| Components boundary audit | Pass, no forbidden Economy/domain terms in touched production files | `bundle://proof/SB02/transcripts/boundary-audit.txt` |
| Anti-stub audit | Pass, no TODO/NotImplemented/placeholder production paths in touched files | `bundle://proof/SB02/transcripts/anti-stub-audit.txt` |

## Source assertions

| Assertion | Evidence |
| --- | --- |
| `WebGlRunDocumentRunner` now merges `WebGlRunFrameApplyResult.Errors` and returns before `frameApplier.ApplyAsync`. | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunDocumentRunner.cs`; `bundle://proof/SB02/transcripts/source-assertions.txt` |
| `WebGlRunBrowserApplyAdapter` now distinguishes pre-apply validation, reset, and batch failure reasons. | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB02/transcripts/source-assertions.txt` |
| Fake applier proof shows no apply call occurs after frame conversion errors. | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunDocumentRunnerTests.cs`; `bundle://proof/SB02/transcripts/passing-tests.txt` |

## Anti-stub audit

`bundle://proof/SB02/transcripts/anti-stub-audit.txt` passed for touched production files.

## Browser artifacts

None. SB02 uses fake runtime/applier tests and does not claim browser-rendered behavior.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunBrowserApplyResult.FailureReason` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` and `bundle://proof/SB02/transcripts/source-assertions.txt` prove the adapter writes `PreApplyValidationFailed`, `ResetFailed`, and `BatchFailed`. | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` and `bundle://proof/SB02/transcripts/passing-tests.txt` assert all three values. | `ApplyAsync(WebGlRunFrameApplyResult)` sets the reason before returning the public result from each failure branch. | `bundle://proof/SB02/transcripts/failing-first.txt` proves the runner path previously allowed a shallow success; adapter tests prove failure reasons are not inferred from generic error counts only. |
