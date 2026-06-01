# Requirement Traceability

| Requirement | Owning subbundle | Source files or artifacts | Required proof |
|---|---|---|---|
| R01 Cross-repo validation and warning budget | SB01 | `README.md`, `04_validation/validation_commands.md`, `04_validation/forbidden_reference_policy.md` | Branch/status transcript, warning budget, focused validation transcript |
| R02 Browser apply adapter | SB02 | `03_code_skeletons/Components_WebGlRunBrowserApplyAdapter_shape.md`, Components WebGlRunLib source | WebGlRunLib fake-runtime tests, source assertions, anti-stub audit |
| R03 Stage barrier hardening | SB03 | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js`, `32-webgl-scene-stage-barriers.js`, `33-webgl-scene-command-journal.js` | JS audit, WebGlLib tests where applicable, journal/source assertions |
| R04 Runtime snapshot | SB04 | `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeSnapshot.cs`, scene diagnostics | Snapshot export test and source assertions |
| R05 Desktop sandbox page | SB05 | `03_code_skeletons/Economy_SimulationSandboxPage_shape.md`, Economy Components/SimulationSandbox | Blazor build/test, large-screen browser proof or explicit host blocker |
| R06 Real scenario artifacts | SB06 | `03_code_skeletons/Economy_RealScenarioRunnerOptions_shape.md`, `EconomyRealScenarioRunner.cs`, `EconomyRealProbeArtifactExporter.cs` | Exporter tests, artifact assertions, readiness JSON assertions |
| R07 Strict visual mapping | SB07 | `08_probe_scenarios/`, fixture `visual.mapping.json`, WebGlBridge validator | Strict mapping tests for no fallback/no-op |
| R08 Session persistence | SB08 | `EconomySimulationSandboxSessionService.cs`, `FileSimulationSnapshotStore.cs` | Export/import restart-style test, snapshot listing test |
| R09 Snapshot analysis facets | SB09 | `SimulationSnapshotAnalyzers.cs`, `SimulationSnapshotAnalysis.cs` | Shared-resource and finite-resource analyzer tests, domain-term scan |
| R10 Backend registry and ledger readiness | SB10 | `EconomySimulationSandboxPipelines.cs`, backend registry contracts | Fake/missing/ledger descriptor tests |
| R11 Browser smoke artifacts | SB11 | Economy sandbox page, WebGlRun browser adapter, artifact folder | `browser-smoke-readiness.json`, scene/frame/snapshot proof JSON, screenshot or blocker |
| R12 Performance gate | SB12 | `09_performance/performance_risk_register.md`, performance tests/audits | Metrics transcript with projection/export/snapshot size data |
| R13 Domain leakage/refactor gate | SB13 | generic Components/Economy source scans | Forbidden term scan, JS line-count audit, split follow-up list |
| R14 Final validation and closure | SB14 | `04_validation/validation_commands.md`, all proof manifests | Required command transcripts, final fake-proof resistance, completed validator |
