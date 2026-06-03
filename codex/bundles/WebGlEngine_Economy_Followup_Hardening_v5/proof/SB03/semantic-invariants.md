# Semantic invariants SB03

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB03-PAUSE-STOPS-RUNTIME` | Clicking Pause must stop queued command stages and active/queued WebGL runtime work within a bounded time. | `bundle://proof/SB03/browser/runplayback-pause-assertions.json`; `bundle://proof/SB03/transcripts/runplayback-pause-playwright.txt`. | Passed. |
| `SB03-SHORT-COMMAND` | Play must return command flow to the component while playback continues on a cancellable background loop. | `bundle://proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt`; `bundle://proof/SB03/transcripts/components-build-after-runplayback-pause.txt`. | Passed. |
| `SB03-STALE-CALLBACKS` | Motion-completed callbacks from stale runtime work must not overwrite paused/canceled status. | `bundle://proof/SB03/browser/runplayback-pause-assertions.json`; `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`. | Passed. |
| `SB03-DOMAIN-NEUTRAL` | Components remain generic and do not introduce Economy or production-line semantics. | `bundle://proof/SB03/transcripts/anti-stub-audit.txt`; `bundle://proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB03 result |
| --- | --- |
| Shallow-pass trap | The UI could show `Playing = False` while JS command stages continued after Pause. |
| Adversarial negative proof | SB01 reproduced queued runtime work after Pause before the fix. |
| Semantic positive proof | SB03 browser proof clicked the real Pause button and verified frame/stage/motion counts stayed stable after the deadline. |
| Anti-stub audit | `bundle://proof/SB03/transcripts/anti-stub-audit.txt`. |
| Raw-note literal closure | The raw note "Pressing Pause during a performance/playback test did not stop the scene" is solved for `/run-playback`. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `playbackGeneration` | RunPlayback increments generation on Play/Stop/Dispose. | `PlayLoopAsync`, `IsActivePlayback`. | New generation starts playback; Stop/Dispose invalidates stale work. | Browser proof shows no frame or journal progress after Pause. |
| `playbackCancellation` | RunPlayback owns CTS lifecycle. | WebGlRun runner and browser frame applier. | Created on Play, canceled on Stop, disposed when loop exits. | Browser proof records Pause returning in 150 ms and runtime queues cleared. |
| `isPlaying` | RunPlayback command handlers and loop finalizer. | UI status and motion-completed callback guard. | False after Pause, Cancel, loop completion, and Dispose. | Browser proof proves status stays `Paused.` instead of being overwritten by motion completion. |
| `lastRuntimeStopReason` | SB02 WebGlLib runtime stop API. | RunPlayback proof and diagnostics panel. | Updated by `StopRuntimeActivityAsync("Paused.")`. | Browser proof records `lastRuntimeStopReason = "Paused."`. |
