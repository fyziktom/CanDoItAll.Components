# SB08 Semantic Invariants

Status: Completed

## Invariants

| Invariant ID | Expected behavior | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
|---|---|---|---|---|---|
| SB08-I1 | A generic runner exports the required real-scenario artifact contract under `artifacts/economy/real-scenario-runs/<scenario-id>/`. | A test could only check that some files exist, or keep writing the old `analysis` folder. | `bundle://proof/SB08/transcripts/real-scenario-runner-failing-first.txt` shows the committed baseline lacked the runner contract and required `snapshot-analysis` folder. | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` and `bundle://proof/SB08/transcripts/generated-artifact-inventory.txt` prove both `shared-well` and `farmer-land` produce input validation, frames, deltas, visual frames, run document, snapshots, snapshot analysis, and readiness report. | `bundle://proof/SB08/transcripts/anti-stub-audit.txt` |
| SB08-I2 | Strict WebGL run-document errors stop export before artifacts are written. | A runner could rerun happy-path exports while silently ignoring invalid source metadata. | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` includes `RunnerRejectsStrictlyInvalidRunDocumentBeforeExport`, which removes `source.inputPackHash` and expects `missing-source-input-pack-hash` with no output directory. | The same passing transcript proves valid real scenarios export after strict validation succeeds. | `bundle://proof/SB08/transcripts/anti-stub-audit.txt` |
| SB08-I3 | The runner remains generic and keeps joined simulation plus visualization in Economy, not Components. | A shallow implementation could branch on `shared-well`/`farmer-land` fixture names or move Economy behavior into Components. | `bundle://proof/SB08/transcripts/source-assertions.txt` scans production sandbox code for fixture-name branching. | `bundle://proof/SB08/transcripts/source-assertions.txt` cites production runner/exporter code paths and `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` exercises the Economy test project only. | `bundle://proof/SB08/transcripts/anti-stub-audit.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `EconomyRealScenarioRunResult.StrictValidation` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioRunner.cs` | Runner export gate, tests, and later readiness checks | Produced after session projection; errors prevent artifact export | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` |
| `snapshot-analysis/<snapshot-id>.json` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` | Snapshot analysis and readiness consumers in SB09/SB10/SB12 | Produced once per snapshot from `SimulationSnapshotAnalysisService.Analyze` | `bundle://proof/SB08/transcripts/real-scenario-runner-failing-first.txt` |
| `readiness-report.json` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealProbeArtifactExporter.cs` | SB12 readiness report and SB13 large-screen smoke planning | Produced after all scenario artifacts and metrics are written | `bundle://proof/SB08/transcripts/real-scenario-runner-tests.txt` |

## Contract

SB08 is closed only if the artifact inventory contains every required file class for both probes, the test transcript includes a strict invalid run-document rejection, the source assertions prove no fixture-specific production branching, and the anti-stub audit remains clean.
