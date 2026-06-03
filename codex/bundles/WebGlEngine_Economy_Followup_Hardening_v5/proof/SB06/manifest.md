# Proof manifest SB06

Status: completed
Completed: 2026-06-03

- Objective: Economy deterministic replay performance strategy.
- Gate: Passed. Manual apply and seek/backward-style navigation use full deterministic replay with scene reset; contiguous forward Step after a stable browser frame applies only the delta frame without reset.
- Owned findings: F06.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor` | `49fdcfd8e59d6fc4a8f8b6b0f4dad5327856bcfcbdbd378711098d8330c55bba` | Adds stable-frame tracking, replay planning, replay-mode diagnostics, and incremental/full apply selection. |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomySimulationSandboxComponentTests.cs` | `19036611529a70e37a89f616fe65cbee5f84f6a1a5871c348094035c8f276ef3` | Adds regression coverage for full replay diagnostics and contiguous forward-step incremental replay. |

## Proof artifact hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB06/transcripts/failing-first-economy-replay-gap.txt` | See `bundle://proof/SB06/transcripts/changed-file-hashes.txt`. | Failing-first source-contract proof that previous sandbox replay always used full reset replay. |
| `bundle://proof/SB06/transcripts/economy-component-focused-tests.txt` | `fe59dccaaa8e33730943b257e7bd7b9c518d34d4deb42022c9face7760a61c1f` | Focused Economy component tests passed, 4 tests. |
| `bundle://proof/SB06/transcripts/economy-build-after-replay-strategy.txt` | `10f16df1a302eea9c2624b0a22b1d1a0312a7949d4ca42c20ff79abbad16435a` | Economy solution build passed after replay-strategy changes. |
| `bundle://proof/SB06/browser/economy-replay-mode-browser-proof.js` | `d73e6fa58b619efa075141e0fd42c011536ad53a1f91172ee00532a5f42eef54` | Browser proof harness for full/incremental/full replay flow. |
| `bundle://proof/SB06/browser/economy-replay-mode-assertions.json` | `9370fce4d69e42a52aa97a2746495bdb5621f631c58a5c7c492001c4db82d723` | Machine-readable browser assertions; all assertions passed. |
| `bundle://proof/SB06/browser/economy-replay-mode-after.png` | `0ae03c80eee2662f326dfda02c097957e9e7998c3cd5ddb17df68e6e353fc12e` | Screenshot of final full replay through frame 2 with browser diagnostics visible. |
| `bundle://proof/SB06/transcripts/economy-replay-mode-playwright.txt` | `37c6ee3ce9cc2a08a10dcb68f4fad7d7e4b18a43057179b5d3ad540477f26fcf` | Playwright transcript for the full/incremental/full browser proof. |
| `bundle://proof/SB06/transcripts/source-assertion-economy-replay-scan.txt` | See `bundle://proof/SB06/transcripts/changed-file-hashes.txt`. | Source scan proving replay-plan fields, diagnostics, and focused tests exist. |
| `bundle://proof/SB06/transcripts/components-domain-boundary-scan.txt` | See `bundle://proof/SB06/transcripts/changed-file-hashes.txt`. | Boundary scan proving WebGlLib/WebGlRunLib code stayed free of Economy/domain references. |
| `bundle://proof/SB06/transcripts/anti-stub-economy-replay-scan.txt` | See `bundle://proof/SB06/transcripts/changed-file-hashes.txt`. | Anti-stub audit for changed Economy source, tests, and browser proof harness. |
| `bundle://proof/SB06/transcripts/proof-hygiene-inventory.txt` | See `bundle://proof/SB06/transcripts/changed-file-hashes.txt`. | SB06 proof inventory with zero blank artifacts. |
| `bundle://proof/SB06/transcripts/changed-file-hashes.txt` | Self-contained transcript. | Portable SHA-256 inventory for changed files and primary proof artifacts. |

## Command transcripts

- `bundle://proof/SB06/transcripts/failing-first-economy-replay-gap.txt`: baseline source scan showed `BuildDeterministicReplay` always selected frames up to the current frame and always required scene reset.
- `bundle://proof/SB06/transcripts/economy-component-focused-tests.txt`: `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore --filter "FullyQualifiedName~EconomySimulationSandboxComponentTests"` passed 4 tests.
- `bundle://proof/SB06/transcripts/economy-build-after-replay-strategy.txt`: `dotnet build .\CanDoItAll.Economy.slnx --no-restore` passed with existing warnings and zero errors.
- `bundle://proof/SB06/transcripts/economy-replay-mode-playwright.txt`: browser proof passed on `http://127.0.0.1:5311/economy/simulation-sandbox` using a local-dev `--no-launch-profile` node.

## Browser proof summary

`bundle://proof/SB06/browser/economy-replay-mode-assertions.json` proves:

- manual Apply frame used `replayMode = full`, `resetApplied = True`, and applied frame `0`.
- contiguous forward Step used `replayMode = incremental`, `resetApplied = False`, and applied only frame `1`.
- Last used `replayMode = full`, `resetApplied = True`, and applied frames `0,1,2` through target frame `2`.

The screenshot `bundle://proof/SB06/browser/economy-replay-mode-after.png` visually confirms the final sandbox state: step 2, browser apply applied, and browser runtime diagnostics showing full replay of frames `0,1,2`.

## Semantic adequacy gate

- Shallow-pass trap: showing a replay mode before apply completion could falsely pass while the browser was still applying stale frames.
- Adversarial negative proof: the strengthened browser proof waits for target frame, applied frame indexes, and stable-frame diagnostics before accepting each phase.
- Semantic positive proof: focused tests and browser assertions prove full reset replay for manual/seek and single-frame delta replay for forward Step.
- Boundary proof: `bundle://proof/SB06/transcripts/components-domain-boundary-scan.txt` shows no Economy/domain references were introduced into WebGlLib/WebGlRunLib code.
- Anti-stub audit: `bundle://proof/SB06/transcripts/anti-stub-economy-replay-scan.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `LastStableBrowserFrameIndex` | `EconomySimulationSandboxPage.razor` updates it after successful browser apply. | `BuildReplayPlan` decides whether contiguous forward Step can apply incrementally. | Null until a successful apply; then tracks the latest browser-applied target frame; reset when scenario/browser state resets. | Browser proof requires stable frame `0`, then `1`, then `2`; focused test proves the incremental step starts only after a stable frame exists. |
| `ReplayMode` | `BuildReplayPlan` emits `full` or `incremental`. | Browser diagnostics, status text, and proof assertions. | `full` for manual/seek/back/non-contiguous plans; `incremental` only for contiguous forward Step. | Browser proof fails unless Step is `incremental` and Last is `full`. |
| `RequestedFrameIndexes` and `FrameReplayCount` | `BuildReplayPlan` records the exact frames sent to WebGlRun browser apply. | Browser diagnostics, tests, and proof assertions. | Incremental plan includes only frames after the stable frame; full plan includes all frames through target. | Browser proof requires Step `requestedFrameIndexes = 1` and Last `requestedFrameIndexes = 0,1,2`. |
| `ResetApplied` / `RequiresSceneReset` | `BuildReplayPlan` maps replay mode to browser apply reset policy. | `WebGlRunBrowserApplyAdapter` and browser diagnostics. | True for full deterministic replay; false for incremental forward replay. | Browser proof requires reset on manual/Last and no reset on Step. |
