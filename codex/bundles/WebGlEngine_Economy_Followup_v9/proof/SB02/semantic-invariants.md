# SB02 semantic invariants

## Invariants

| Invariant | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing proof | Status |
| --- | --- | --- | --- | --- | --- |
| SB02-I01 | Pause stops browser runtime before C# playback task drain can delay visible motion shutdown. | Only set `isPlaying = false` or cancel the C# token, then wait for task drain before stopping browser work. | `bundle://proof/SB02/transcripts/webglrunlib-tests.txt` asserts immediate `stop:false` before `cancel`, then final `stop:true`. | Browser proof observed active motion/queued stages before Pause and zero motion/stage blockers within 208 ms. | Passed |
| SB02-I02 | Stale MotionCompleted and CommandCompleted callbacks are ignored after runtime stop generation advances. | Trust callbacks only because `isPlaying` is true, allowing old runtime work to overwrite new playback status. | `WebGlRunRuntimeStopGenerationPolicy_rejects_stale_callbacks` in `bundle://proof/SB02/transcripts/webglrunlib-tests.txt`. | Source assertions show both callbacks route through `HandleRuntimeCompletion`. | Passed |
| SB02-I03 | Operators and browser proof can see runtime stop generation and idle blockers. | Hide runtime generation/blockers in raw JSON or rely on screenshots without assertions. | Browser proof fails if `runtimeStopGeneration < 1` or motion/stage blockers remain. | Screenshot and assertions show stop generation `3`, idle blockers `none`, and no disallowed console errors. | Passed |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Proof |
| --- | --- | --- | --- | --- |
| `runtimeStopGeneration` | JS runtime stop and command result completion | RunPlayback generation guard and UI diagnostics | Stop increments runtime generation; command/motion callbacks carry generation; C# ignores stale callback generations. | `bundle://proof/SB02/transcripts/source-assertions.txt`, `bundle://proof/SB02/transcripts/webglrunlib-tests.txt` |
| Immediate pre-stop pointerdown event | Sandbox proof bridge JS | WebGL scene runtime | User pointerdown on Pause/Cancel/Reset/Step clears runtime work before Blazor server event handling. | `bundle://proof/SB02/browser/runplayback-pause-assertions.json` |
| Idle blocker display | RunPlayback runtime idle result | UI panel and Diagnostics JSON | Runtime idle blockers update after stop/drain and appear in `webgl-run-idle-blockers`. | `bundle://proof/SB02/browser/runplayback-pause-after.png` |

## Anti-stub audit

`bundle://proof/SB02/transcripts/anti-stub-scan.txt` found no TODO, NotImplemented, fixture-specific, or template-only markers in changed production files.
