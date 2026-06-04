# Semantic invariants - SB02

Status: completed

| Invariant | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing proof | Closure |
|---|---|---|---|---|---|
| SB02-IDLE-CONTRACT | Runtime idle means no active motions, queued motions, queued command stages, active barrier, automatic stage work, or scheduled render work. | Returning stop success while browser queues still exist. | SB01 browser baseline showed UI/runtime proof could be stale at first sample. | `bundle://proof/SB02/browser/pause-settled-after.json` proves zero active/queued work after Pause. | Passed |
| SB02-STOP-SNAPSHOT | Stop result and diagnostics expose idle, timeout, elapsed, blocker, and generation fields. | Metadata-only stop result without a typed diagnostic snapshot. | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests-failing-first.txt`. | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests.txt`. | Passed |
| SB02-STALE-CALLBACK | Motion completion callbacks from an earlier stop generation cannot mutate UI after stop. | Checking only C# `isPlaying` after callback delivery. | Generation mismatch path audited in `bundle://proof/SB02/transcripts/source-assertion-runtime-stop-scan.txt`. | Browser proof asserts no late `Motion completed:` status mutation after pause. | Passed |
| SB02-SHARED-STOP | Pause, Cancel, Reset, Step, and Seek use the same stop primitive before changing playback state. | Separate partial stop paths for different buttons. | Source assertion scan would show divergent handlers. | `RunPlayback.razor.cs` source assertion shows handlers route through `StopPlaybackAsync`, which calls `StopRuntimeActivityAsync(... waitForIdle: true)`. | Passed |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative test |
|---|---|---|---|---|
| `runtimeStopGeneration` | JS runtime stop and motion enqueue modules | Motion-completed guard, diagnostics, browser proof | Incremented on stop, copied into diagnostics, compared against motion generation before callback | Failing-first diagnostics test and browser pause proof |
| `lastRuntimeStopIdle` / `lastRuntimeStopTimedOut` / `lastRuntimeStopBlockers` | JS runtime idle wait | C# `WebGlRuntimeDiagnostics`, stop command diagnostics, RunPlayback proof JSON | Updated after idle wait when a stop has occurred | Failing-first diagnostics test |

