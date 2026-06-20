# Proof manifest SB03

Status: completed
Completed: 2026-06-03

- Objective: RunPlayback playback state-machine and pause fix.
- Gate: Passed. Browser proof clicked Play, observed queued stage work, clicked Pause, and proved C# state plus WebGL runtime diagnostics stopped within the bounded deadline.
- Owned findings: F01, F02, F04.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor` | `585FDE7E67824FB96C388CFDE23F7F92A568C3AB5D5000B3E1740ABF9FCE1BEC` | Adds component disposal contract for playback cancellation. |
| `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | `5DBE925C2C592FA13ED34F9BFE7828A8877FC55250BF594866ABD92CC9A321E1` | Refactors Play/Pause/Cancel into generation-scoped background playback with runtime stop and stale callback suppression. |

## Proof artifact hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB03/browser/runplayback-pause-browser-proof.js` | `0CBF0A74241BFB7851E9255B122DD464981EC0288E98FF47441642F51AADF020` | Browser proof script. |
| `bundle://proof/SB03/browser/runplayback-pause-assertions.json` | `22807D1FC358D499553101D4086DD3D0C2BB8355D993D19B0FBF7656117CC736` | Machine-readable pause assertions. |
| `bundle://proof/SB03/browser/runplayback-pause-after.png` | `F4CEF5009DE48C1E2A8C9AC0E6EF3D0E1FE3CEEE21CCDCEC342DA23D26AA0C50` | Browser screenshot after pause proof. |
| `bundle://proof/SB03/transcripts/components-build-after-runplayback-pause.txt` | `1E5AFBD6BB8F9AA5A03AAFB81CB4F9BF1F6C9DF5513F37CF95EC196CEB01200E` | Solution build passed with zero warnings/errors. |
| `bundle://proof/SB03/transcripts/webglrunlib-focused-tests.txt` | `72DCB8B9B490A4B38E93C9B712A69ECB1379BE8BCFD5DD86CA2787C43B977C17` | WebGlRunLib focused test project passed, 53 tests. |
| `bundle://proof/SB03/transcripts/runplayback-pause-playwright.txt` | `FC3376D2F9E482F33E653B0C0A7BEBFE2BF236384E8634CC96078BEF583D71F0` | Playwright pause proof transcript. |
| `bundle://proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt` | `38F33F7CD06CB8567148F2AC7D7B9CAF31056F679D81CCABD19DFD0B8CBE549D` | Source scan proving playback state-machine contracts exist. |
| `bundle://proof/SB03/transcripts/anti-stub-audit.txt` | `17C2B363DF4AD958432BE0931FCD36D5F62F781F2A4C6E409532450EAC24131E` | Anti-stub audit for SB03 changed/proof files. |
| `bundle://proof/SB03/transcripts/proof-hygiene-inventory.txt` | `3E4055C9C9E32DD78AE5109DA0402513FF2D9111D936B96AAC28E8D14AD8322A` | SB03 inventory: eight transcript files, zero blanks. |
| `bundle://proof/SB03/transcripts/sandbox-server-sb03.out.txt` | `5AD90AC1089C2C89A1EE1AD871BB60AEF93422039ACBD2306173BF504C57576B` | Sandbox server stdout for browser proof. |
| `bundle://proof/SB03/transcripts/sandbox-server-sb03.err.txt` | `4AF8BFA475AE27E73110FD770F7F35D3355ACB498F394DA2FC5E78A5A5D1490E` | Sandbox server stderr note; no stderr emitted. |

## Command transcripts

- `bundle://proof/SB03/transcripts/components-build-after-runplayback-pause.txt`: `dotnet build .\CanDoItAll.Components.slnx --no-restore` passed with zero warnings and zero errors.
- `bundle://proof/SB03/transcripts/webglrunlib-focused-tests.txt`: `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore` passed 53 tests.
- `bundle://proof/SB03/transcripts/runplayback-pause-playwright.txt`: browser proof exercised the real `/run-playback` route and UI buttons.
- `bundle://proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt`: source scan proves generation, CTS, runtime stop, pause/cancel, stale callback suppression, and dispose contracts are present.

## Browser proof summary

`bundle://proof/SB03/browser/runplayback-pause-assertions.json` records:

- before Pause: frame `1` and `queuedCommandStageCount = 1`;
- Pause click returned in `150 ms`;
- after the pause deadline: frame still `1`, `queuedCommandStageCount = 0`, `activeMotionCount = 0`, and `commandStageBarrierPolicy = ""`;
- C# state showed `Playing = False`, status stayed `Paused.`, and runtime diagnostics recorded `lastRuntimeStopReason = "Paused."`;
- frame, command-stage journal, and motion-completed counts stayed stable after the deadline.

## Semantic adequacy gate

- Shallow-pass trap: C# `isPlaying = false` could make the UI look paused while queued WebGL command stages kept running.
- Failing-first baseline: `bundle://proof/SB01/browser/failing-first-pause-assertions.json` and `bundle://proof/SB01/transcripts/failing-first-pause-playwright.txt` captured exactly that stale queued-stage behavior before the fix.
- Semantic positive proof: SB03 browser proof clicks the real Pause button, waits past the deadline, and verifies runtime diagnostics plus component state remain stopped.
- Anti-stub audit: `bundle://proof/SB03/transcripts/anti-stub-audit.txt`.
- Raw-note closure: F01/F02/F04 are solved for RunPlayback; deeper runner lifecycle contracts continue in SB04/SB05.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `playbackGeneration` | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` increments on Play/Stop/Dispose boundaries. | `PlayLoopAsync` and `IsActivePlayback`. | A new Play owns one generation; Stop/Dispose invalidates stale loops and callbacks. | SB03 browser proof shows status remains `Paused.` and frame/stage counts stay stable after Pause. |
| `playbackCancellation` | RunPlayback creates a CTS on Play and cancels it on Pause/Cancel/Reset/Step/Seek/Dispose. | `PlayLoopAsync` and `BrowserFrameApplier.ApplyAsync`. | CTS is disposed when the owning generation exits; Stop cancels before runtime stop proof capture. | Source scan plus browser proof prove Pause returns promptly and no frame progress occurs after the deadline. |
| `isPlaying` | RunPlayback sets true only while an active generation is running. | UI status, `PlaybackStatus`, `HandleMotionCompleted`, and `IsActivePlayback`. | Set false by Stop/loop completion/Dispose; stale motion-completed callbacks return without overwriting status. | Browser proof proves `Playing = False` and status remains `Paused.` after runtime work is cleared. |
| `lastRuntimeStopReason` and `runtimeStopCount` | SB02 runtime stop API records stop diagnostics. | RunPlayback pause proof and diagnostics panel. | Pause calls `StopRuntimeActivityAsync("Paused.")`; diagnostics persist until scene reset/import. | Browser proof proves the recorded reason is `Paused.` after the Pause UI action. |
