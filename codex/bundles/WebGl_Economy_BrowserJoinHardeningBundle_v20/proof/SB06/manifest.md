# SB06 Proof Manifest

Status: Completed

## Scope

Economy real scenario artifact runner hardening.

## Changed File Hashes

- `bundle://proof/SB06/transcripts/changed-file-hashes.txt`

## Command Transcripts

- `bundle://proof/SB06/transcripts/real-probe-exporter-tests.txt`
- `bundle://proof/SB06/transcripts/artifact-inventory-assertions.txt`
- `bundle://proof/SB06/transcripts/source-assertions.txt`
- `bundle://proof/SB06/transcripts/anti-stub-audit.txt`

## Generated Artifacts

- `bundle://proof/SB06/generated-real-scenario-runs/shared-well`
- `bundle://proof/SB06/generated-real-scenario-runs/farmer-land`

## Source Assertions

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` supports optional output cleanup and writes readiness reports under `volatile-reports`.
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` removes wall-clock `generatedAtUtc` from canonical input validation and writes `artifact-manifest.json` with canonical vs volatile categories.
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioRunner.cs` forwards cleanup options into export.
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioReadinessReporter.cs` cites `volatile-reports/readiness-report.json` and uses browser-smoke-input wording.
- Focused tests now write to temp output roots by default and only copy artifacts into bundle proof as explicit evidence.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| Canonical real scenario artifacts | `EconomyRealProbeArtifactExporter` | SB07 strict mapping, SB11 browser smoke, SB12 performance | Produced for shared-well and farmer-land runs | `bundle://proof/SB06/transcripts/artifact-inventory-assertions.txt` proves canonical input validation has no volatile timestamp. |
| Volatile readiness report | `EconomyRealProbeArtifactExporter` | Readiness reporter and bundle closure | Written under `volatile-reports/readiness-report.json` | Focused tests assert `readyForLargeScreenBrowserExecution` is absent and `browserRuntimeExercised` is false. |
| Cleanup option | `EconomyRealScenarioRunOptions` and exporter options | Repeatable local/proof runs | Optional before export | Focused tests create a stale artifact and prove it is removed only when cleanup is enabled. |

## Semantic Adequacy Evidence

- Semantic positive proof: shared-resource and finite-resource probe exports contain input validation, simulation frames/deltas, visual frames, WebGL run document, snapshots, snapshot analysis, artifact manifest, and volatile readiness reports.
- Adversarial negative proof: readiness reports use `readyForLargeScreenBrowserSmokeInput`, keep `browserRuntimeExercised` false, and omit `readyForLargeScreenBrowserExecution`.
- Anti-stub audit: `bundle://proof/SB06/transcripts/anti-stub-audit.txt`.

## Closure

SB06 passed. Headless real-scenario artifacts are deterministic where canonical, volatile where appropriate, and precise about browser-smoke readiness.
