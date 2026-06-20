# SB08 Semantic Invariants

## Invariants

- Runtime, UI, and oracle readiness bands must not mark `exercised` true from booleans alone.
- Evidence accepted for runtime, UI, and oracle bands must include an `EvidenceRef` with kind, path, strict lowercase `sha256:<64 hex>` hash, and schema version.
- Evidence refs must match the expected readiness band; mismatched or malformed refs fail validation.
- Browser observer evidence is valid only when both runtime and UI evidence validators pass.
- `ResearchReady` requires validator-passed oracle evidence and validator-passed browser-observer evidence.
- Broken visual/runtime evidence must not mutate headless economic truth or invalidate independently valid oracle proof.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EconomyExperimentEvidenceRef` | Evidence producers through `EconomyExperimentEvidenceRecord.Ref` and `FromCitation` | `EconomyExperimentEvidenceValidator` and serialized readiness reports | Created with kind/path/hash/schemaVersion and attached to runtime/UI/oracle evidence records | `ReadinessReportV3_BrokenEvidenceRefsBlockExerciseAndResearchReady` mutates runtime ref hash and proves validation failure |
| `evidenceValidatorsPassed` | `BuildExplicitBand` and `BuildBrowserObserverBand` | `ResearchReady`, root report metadata, readiness gates, tests | Set after evidence validation; true only when required evidence refs validate | Broken runtime ref keeps runtime and browser observer validator metadata false |
| `exerciseRequested` / `exercised` split | `BuildExplicitBand` | Readiness status, browser observer, confidence, tests | Requested flags are recorded, but accepted exercise is true only after validators pass | Boolean-only and broken-ref tests show requested exercise without accepted exercise |
| `validEvidenceCount` / `invalidEvidenceCount` | `EconomyExperimentEvidenceValidator` through `BuildExplicitBand` | Report metadata and diagnostics | Counts all supplied evidence records by strict ref validation | Broken-ref test asserts 0 valid and 1 invalid runtime evidence record |

## Proof Links

- `bundle://proof/SB08/readiness-report-v3-tests.txt`
- `bundle://proof/SB08/readiness-adjacent-tests.txt`
- `bundle://proof/SB08/simulationsandbox-build.txt`
- `bundle://proof/SB08/transcripts/source-assertions.txt`
- `bundle://proof/SB08/anti-stub-audit.txt`
