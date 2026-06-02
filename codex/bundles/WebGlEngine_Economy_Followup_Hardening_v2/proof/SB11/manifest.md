# SB11 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

Recorded in `proof/SB11/transcripts/changed-file-hashes.txt`.

| Repo | File | SHA256 |
| --- | --- | --- |
| Economy | `src/CanDoItAll.Economy.Components/wwwroot/css/economy-components.css` | `04c9d766b033be2a1d8c6335ebcd96982cbcf3691b1c2eff2e6f1f1bb8d678be` |

## Command transcripts

- Browser tooling note: `proof/SB11/transcripts/browser-tooling-note.txt`.
- Components WebGlSandbox server start/readiness/stop/listener proof: `proof/SB11/transcripts/server-start.txt`, `server-readiness-poll.txt`, `components-webglsandbox-server-stop.txt`, and `components-webglsandbox-listener-check-after-stop.txt`.
- `/run-playback` Playwright route/action transcript: `proof/SB11/transcripts/run-playback-playwright-transcript.json`.
- Economy Node first-start build-lock blocker and build-server reset: `proof/SB11/transcripts/economy-node-server.out.txt`, `economy-node-server.err.txt`, and `dotnet-build-server-shutdown-before-economy-node.txt`.
- Economy Node Release build/start/readiness/browser/stop proof: `proof/SB11/transcripts/economy-node-release-build.txt`, `economy-node-release-server-start.txt`, `economy-node-readiness-poll.txt`, `economy-sandbox-playwright-transcript.json`, and `economy-node-release-server-stop.txt`.
- Failing-first responsive overflow proof: `proof/SB11/transcripts/failing-first-economy-narrow-overflow.txt`.
- Passing build after responsive fix: `proof/SB11/transcripts/economy-node-release-build-after-responsive-fix.txt`.
- Assertion summary: `proof/SB11/transcripts/browser-proof-summary-assertions.txt`.
- Source assertions and fixture-path scans: `proof/SB11/transcripts/source-policy-assertions.txt` and `no-test-fixture-path-source-scan.txt`.
- Components boundary audits: `proof/SB11/transcripts/components-webgllib-boundary-audit.txt` and `components-webglrunlib-boundary-audit.txt`.
- Anti-stub/placeholder scan: `proof/SB11/transcripts/anti-stub-placeholder-scan.txt`.
- Changed file hash transcript: `proof/SB11/transcripts/changed-file-hashes.txt`.

## Browser artifacts

- `/run-playback` large and narrow screenshots: `proof/SB11/browser/run-playback-large.png`, `proof/SB11/browser/run-playback-narrow.png`.
- `/run-playback` diagnostics/proof/logs: `proof/SB11/browser/run-playback-large-diagnostics.json`, `run-playback-narrow-diagnostics.json`, `run-playback-proof.json`, `run-playback-console.log`, `run-playback-console-errors.log`, and `run-playback-console-warnings.log`.
- `/economy/simulation-sandbox` large and narrow screenshots: `proof/SB11/browser/economy-sandbox-large.png`, `proof/SB11/browser/economy-sandbox-narrow.png`.
- `/economy/simulation-sandbox` diagnostics/proof/logs: `proof/SB11/browser/economy-sandbox-large-diagnostics.json`, `economy-sandbox-narrow-diagnostics.json`, `economy-sandbox-proof.json`, `economy-sandbox-console.log`, `economy-sandbox-console-errors.log`, and `economy-sandbox-console-warnings.log`.

## Source assertions

- Components `RunPlayback.razor` declares `@page "/run-playback"` and exposes the proof stage plus diagnostics JSON test ids.
- Economy Node `SimulationSandbox.razor` declares `@page "/economy/simulation-sandbox"`.
- Economy Node registers `IEconomySimulationScenarioCatalog` from `AppContext.BaseDirectory/SimulationScenarios/EconomySimulationSandbox` and copies those runtime scenario files to build/publish output.
- Economy `EconomySimulationSandboxPage.razor` injects `IEconomySimulationScenarioCatalog` and exposes sandbox controls, summary, WebGL view, diagnostics, browser runtime, and analysis test ids.
- Economy responsive CSS now overrides the desktop `min-width: 1280px` at `@media (max-width: 800px)`, allowing the 390px viewport proof to pass without horizontal overflow.
- Source and browser text scans confirm no runtime UI path depends on `tests/` fixture content.

## Anti-stub audit

`proof/SB11/transcripts/anti-stub-placeholder-scan.txt` passes for the referenced browser UI routes and responsive CSS. Boundary audits for WebGlLib and WebGlRunLib also pass, preserving the Components generic boundary.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Generic playback browser state | Components WebGlSandbox `/run-playback` | SB11 browser assertions and future WebGlRun validation | Started from local app, exercised Step, Batch frame, Snapshot in large and narrow viewports | Console/page errors are zero; batch metrics require all 24 commands/stages after batch frame |
| Economy runtime scenario source | Economy Node DI and copied `SimulationScenarios/EconomySimulationSandbox` content | Economy sandbox browser page | Node starts from runtime content under build output, not test fixtures | Source and browser text scans reject `tests/` fixture paths |
| Economy browser runtime state | `EconomySimulationSandboxPage` and WebGL interop runtime | Browser diagnostics, summary, and runtime panels | Load, Apply frame, Step, First, Last, Snapshot, Analyze exercised in the hosted route | Browser assertions require scene ready, applied browser state, reset state, zero runtime errors, and snapshot analysis |
| Narrow responsive layout | Economy CSS media query | 390px viewport browser proof | Root min-width, controls, summary, layout, scene, and panels adapt below 800px | Failing-first transcript captured 1280px scroll width before the CSS fix; passing proof has scrollWidth equal to clientWidth |
| Browser console review | Playwright console/page-error collectors | Execution report and SB12 red-team closure | Logs stored per route with error and warning splits | Only expected WebGL ReadPixels performance warnings remain; error logs are empty |

## Gate decision

Pass. SB11 has artifact-backed browser proof for Components `/run-playback` and Economy Node `/economy/simulation-sandbox` across large and narrow viewports, including route actions, screenshots, diagnostics JSON, console review, fixture-path assertions, source audits, and the responsive CSS fix required by the failing-first narrow overflow proof.
