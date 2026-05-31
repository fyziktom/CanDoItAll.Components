# Proof manifest SB05

Status: Completed

## Scope

WebGlRun action-plan to runtime command-batch contract: stage barrier propagation, queue policy fields, unsupported action rejection, and C#/JS batch normalization parity.

## Changed Files

- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlObjectMotionCommand.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrame.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunStageBarrierPolicy.cs`
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionCompilerTests.cs`

SHA-256 hashes:

- `bundle://proof/SB05/hashes/changed-file-hashes.txt`

## Command Transcripts

- Failing-first stage barrier scan: `bundle://proof/SB05/transcripts/failing-first-run-plan-stage-barriers.txt`
- Failing-first broader contract scan: `bundle://proof/SB05/transcripts/failing-first-run-plan-barrier-contract.txt`
- WebGlRunLib tests: `bundle://proof/SB05/transcripts/webglrunlib-tests.txt`
- Command batch parity audit: `bundle://proof/SB05/transcripts/command-batch-parity-audit.txt`
- Runtime audit: `bundle://proof/SB05/transcripts/runtime-audit.txt`

## Source Assertions

- Run-plan batch source assertions and anti-stub scan: `bundle://proof/SB05/source-assertions/run-plan-batch-source-assertions.txt`
- Motion stages now emit `wait-for-object-motions` barriers with subject object IDs.
- Patch/wait stages with durations emit `time-delay` barriers.
- `WebGlRunFrameApplyResult` carries barrier policy/object IDs to `WebGlSceneCommandBatchStage`.
- Unsupported direct compiler actions add plan errors and do not produce silent wait/no-op command stages.
- Existing supported no-op mappings record `explicitNoOpMapping` metadata.

## Semantic Adequacy Gate

- Shallow-pass trap: stage IDs and waitSeconds can look correct while the runtime receives no barrier policy and unsupported actions quietly become empty stages.
- Adversarial negative proof: `bundle://proof/SB05/transcripts/failing-first-run-plan-stage-barriers.txt` shows WebGlRun did not propagate stage barriers before implementation.
- Semantic positive proof: `bundle://proof/SB05/transcripts/webglrunlib-tests.txt` proves barrier propagation, object IDs, unsupported-action rejection, and existing compiler behavior; `bundle://proof/SB05/transcripts/command-batch-parity-audit.txt` proves JS fixture parity.
- Anti-stub audit: `bundle://proof/SB05/source-assertions/run-plan-batch-source-assertions.txt` records no `TODO`, `NotImplemented`, `not implemented`, or fixture-specific markers in changed compiler/test files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `BarrierPolicy` / `BarrierObjectIds` on run stages | `repo://src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunStageBarrierPolicy.cs` and compiler calls | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`, runtime stage runner | Produced while compiling actions, copied into command-batch stages, consumed by JS stage barriers. | Failing-first scan and WebGlRunLib test assertions for barrier policies/object IDs. |
| `QueuePolicy` on motion commands | WebGlRun compiler and WebGlLib command contract | JS motion enqueue runtime | Produced as append policy for generated run motions and consumed by the runtime queue policy normalizer. | SB04 reject/cancel/replace audit plus SB05 source assertions. |
| Unsupported-action plan errors | `repo://src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionNormalizer.cs` and compiler error transfer | Plan validity and batch compiler callers | Unsupported actions remain in `plan.Errors` and are not converted into stages/motions. | `Batch_compiler_rejects_unsupported_action_without_wait_fallback` in WebGlRunLib tests. |

## Failures / Blockers

- No SB05 blocker.
- Runtime audit remains green with warning-threshold files only after splitting the barrier helper below the C# production file-size threshold.
