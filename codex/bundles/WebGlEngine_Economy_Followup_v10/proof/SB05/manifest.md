# Proof manifest SB05

Status: completed

Required proof: Playwright proof asserts activeMotion=0, queuedMotion=0, queuedStage=0 immediately and after drain.

## Artifacts

- WebGlRunLib test transcript: `bundle://proof/SB05/components-webglrun-phase-b-test.txt`
- Playwright browser assertions: `bundle://proof/SB07/playwright-runtime-state-assertions.txt`
- Playwright diagnostics JSON: `bundle://proof/SB07/playwright-diagnostics-json.txt`
- Playwright screenshot: `bundle://proof/SB07/run-playback-phase-b.png`
- Source hashes: `bundle://proof/SB05/phase-b-source-hashes.txt`
- Anti-stub audit: `bundle://proof/SB05/anti-stub-scan.txt`

## Source Assertions

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackStopCoordinator.cs` performs an immediate runtime stop, cancels C# playback, performs a final idle stop, drains the playback task, and optionally performs a late-drain stop.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` calls the stop coordinator from Pause/Cancel/Reset/Step/Seek flows and records runtime stop generation.
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackStopCoordinatorTests.cs` proves stop-before-cancel ordering and stale runtime callback rejection.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Runtime stop generation | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeStopGenerationPolicy.cs` | Stop increments generation; C# syncs latest generation; stale callbacks are ignored. | `bundle://proof/SB05/components-webglrun-phase-b-test.txt` includes stale callback rejection. |
| First-observable runtime idle after pause | Browser runtime on `/run-playback` | RunPlayback inspector diagnostics | Pause calls runtime stop before drain; browser proof shows active motions, queued motions, queued stages, and blockers are zero after the pause proof snapshot. | `bundle://proof/SB07/playwright-runtime-state-assertions.txt` |

## Gate Result

Pass. The browser assertion transcript reports `activeMotionCount=0`, `queuedMotionCount=0`, `queuedCommandStageCount=0`, `runtimeIdle=true`, and no blockers after pause.
