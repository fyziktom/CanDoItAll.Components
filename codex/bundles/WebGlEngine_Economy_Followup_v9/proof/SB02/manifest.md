# Proof manifest - SB02

Status: completed

## Scope

SB02 changes the Components Run Playback pause/stop path so the browser runtime is stopped immediately before playback cancellation/drain work can delay visible motion shutdown. It also exposes runtime stop generation and idle blockers in UI diagnostics, and guards stale runtime callbacks by generation.

## Changed files

Changed-file hashes:

- `bundle://proof/SB02/transcripts/changed-file-hashes.txt`

Production files:

- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackStopCoordinator.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeStopGenerationPolicy.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
- `repo://src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl-proof.js`

Test/proof files:

- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackStopCoordinatorTests.cs`
- `bundle://proof/SB02/browser/runplayback-pause-proof.mjs`
- `bundle://proof/SB02/semantic-invariants.md`

## Proof artifacts

- Stop ordering unit proof: `bundle://proof/SB02/transcripts/webglrunlib-tests.txt`
- WebGlLib regression proof: `bundle://proof/SB02/transcripts/webgllib-tests.txt`
- Sandbox build proof: `bundle://proof/SB02/transcripts/webglsandbox-build.txt`
- Browser proof transcript: `bundle://proof/SB02/transcripts/runplayback-pause-playwright.txt`
- Browser assertions: `bundle://proof/SB02/browser/runplayback-pause-assertions.json`
- Browser screenshot: `bundle://proof/SB02/browser/runplayback-pause-after.png`
- Browser console log: `bundle://proof/SB02/browser/runplayback-pause-console.log`
- Source assertions: `bundle://proof/SB02/transcripts/source-assertions.txt`
- Anti-stub scan: `bundle://proof/SB02/transcripts/anti-stub-scan.txt`
- Sandbox server logs: `bundle://proof/SB02/transcripts/webgl-sandbox-sb02.out.txt`, `bundle://proof/SB02/transcripts/webgl-sandbox-sb02.err.txt`

## Semantic adequacy gate

- Shallow-pass trap: a C# pause flag or a final idle wait after task drain could leave browser motion visible during the wait window.
- Adversarial negative proof: `WebGlRunPlaybackStopCoordinatorTests.Stop_async_stops_runtime_before_cancel_then_final_idle_stop_before_task_drain` asserts the sequence `stop(wait:false) -> cancel -> stop(wait:true)` and would fail the old cancel-first/wait-only structure.
- Semantic positive proof: browser proof clicked Play, observed active motion plus queued command stages before Pause, clicked Pause, and observed zero active motions, queued motions, queued command stages, and stage barriers within 208 ms.
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-scan.txt` found no `TODO`, `NotImplemented`, fixture-specific, or template-only markers in changed production files.
- Raw-note literal closure: SB02 reduces simulator/browser observer noise for pause/playback by stopping the actual browser runtime before host drain work.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative / proof citation |
| --- | --- | --- | --- | --- |
| `runtimeStopGeneration` command callback metadata/diagnostics | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` via `WebGlRunRuntimeStopGenerationPolicy` | Created on each runtime stop, copied onto command callbacks, compared before RunPlayback accepts MotionCompleted/CommandCompleted callbacks, displayed in diagnostics UI. | `bundle://proof/SB02/transcripts/webglrunlib-tests.txt`, `bundle://proof/SB02/browser/runplayback-pause-assertions.json` |
| Immediate playback stop sequence | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackStopCoordinator.cs` | RunPlayback `StopPlaybackAsync` | Immediate `StopRuntimeActivityAsync(waitForIdle:false)`, cancellation callback, final `StopRuntimeActivityAsync(waitForIdle:true)`, task drain, late drain. | `bundle://proof/SB02/transcripts/webglrunlib-tests.txt` |
| Sandbox pointerdown pre-stop event | `repo://src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl-proof.js` | WebGL runtime `stopRuntimeActivity` | Pause/Cancel/Reset/Step pointerdown stops browser runtime before Blazor server event latency can leave visible motion running. | `bundle://proof/SB02/browser/runplayback-pause-assertions.json` |
| Idle blocker UI diagnostics | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor` | Browser proof and operator-visible diagnostics panel | Latest runtime idle blockers are exposed as `webgl-run-idle-blockers` and in Diagnostics JSON. | `bundle://proof/SB02/browser/runplayback-pause-after.png`, `bundle://proof/SB02/browser/runplayback-pause-assertions.json` |

## Closure

SB02 passes. Browser proof route: `http://localhost:5298/run-playback`, viewport `1920x1080`, Play -> Pause, motion/stage blockers cleared within 208 ms, UI diagnostics exposed runtime stop generation `3` and idle blockers `none`, no browser console errors.
