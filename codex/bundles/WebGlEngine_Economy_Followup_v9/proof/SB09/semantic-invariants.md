# SB09 Semantic Invariants

## Invariants

- Golden oracle expectations must live in an external JSON corpus, not only inline test code.
- The oracle runner must compare final stores, flow count, issue count, relationships, metrics, invariants, diagnostics, deterministic replay hash chain, and expected external frame hash chain.
- All oracle mismatches must produce stable path-addressed diffs.
- A broken expected value must fail the oracle report and preserve the expected/actual values.
- Positive oracle corpus cases must prove deterministic primary and replay frame hash chains.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EconomyGoldenOracleCorpus` | External JSON corpus loader | `EconomyGoldenOracleRunner` and tests | Loaded from `economic-oracles.json` with schema `economy-golden-oracle-corpus/v1` | Missing scenario keys produce `scenarios.<key>` diffs |
| `EconomyGoldenOracleReport` | `EconomyGoldenOracleRunner.Run` | Tests, proof report, future readiness/oracle consumers | Summarizes case counts, pass/fail counts, flattened diffs, and per-case hash chains | Corrupted expected metric makes report fail with a path-addressed diff |
| `EconomyGoldenOracleDiff.Path` | Store/flow/issue/relationship/metric/invariant/diagnostic/hash comparisons | Tests and proof JSON | Uses stable paths such as `metrics.resourceTotal` and `stores.<id>.quantity` | `negative-diff-proof.json` records `metrics.resourceTotal` expected `999` vs actual `10` |
| `FrameHashChain` / `ReplayFrameHashChain` | `SimulationDeterministicHash.HashFrame` over primary and replay runs | Golden oracle report and tests | Recorded for each oracle case and compared for deterministic replay and external expected chain equality | Any mismatch emits `frameHashChain` or `frameHashChain.expected` diff |

## Proof Links

- `bundle://proof/SB09/oracle-corpus/economic-oracles.json`
- `bundle://proof/SB09/oracle-report.json`
- `bundle://proof/SB09/negative-diff-proof.json`
- `bundle://proof/SB09/oracle-corpus-tests.txt`
- `bundle://proof/SB09/simulationsandbox-build.txt`
- `bundle://proof/SB09/transcripts/source-assertions.txt`
