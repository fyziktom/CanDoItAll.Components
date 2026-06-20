# SB03 Runtime Idle Contract

## Contract

- `WebGlSceneView.ApplyCommandBatchAndWaitAsync` requires runtime idle by default and sends `requireRuntimeIdle=true` plus `hardFailOnIdleTimeout=true` to the browser command-batch bridge.
- `WebGlSceneView.StopRuntimeActivityAsync` can wait for runtime idle and, when idle is required, marks the returned command result failed if the browser cannot prove idle.
- Browser command batch settle treats an idle timeout as a hard failure when idle proof is required.
- Browser runtime stop clears active motions, queued motions, command stages, barriers, scheduled render requests, and render-loop reasons before idle proof is evaluated.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `requireRuntimeIdle` | `WebGlSceneView.ApplyCommandBatchAndWaitAsync` | browser command-batch settle | per command batch | `proof/SB03/runtime-idle-tests.txt` asserts the option is sent by default |
| `hardFailOnIdleTimeout` | `WebGlSceneView.ApplyCommandBatchAndWaitAsync` | browser command-batch settle | per command batch | `proof/SB03/runtime-idle-tests.txt` covers hard-fail option propagation |
| `runtimeIdleRequired` | `WebGlSceneView.StopRuntimeActivityAsync` | host stop result metadata/diagnostics | stop/pause/cancel/reset | `proof/SB03/runtime-idle-tests.txt` covers failed idle proof marking the command failed |
| render scheduler clear | browser runtime stop | idle wait diagnostics | stop/pause/cancel/reset | SB02 browser proof shows idle true with zero blockers after pause |

## Proof

`proof/SB03/runtime-idle-tests.txt` passed 5 WebGlLib lifecycle tests with exit code 0. SB02 browser proof then exercises the contract through real Play/Pause on `/run-playback`.
