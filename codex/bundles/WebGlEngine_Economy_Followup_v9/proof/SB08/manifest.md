# Proof manifest - SB08

Status: completed

## Scope

SB08 closes the readiness evidence contract v3 gap in Economy. Runtime, UI, and oracle readiness bands now distinguish requested exercise from accepted exercise, and accepted exercise requires validator-passed evidence refs with kind, path, hash, and schema version. Research-ready status depends on validator-passed oracle and browser-observer evidence rather than booleans or evidence counts alone.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB08/transcripts/changed-file-hashes.txt`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EconomyExperimentEvidenceRef` | Evidence producers through `EconomyExperimentEvidenceRecord.Ref` and `FromCitation` | `EconomyExperimentEvidenceValidator` and serialized readiness reports | Created with kind/path/hash/schemaVersion and attached to runtime/UI/oracle evidence records | `ReadinessReportV3_BrokenEvidenceRefsBlockExerciseAndResearchReady` mutates runtime ref hash and proves validation failure |
| `evidenceValidatorsPassed` | `BuildExplicitBand` and `BuildBrowserObserverBand` | `ResearchReady`, root report metadata, readiness gates, tests | Set after evidence validation; true only when required evidence refs validate | Broken runtime ref keeps runtime and browser observer validator metadata false |
| `exerciseRequested` / `exercised` split | `BuildExplicitBand` | Readiness status, browser observer, confidence, tests | Requested flags are recorded, but accepted exercise is true only after validators pass | Boolean-only and broken-ref tests show requested exercise without accepted exercise |
| `validEvidenceCount` / `invalidEvidenceCount` | `EconomyExperimentEvidenceValidator` through `BuildExplicitBand` | Report metadata and diagnostics | Counts all supplied evidence records by strict ref validation | Broken-ref test asserts 0 valid and 1 invalid runtime evidence record |

## Proof Artifacts

- Readiness v3 and broken evidence tests: `bundle://proof/SB08/readiness-report-v3-tests.txt`
- Adjacent readiness/browser observer regression tests: `bundle://proof/SB08/readiness-adjacent-tests.txt`
- SimulationSandbox build: `bundle://proof/SB08/simulationsandbox-build.txt`
- Readiness evidence contract report: `bundle://proof/SB08/readiness-evidence-contract-report.md`
- Source assertions: `bundle://proof/SB08/transcripts/source-assertions.txt`
- Changed-file hashes: `bundle://proof/SB08/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB08/anti-stub-audit.txt`
- Bundle validator transcript: `bundle://proof/SB08/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB08/semantic-invariants.md`

## Closure

SB08 passes. The focused v3 readiness test run passed 2/2, including the required broken evidence negative test. Adjacent readiness/browser observer tests passed 6/6, confirming the stricter evidence exercise gate preserves the existing separation between headless/oracle correctness and browser observer evidence. `CanDoItAll.Economy.SimulationSandbox` builds with 0 warnings and 0 errors.
