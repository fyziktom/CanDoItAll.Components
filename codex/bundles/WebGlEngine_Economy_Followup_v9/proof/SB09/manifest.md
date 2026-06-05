# Proof manifest - SB09

Status: completed

## Scope

SB09 closes the external golden oracle corpus gap in Economy. Current golden oracle cases now run through a production `EconomyGoldenOracleRunner` that loads an external corpus, evaluates final stores, flows, issues, relationships, metrics, invariants, diagnostics, and deterministic frame hash chains, and reports path-addressed diffs for broken expected values.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB09/transcripts/changed-file-hashes.txt`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyGoldenOracleCorpus.cs`

Economy tests and corpus:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/GoldenOracles/economic-oracles.json`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EconomyGoldenOracleCorpus` | External JSON corpus loader | `EconomyGoldenOracleRunner` and tests | Loaded from `economic-oracles.json` with schema `economy-golden-oracle-corpus/v1` | Missing scenario keys produce `scenarios.<key>` diffs |
| `EconomyGoldenOracleReport` | `EconomyGoldenOracleRunner.Run` | Tests, proof report, future readiness/oracle consumers | Summarizes case counts, pass/fail counts, flattened diffs, and per-case hash chains | Corrupted expected metric makes report fail with a path-addressed diff |
| `EconomyGoldenOracleDiff.Path` | Store/flow/issue/relationship/metric/invariant/diagnostic/hash comparisons | Tests and proof JSON | Uses stable paths such as `metrics.resourceTotal` and `stores.<id>.quantity` | `negative-diff-proof.json` records `metrics.resourceTotal` expected `999` vs actual `10` |
| `FrameHashChain` / `ReplayFrameHashChain` | `SimulationDeterministicHash.HashFrame` over primary and replay runs | Golden oracle report and tests | Recorded for each oracle case and compared for deterministic replay and external expected chain equality | Any mismatch emits `frameHashChain` or `frameHashChain.expected` diff |

## Proof Artifacts

- External corpus: `bundle://proof/SB09/oracle-corpus/economic-oracles.json`
- Oracle report: `bundle://proof/SB09/oracle-report.json`
- Negative diff proof: `bundle://proof/SB09/negative-diff-proof.json`
- Oracle corpus tests: `bundle://proof/SB09/oracle-corpus-tests.txt`
- SimulationSandbox build: `bundle://proof/SB09/simulationsandbox-build.txt`
- Source assertions: `bundle://proof/SB09/transcripts/source-assertions.txt`
- Changed-file hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`
- Bundle validator transcript: `bundle://proof/SB09/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB09/semantic-invariants.md`

## Closure

SB09 passes. The GoldenOracleSuite test run passed 3/3, covering positive external corpus validation, a broken expected-value diff at `metrics.resourceTotal`, and negative scenario diagnostics. `CanDoItAll.Economy.SimulationSandbox` builds with 0 warnings and 0 errors.
