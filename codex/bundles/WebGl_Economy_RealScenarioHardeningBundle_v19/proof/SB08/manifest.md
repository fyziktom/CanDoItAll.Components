# SB08 Proof Manifest

Status: Completed

## Scope

SB08 proves Economy owns the joined simulation + visualization headless real-scenario runner. The runner remains generic, exports the required artifact contract for `shared-well` and `farmer-land`, and refuses strict WebGL run-document validation failures before writing artifacts.

## Evidence

| Evidence | Path | Result |
|---|---|---|
| Failing-first committed-baseline contract audit | `bundle://proof/SB08/transcripts/real-scenario-runner-failing-first.txt` | Failed as expected |
| Focused real-scenario runner tests | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` | Passed |
| Generated artifact inventory | `bundle://proof/SB08/transcripts/generated-artifact-inventory.txt` | Passed |
| Source assertions | `bundle://proof/SB08/transcripts/source-assertions.txt` | Passed |
| Anti-stub audit | `bundle://proof/SB08/transcripts/anti-stub-audit.txt` | Passed |
| Changed file and artifact hashes | `bundle://proof/SB08/transcripts/changed-file-hashes.txt` | Captured |

## Generated Artifacts

- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/input-pack.validation.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/simulation.frames.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/simulation.deltas.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/visual.frames.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/webgl.run-document.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/snapshots/<snapshot-id>.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/snapshot-analysis/<snapshot-id>.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/readiness-report.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/input-pack.validation.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/simulation.frames.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/simulation.deltas.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/visual.frames.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/webgl.run-document.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/snapshots/<snapshot-id>.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/snapshot-analysis/<snapshot-id>.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/readiness-report.json`

## Source References

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioRunner.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyRealProbeArtifactExporterTests.cs`

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `EconomyRealScenarioRunResult.StrictValidation` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioRunner.cs` | Runner export gate and focused tests | Session is loaded/projected, strict options are applied, validation errors are copied before artifact export | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` rejects a missing `source.inputPackHash` stage before export |
| `artifacts/economy/real-scenario-runs/<scenario-id>/snapshot-analysis/<snapshot-id>.json` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` | SB09/SB10/SB12 downstream artifact readers and readiness proofs | Exporter writes one analysis document per emitted snapshot beside the snapshot payload | `bundle://proof/SB08/transcripts/real-scenario-runner-failing-first.txt` shows the committed baseline wrote `analysis` instead |
| `readiness-report.json` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` | SB12 readiness report and SB13 smoke planning | Report summarizes input validity, backend/visual/WebGL counts, snapshot count, diagnostics, and large-screen readiness | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` requires non-zero counts and `readyForLargeScreenBrowserExecution=true` |

## Closure

The SB08 gate passed. `EconomyRealScenarioRunner` is generic production sandbox code with no fixture-name branching, both required probes generated the complete artifact set, and strict validation failures stop export before artifacts are written.
