# SB02 semantic invariants

## SB02-INV-001 Runner frame conversion errors are fail-closed

- Source raw note: R01 and W02 require runner paths to stop for all known frame/apply errors, including errors returned by `WebGlRunFrameApplyResult.FromFrame`.
- Expected behavior: when execution validation passes but frame conversion returns errors, `WebGlRunDocumentRunner.ApplyCurrentFrameAsync` returns failure and does not call the applier.
- Disallowed shallow implementation: validating execution only and still applying the converted command batch when `frameResult.Errors` is non-empty.
- Failing-first test and transcript: `Runner_does_not_apply_when_frame_conversion_fails_after_execution_validation`, `bundle://proof/SB02/transcripts/failing-first.txt`.
- Passing test and transcript: same test in `bundle://proof/SB02/transcripts/passing-tests.txt`.
- Changed source files and hashes: `bundle://proof/SB02/changed-file-hashes.md`.
- Production assertions: `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunDocumentRunner.cs` merges `frameResult.Errors`, records failed stage ids, syncs diagnostics, and returns before `frameApplier.ApplyAsync`.
- Red-team negative case: mixed direct frame motion plus staged commands targets a known object, so execution validation passes and only the conversion policy catches the error.
- Downstream dependency check: full WebGlRunLib tests pass in `bundle://proof/SB02/transcripts/webglrunlib-full-tests.txt`; SB03 can rely on runner fail-closed semantics.

## SB02-INV-002 Browser adapter failure reasons are typed enough for diagnostics

- Source raw note: R01 requires browser apply diagnostics to explicitly distinguish `PreApplyValidationFailed`, `ResetFailed`, and `BatchFailed`.
- Expected behavior: `WebGlRunBrowserApplyResult.FailureReason` is set by the production adapter branch that failed.
- Disallowed shallow implementation: returning a failed result with only generic error strings and no branch-specific diagnostic signal.
- Failing-first proof: adapter tests were added with the new failure-reason assertions before the implementation was completed; the focused passing transcript verifies all branches after implementation.
- Passing proof: `bundle://proof/SB02/transcripts/passing-tests.txt`.
- Changed source files and hashes: `bundle://proof/SB02/changed-file-hashes.md`.
- Production assertions: `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`.
- Red-team negative case: pre-apply frame errors do not import or apply; reset failure imports but does not apply; batch failure applies once then reports `BatchFailed`.
- Downstream dependency check: SB03 multi-frame adapter work must preserve these failure reason branches.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `WebGlRunBrowserApplyResult.FailureReason` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; `bundle://proof/SB02/transcripts/source-assertions.txt` | `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`; `bundle://proof/SB02/transcripts/passing-tests.txt` | The public result is produced by `ApplyAsync(WebGlRunFrameApplyResult)` for every adapter call path. | `bundle://proof/SB02/transcripts/passing-tests.txt` covers three distinct negative branches instead of one generic failure. |
