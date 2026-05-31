# Semantic invariants SB08

Status: Completed

- Invariant ID: `SB08-SNAPSHOT-BUILDER-DATA-HASH`
- Source raw note: RN-006
- Expected behavior: Snapshot creation is available through production builder contracts, produces deterministic snapshots with provenance, and keeps the data hash stable when only runtime diagnostics change.
- Disallowed shallow implementation: keeping direct test-only snapshot construction, or letting volatile runtime diagnostics define the data hash used for downstream analysis/storage.
- Failing-first test: `bundle://proof/SB08/transcripts/failing-first-snapshot-builder-contracts.txt`
- Passing tests: `bundle://proof/SB08/transcripts/simulation-snapshot-tests.txt`; `bundle://proof/SB08/transcripts/economy-tests.txt`; `bundle://proof/SB08/transcripts/economy-boundary-audit.txt`
- Changed source files: `economy://src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotBuilder.cs`, `economy://tests/CanDoItAll.Economy.Tests/SimulationSnapshotBuilderTests.cs`
- Production assertions: `bundle://proof/SB08/source-assertions/snapshot-builder-source-map.txt`; `bundle://proof/SB08/source-assertions/anti-stub-scan.txt`
- Red-team negative case: runtime diagnostics change the full deterministic snapshot hash but not `ProvenanceHashes["snapshot.data"]`.
- Downstream dependency check: SB09, SB10, SB12, SB13, and SB14 may build on reusable snapshot creation and stable data provenance.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Build request/result contracts | Snapshot builder service | Snapshot pipelines and tests | Carries scenario, run, frame, delta, event, metric, invariant, visual, and runtime inputs into a validated result. | Contract scan and source assertions. |
| `snapshot.data` provenance hash | Provenance builder | Analysis/store/probe phases | Built from data-only snapshot content and excludes runtime diagnostics. | Runtime-diagnostics stability test. |
| Optional runtime attachment | Snapshot builder | Visual/runtime consumers | Runtime diagnostics are copied into optional `SimulationSnapshotVisualState`; no WebGL reference is required. | Finite-resource market positive test. |

## Completed Validator Tokens

Shallow-pass trap: SB08 rejects test-local snapshot helpers as proof of production readiness.

Adversarial negative proof: failing-first evidence records the missing reusable snapshot builder before implementation.

Semantic positive proof: snapshot builder tests and full Economy tests prove production builder output, provenance hashes, and stable snapshot data hashes.

Anti-stub audit: SB08 anti-stub source assertion confirms the builder proof is not placeholder code.
