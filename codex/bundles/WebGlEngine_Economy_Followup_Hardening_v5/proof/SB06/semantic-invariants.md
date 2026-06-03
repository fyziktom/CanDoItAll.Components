# Semantic invariants SB06

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB06-FORWARD-INCREMENTAL` | Contiguous forward Step after a stable browser frame applies only the delta frame and does not reset the scene. | `bundle://proof/SB06/browser/economy-replay-mode-assertions.json`; `bundle://proof/SB06/transcripts/economy-component-focused-tests.txt`. | Passed. |
| `SB06-SEEK-FULL-RESET` | Manual apply, backward, seek, and non-contiguous movement use full deterministic replay with scene reset. | `bundle://proof/SB06/browser/economy-replay-mode-assertions.json`; `EconomySimulationSandboxPage_LastReplaysAllDeltaFramesFromInitialScene`. | Passed. |
| `SB06-STABLE-FRAME-GUARD` | Incremental replay is allowed only when the browser has a known stable target frame immediately before the current frame. | `bundle://proof/SB06/transcripts/source-assertion-economy-replay-scan.txt`; `EconomySimulationSandboxPage_StepForwardUsesIncrementalReplayAfterStableFrame`. | Passed. |
| `SB06-DIAGNOSTIC-PROOF` | Browser diagnostics expose replay mode, requested frames, replay count, reset policy, stable frame, and applied frame result. | `bundle://proof/SB06/browser/economy-replay-mode-assertions.json`; `bundle://proof/SB06/transcripts/source-assertion-economy-replay-scan.txt`. | Passed. |
| `SB06-DOMAIN-BOUNDARY` | Components packages stay generic; Economy-specific replay decisions remain in the Economy sandbox host. | `bundle://proof/SB06/transcripts/components-domain-boundary-scan.txt`; `bundle://proof/SB06/transcripts/anti-stub-economy-replay-scan.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB06 result |
| --- | --- |
| Shallow-pass trap | A proof could pass from planned diagnostics before browser apply completed. The browser script now waits for target frame and applied frame indexes for each phase. |
| Adversarial negative proof | Forward Step would fail if it reset the scene, replayed more than one frame, or lacked stable-frame diagnostics; Last would fail if it skipped full replay/reset. |
| Semantic positive proof | Focused tests plus browser proof cover full manual apply, incremental Step, and full Last seek. |
| Anti-stub audit | `bundle://proof/SB06/transcripts/anti-stub-economy-replay-scan.txt`. |
| Raw-note literal closure | F06 is solved for the Economy browser sandbox: repeated contiguous stepping no longer rebuilds from frame zero after a stable browser frame exists. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `LastStableBrowserFrameIndex` | Economy sandbox browser apply success path. | Replay planner. | Reset with browser/scenario state; updated only after successful apply. | Browser proof requires stable frame updates through `0`, `1`, and `2`. |
| `ReplayMode` | `BuildReplayPlan`. | Status text and browser diagnostics. | `full` for reset replay, `incremental` for contiguous forward Step. | Browser proof requires Step incremental and Last full. |
| `RequestedFrameIndexes` | `BuildReplayPlan`. | Browser diagnostics and tests. | Derived from the current frame and stable frame; full replay includes all frames through target. | Browser proof requires `1` for Step and `0,1,2` for Last. |
| `FrameReplayCount` | Browser diagnostic projection of requested replay frames. | Tests and proof assertions. | Equals requested frame count for each apply plan. | Browser proof requires Step count `1` and Last count greater than Step. |
| `ResetApplied` / `RequiresSceneReset` | Replay plan and browser apply request. | WebGlRun browser apply adapter and diagnostics. | True for full replay; false for incremental Step. | Browser proof requires reset true on manual/Last and false on Step. |
