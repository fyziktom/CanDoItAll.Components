# Semantic invariants SB02

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB02-STOP-IDEMPOTENT` | Calling runtime stop twice must succeed and leave active motions, queued motions, queued command stages, and active barriers at zero. | `bundle://proof/SB02/transcripts/runtime-stop-audit.txt`; `bundle://proof/SB02/browser/runtime-stop-assertions.json`. | Passed. |
| `SB02-PUBLIC-FACADE` | WebGlLib must expose a public JS and C# stop operation without moving run or Economy semantics into WebGlLib. | `bundle://proof/SB02/transcripts/source-assertion-runtime-stop-scan.txt`; `bundle://proof/SB02/transcripts/webgllib-focused-tests.txt`. | Passed. |
| `SB02-DIAGNOSTICS` | Runtime stop must write observable diagnostics for stop count, reason, cleared motions, and cancelled command-stage work. | `bundle://proof/SB02/browser/runtime-stop-assertions.json`; `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`. | Passed. |
| `SB02-BOUNDARY` | Components contracts remain generic and domain-neutral. | `bundle://proof/SB02/transcripts/webgllib-runtime-boundary-audit.txt`; `bundle://proof/SB02/transcripts/anti-stub-audit.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB02 result |
| --- | --- |
| Shallow-pass trap | A facade-only stop API could return success while leaving command-stage or motion queues alive. |
| Adversarial negative proof | `repo://tools/webgllib/audit-runtime-stop.cjs` builds a state with active motion, queued motions, active barrier, and queued stage; after stop all are zero, and a second stop reports zero new work. |
| Semantic positive proof | `bundle://proof/SB02/browser/runtime-stop-assertions.json` calls the real public browser facade on `/run-playback` and verifies runtime queues/stages clear. |
| Anti-stub audit | `bundle://proof/SB02/transcripts/anti-stub-audit.txt`. |
| Raw-note literal closure | F03 is solved. F01 remains partially solved because RunPlayback still must call the new stop API in SB03. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `runtimeStopCount` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js`. | JS diagnostics/proof snapshots and C# `WebGlRuntimeDiagnostics`/`WebGlSceneProofSnapshot`. | Increments on `stopRuntimeActivity`; persists until scene reset/import. | Runtime audit proves second stop increments count while no work remains. |
| `lastRuntimeStopReason` | Runtime stop module. | Browser JSON proof, JS diagnostics, C# DTOs. | Updated every stop call with normalized reason. | Browser proof records distinct first/second stop reasons. |
| `clearedMotionCount` and `lastRuntimeStopClearedMotionCount` | Runtime stop module. | JS/C# diagnostics and proof snapshots. | Cumulative and last-stop motion-clear counts update only during stop. | Runtime audit starts with active/queued motions and proves second idle stop reports zero new clears. |
| `lastRuntimeStopCancelledCommandStageCount` | Runtime stop and stage cancel module. | JS/C# diagnostics and proof snapshots. | Updated during stop/stage cancel from active barrier plus queued stage count. | Runtime audit proves stage-only cancel clears stages without clearing motions. |
