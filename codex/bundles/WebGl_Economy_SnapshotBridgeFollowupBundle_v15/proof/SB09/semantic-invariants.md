# Semantic invariants SB09

Status: Completed

- Invariant ID: `SB09-SNAPSHOT-ANALYSIS-GENERIC-FINDINGS`
- Source raw note: RN-006
- Expected behavior: Snapshot analysis is a production reusable system that emits generic findings with severity, source paths, facts, and a short explanation capable of answering admin/paperwork pressure questions.
- Disallowed shallow implementation: private test-only analysis, scenario-specific string matching, or human-only prose without machine-readable finding codes and source paths.
- Failing-first test: `bundle://proof/SB09/transcripts/failing-first-snapshot-analysis-services.txt`
- Passing tests: `bundle://proof/SB09/transcripts/snapshot-analysis-probe-tests.txt`; `bundle://proof/SB09/transcripts/economy-tests.txt`; `bundle://proof/SB09/transcripts/economy-boundary-audit.txt`
- Changed source files: `economy://src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotAnalysis.cs`, `economy://src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotAnalyzers.cs`, `economy://tests/CanDoItAll.Economy.Tests/SimulationSnapshotAnalysisProbeTests.cs`
- Production assertions: `bundle://proof/SB09/source-assertions/snapshot-analysis-source-map.txt`; `bundle://proof/SB09/source-assertions/genericity-scan.txt`; `bundle://proof/SB09/source-assertions/anti-stub-scan.txt`
- Red-team negative case: production analysis files contain no shared-resource fixture terms while the shared-resource probe still gets an admin/paperwork explanation.
- Downstream dependency check: SB13 and SB14 may rely on generic analysis findings to explain probe pressure without scenario-specific analyzer code.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Analyzer fleet | Seven production analyzer classes | Analysis service | Each analyzer contributes generic facts and source paths from snapshot data. | Probe test asserts all required codes are present. |
| Human-readable summary | Analysis service summary builder | User-facing probe explanations | Summary combines generic facts into an admin/paperwork pressure answer. | Summary asserts include paperwork/admin/issue but exclude fixture terms. |
| Genericity guarantee | Source scan | Bundle gate | Production analysis remains scenario-neutral. | Genericity scan found no shared-resource fixture-specific terms. |

## Completed Validator Tokens

Shallow-pass trap: SB09 rejects prose-only snapshot analysis and test-only report helpers.

Adversarial negative proof: failing-first snapshot-analysis transcript records the missing production analysis service before implementation.

Semantic positive proof: snapshot analysis probe, full Economy tests, and boundary audit prove production analyzers emit generic source-backed findings.

Anti-stub audit: SB09 anti-stub source assertion confirms analyzer proof is not placeholder code.
