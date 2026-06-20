# Proof manifest SB01

Status: completed
Completed: 2026-06-03

- Objective: Current-state, pause reproducer, and proof integrity baseline.
- Gate: Passed. Failing-first pause proof and proof-hygiene inventory exist before implementation work starts.
- Production or test code changed: none. SB01 added proof artifacts only.
- Owned findings: F01, F02, F09.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB01/browser/failing-first-pause-proof.js` | `EBABFF8BE7590F58F816EB1912F232EA745EC0D52465F53D696588592BD1CC84` | Browser proof script for the current pause/runtime gap. |
| `bundle://proof/SB01/browser/failing-first-pause-assertions.json` | `186EA8619E352227C94E26AC6E324C1774C4474D8BABE994A14952197639F91D` | Machine-readable failing-first pause assertions. |
| `bundle://proof/SB01/browser/failing-first-pause-after.png` | `2AC8AF5FE8B60515FA8C39D9206DCD39D5BA2E0C36229C253D4BABF12AD5696E` | Large-screen screenshot after the pause click. |
| `bundle://proof/SB01/transcripts/components-baseline-build.txt` | `5933383593775ABEE9B229850EF108E6A3BE56E446A1D825550B6964F3F7D9FF` | Baseline Components build transcript. |
| `bundle://proof/SB01/transcripts/failing-first-pause-playwright.txt` | `73E9387A7F9A0AB84BABF4A3853720F059AEC6F07925487819014B480A56019E` | Playwright CLI failing-first transcript. |
| `bundle://proof/SB01/transcripts/source-assertion-baseline-scan.txt` | `702F73DA2920243A8F7F1F9DAAD93E47B7F26B905735E41CB5658237DCFD0903` | Source scan showing internal stage cancellation exists but no public `stopRuntimeActivity` API exists. |
| `bundle://proof/SB01/transcripts/proof-hygiene-inventory.txt` | `9F6AC2CFA1AAFC581DB158A646FFA43B38D618C1F9E8B7B4CA31ACD2DC03BD9D` | Proof inventory: seven transcript-like files, zero blank files, one screenshot, one JSON assertion file. |
| `bundle://proof/SB01/transcripts/anti-stub-audit.txt` | `AB72B9A274C51D47F4B299D051CF7497CC863A7CC972BFE5CC44BFA48AAB68F6` | Anti-stub audit for SB01 baseline scope. |

## Command transcripts

- `bundle://proof/SB01/transcripts/components-baseline-build.txt`: `dotnet build .\CanDoItAll.Components.slnx --no-restore` passed with zero warnings and zero errors.
- `bundle://proof/SB01/transcripts/failing-first-pause-playwright.txt`: Playwright opened `/run-playback`, clicked Play, observed runtime work, clicked Pause, and returned JSON evidence.
- `bundle://proof/SB01/transcripts/proof-hygiene-inventory.txt`: no zero-byte transcript-like proof files remain in the SB01 proof set.

## Failing-first pause evidence

`bundle://proof/SB01/browser/failing-first-pause-assertions.json` records:

- before Pause: `queuedCommandStageCount = 1` on `run-frame:1`, stage `run.pose.work`;
- immediately after Pause: runtime still had queued stage work and `playingText` was still `True`;
- later after Pause: C# state reached `False`, but the command-stage journal shows `run.pose.work` completed and `lastStageCancelReason` stayed empty;
- assertions `runtimeStillBusyImmediatelyAfterPause = true`, `renderProgressedAfterPauseClick = true`, and `runtimeStopSignalMissing = true`.

This closes SB01 only as a reproducer/baseline. It does not close F01/F02/F03/F04; those remain owned by SB02 and SB03.

## Source assertions

- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js` contains internal `cancelCommandStageRunner`.
- `repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` exposes `ClearMotionsAsync`, but no public runtime stop method.
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` has `StopPlaybackAsync` that only cancels C# playback state/token in the baseline.
- `bundle://proof/SB01/transcripts/source-assertion-baseline-scan.txt` explicitly reports no `stopRuntimeActivity` public runtime API in `src`, `tools`, or `tests`.

## Semantic adequacy gate

- Shallow-pass trap: a screenshot-only proof or a C# `isPlaying = false` assertion would miss queued command-stage work continuing in JS.
- Adversarial negative proof: `bundle://proof/SB01/browser/failing-first-pause-assertions.json` proves runtime work remains after the pause click even though the C# state later reaches paused.
- Semantic positive proof for SB01 scope: the route builds and runs, Playwright observed the runtime work surface, and proof hygiene inventory is machine-readable.
- Anti-stub audit: `bundle://proof/SB01/transcripts/anti-stub-audit.txt`; no production/test code was changed in SB01 and no stub path was introduced.
- Raw-note closure: the user's observed Pause bug is reproduced, not solved. Implementation remains open for SB02/SB03.

## Production Behavior Artifact Matrix

Not applicable for SB01. This subbundle introduced no new production signal, state, record, or event.
