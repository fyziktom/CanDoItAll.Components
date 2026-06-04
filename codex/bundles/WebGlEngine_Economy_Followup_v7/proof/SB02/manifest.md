# Proof manifest - SB02

Status: completed

## Required artifacts

- `bundle://proof/SB02/browser/pause-settled-after.json`
- `bundle://proof/SB02/browser/pause-settled-after.png`
- `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests.txt`

## Additional artifacts

- `bundle://proof/SB02/browser/pause-settled-after.cjs`
- `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests-failing-first.txt`
- `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-focused-tests.txt`
- `bundle://proof/SB02/transcripts/components-webglrunlib-runtime-stop-focused-tests.txt`
- `bundle://proof/SB02/transcripts/runtime-stop-js-check.txt`
- `bundle://proof/SB02/transcripts/pause-settled-after-playwright.txt`
- `bundle://proof/SB02/transcripts/source-assertion-runtime-stop-scan.txt`
- `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- `bundle://proof/SB02/transcripts/changed-file-hashes.txt`

## Changed-file hashes

See `bundle://proof/SB02/transcripts/changed-file-hashes.txt`.

## Commands

| Command | Transcript | Result |
|---|---|---|
| `dotnet test ... WebGlRuntimeDiagnosticsTests.Runtime_diagnostics_round_trips_runtime_stop_fields` before production fields | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests-failing-first.txt` | Failed: missing typed runtime-stop idle fields |
| `dotnet test ... WebGlRuntimeDiagnosticsTests.Runtime_diagnostics_round_trips_runtime_stop_fields` after implementation | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests.txt` | Passed |
| `dotnet test ... WebGlRuntimeDiagnosticsTests|WebGlSceneCommandResultTests` | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-focused-tests.txt` | Passed 11/11 |
| `dotnet test ... WebGlRunBrowserApplyAdapterTests|WebGlRunDocumentRunnerTests` | `bundle://proof/SB02/transcripts/components-webglrunlib-runtime-stop-focused-tests.txt` | Passed 23/23 |
| `node --check` changed runtime JS modules | `bundle://proof/SB02/transcripts/runtime-stop-js-check.txt` | Passed |
| `node proof/SB02/browser/pause-settled-after.cjs ...` | `bundle://proof/SB02/transcripts/pause-settled-after-playwright.txt` | Passed all assertions |

## Semantic adequacy

- Shallow-pass trap: a C# pause flag or stop-call count could pass while browser motions/stages remain active or UI remains stale.
- Adversarial negative proof: the failing-first test rejected missing typed stop/idle diagnostics; SB01 browser proof captured stale immediate UI state.
- Semantic positive proof: `pause-settled-after.json` injects a long active browser motion, clicks Pause, and proves `Playing=False`, `Paused.`, `lastRuntimeStopIdle=true`, `lastRuntimeStopTimedOut=false`, zero active motions, zero queued motions, zero queued command stages, and advanced stop generation.
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative test |
|---|---|---|---|---|
| `runtimeStopGeneration` / motion generation | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js`; `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` | Browser diagnostics, motion-completed callback guard, C# diagnostics model | Incremented on runtime stop; assigned to motions at enqueue; stale motion completion is ignored if generations differ | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests-failing-first.txt`; `bundle://proof/SB02/browser/pause-settled-after.json` |
| `lastRuntimeStopIdle` / timeout snapshot | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js` | `WebGlSceneView.StopRuntimeActivityAsync`, RunPlayback diagnostics panel, browser proof | Updated after idle wait following a stop; C# stop result copies counts and idle fields into diagnostics | `bundle://proof/SB02/transcripts/components-webgllib-runtime-stop-tests-failing-first.txt` |

