# SB11 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB11-INV-001 | Components `/run-playback` works in a real browser in large and narrow viewports, including Step, Batch frame, Snapshot, diagnostics JSON, and WebGL canvas state. | Source-grep proof or a screenshot without route actions and assertions. | Browser proof fails if route does not load, canvas is absent, batch metrics do not reach 24 commands/stages, snapshot is missing, or overflow is present. | `proof/SB11/transcripts/run-playback-playwright-transcript.json` and `proof/SB11/browser/run-playback-proof.json` pass all assertions. | `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor*`. | SB12 final browser closure can rely on the generic WebGlRun route proof. |
| SB11-INV-002 | Economy Node `/economy/simulation-sandbox` loads runtime scenarios through the app-owned catalog, not test fixture paths. | A route renders because test fixtures are present locally. | `proof/SB11/transcripts/no-test-fixture-path-source-scan.txt` and browser `noTestFixturePath` assertions reject fixture-path use. | Economy browser proof passes with Node DI using `SimulationScenarios/EconomySimulationSandbox` copied to output. | `src/CanDoItAll.Economy.Node/EconomyNodeServiceRegistration.cs`, Node `.csproj`, sandbox page. | R01/R02/R12 closure and package-mode confidence. |
| SB11-INV-003 | Economy sandbox browser controls execute the full required flow: Load scenario, Apply frame, Step, First, Last, Snapshot, Analyze, diagnostics, and browser runtime state. | Only loading the page or applying one frame while leaving navigation/analysis unproven. | Browser assertions fail if any required control is missing or if browser runtime is not applied/ready. | `proof/SB11/browser/economy-sandbox-proof.json` records all required actions, browser apply `applied`, `sceneReady=True`, `runtimeErrorCount=0`, and analysis for 8 findings on the large flow. | `EconomySimulationSandboxPage.razor`, Node route, WebGl interop runtime. | SB12 cross-repo red-team closure. |
| SB11-INV-004 | Economy sandbox layout remains readable and free of horizontal overflow at 1600x1000 and 390x900. | A desktop-only UI passes while narrow devices scroll sideways. | `proof/SB11/transcripts/failing-first-economy-narrow-overflow.txt` captured scrollWidth 1280px at 390px viewport before the fix. | Passing Economy proof records `noHorizontalOverflowLarge=true`, `noHorizontalOverflowNarrow=true`, and 390px scrollWidth equals clientWidth. | `src/CanDoItAll.Economy.Components/wwwroot/css/economy-components.css`. | R12 UI proof and future sandbox demos. |
| SB11-INV-005 | Browser console/page errors remain zero for both routes; remaining warnings are limited to expected WebGL ReadPixels performance warnings. | Ignoring console logs because screenshots look correct. | Browser proof collectors split console errors/warnings and would fail on page errors. | `proof/SB11/transcripts/browser-proof-summary-assertions.txt` records zero console errors for both routes and only ReadPixels warnings. | Components and Economy browser routes. | SB12 final QA/red-team sign-off. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Generic playback route proof | Components WebGlSandbox | WebGlRun maintainers and SB12 | Browser transcript plus screenshots/diagnostics stored under SB11 | Assertion file fails if route, canvas, batch metrics, snapshot, or overflow checks fail. |
| Economy Node runtime provider | Node service registration and runtime scenario content | Economy sandbox UI | Provider registered at app startup and content copied to output/publish | Source/browser scans reject `tests/` fixture paths. |
| Economy sandbox browser runtime | Economy sandbox page and WebGL interop | Browser diagnostics and analysis panels | Full control flow exercised in large viewport, apply/step/snapshot repeated in narrow viewport | Assertion file fails on missing runtime readiness, reset, apply state, or analysis. |
| Responsive CSS override | Economy Components CSS | Narrow viewport sandbox route | Media query below 800px removes desktop minimum width and stacks controls/layout | Failing-first overflow transcript plus passing proof prove the behavior changed. |

## Raw Requirement Closure

| Requirement | Status | Closure proof |
| --- | --- | --- |
| R01 | Solved for runtime UI proof | SB02 removed fixture-path runtime loading; SB11 proves the Economy Node sandbox route loads from runtime scenario content in browser and source/browser scans reject fixture paths. |
| R12 | Solved | SB11 proves Components `/run-playback` and Economy `/economy/simulation-sandbox` across large and narrow viewports with actions, diagnostics, screenshots, console logs, and no overflow. |
| R14 | Preserved for SB11 | Components boundary audits pass and the only SB11 production code change is Economy-owned responsive CSS. |
