# Semantic invariants SB05

Status: Completed

- Invariant ID: `SB05-RUN-PLAN-EXECUTABLE-BATCH`
- Source raw note: RN-005
- Expected behavior: WebGlRun action plans compile into runtime-executable command batches with ordered stages, barrier policies, queue policies, trace metadata, and explicit unsupported-action errors.
- Disallowed shallow implementation: preserving stage IDs while dropping barrier policy/object IDs or silently converting unsupported actions into empty wait stages.
- Failing-first test: `bundle://proof/SB05/transcripts/failing-first-run-plan-stage-barriers.txt`
- Passing test: `bundle://proof/SB05/transcripts/webglrunlib-tests.txt`; `bundle://proof/SB05/transcripts/command-batch-parity-audit.txt`
- Changed source files: `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs`, `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlObjectMotionCommand.cs`, `repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrame.cs`, `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`, `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs`, `repo://src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunStageBarrierPolicy.cs`, `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionCompilerTests.cs`
- Production assertions: `bundle://proof/SB05/source-assertions/run-plan-batch-source-assertions.txt`
- Red-team negative case: unsupported action test proves no silent wait fallback; parity audit proves JS/C# fixture expectations still match.
- Downstream dependency check: SB06-SB07 may rely on traceable executable stages and strict unsupported-action behavior.
- Shallow-pass trap: a status/count-only test could miss that runtime barrier fields were absent.
- Adversarial negative proof: failing-first scan proves stage barrier propagation was absent before SB05.
- Semantic positive proof: WebGlRunLib tests prove generated barrier fields and unsupported-action rejection.
- Anti-stub audit: anti-stub scan found no placeholder markers in changed compiler files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Run-stage barrier contract | WebGlRun compiler and barrier helper | Frame apply result and JS stage runner | Built from action metadata/defaults, copied to command-batch stage, consumed during runtime stage scheduling. | Failing-first scan and WebGlRunLib barrier assertions. |
| Unsupported-action error state | WebGlRun action normalizer and compiler | Plan validity and batch compiler callers | Normalization errors are copied to `WebGlRunActionPlan.Errors`; invalid actions are skipped. | Unsupported-action batch compiler test proves no wait fallback stage is emitted. |
