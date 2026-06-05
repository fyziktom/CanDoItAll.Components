# Proof manifest - SB10

Status: completed

## Scope

SB10 adds a third Economy experiment input pack for multi-goods exchange, elite capital funding, equity-like claims, policy shock fees, unmet demand, and elite-capital dependency. The pack is intentionally structurally different from `shared-well` and `farmer-land`, and is consumed by the existing strict loader, simple-accounts backend, metric/invariant evaluators, and headless readiness runner.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB10/transcripts/changed-file-hashes.txt`

Economy scenario fixture:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/experiment.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/scenario.definition.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/expected.invariants.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/parameters.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/placement.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/institution.rules.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/run.plan.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/visual.mapping.json`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputPackStrictModeTests.cs`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `experiment.multi-goods-elite.exchange-investment.v1` pack | `multi-goods-elite/experiment.json` plus companion documents | `SimulationExperimentInputPackLoader`, strict pack validator, headless runner | Document hashes are computed with `SimulationExperimentInputPackValidator.HashFile`; canonical pack hash is `sha256:37be238a1d086d4c5fbf07c00c40ad3e89869b54da0a4591f81e204266ff77d9`; copied to `proof/SB10/scenario-pack/` | `scenario-pack-tamper-tests.txt` passes document tamper and pack metadata tamper rejection |
| Generic exchange/investment/policy events | `scenario.definition.json` scheduled events using `actor.trade.sell`, `actor.resource.transfer`, `actor.tax-fee.pay`, `rule.violation.detect`, `relationship.trust.change` | Simple-accounts handlers and event compiler | Final-step ordered events produce frame flows/issues/relationships without scenario-specific handlers | Headless metric assertions fail if resource flows disappear; tamper tests fail stale hashes |
| Six third-scenario metrics | `expected.invariants.json` metric definitions | `SimulationFrameMetricEvaluator`, invariant evaluator, `metrics-invariants.json`, tests | Emits HHI, top wealth share, equity-share HHI proxy, liquidity, unmet demand, and elite-capital dependency from the preserved headless run | `MultiGoodsEliteScenarioPack_LoadsRunsAndReportsReadinessMetrics` asserts all six metric values and readiness status |
| Headless readiness report | `EconomyHeadlessExperimentRunner` | `readiness-report.json`, `headless-run-manifest.json`, proof manifest | Run status is `headless-valid`; artifacts include event stream, frames, hashes, metrics, warnings, summary, readiness, manifest | Missing/invalid pack hashes or missing final-frame flows cause the test or runner to fail |

## Proof Artifacts

- Scenario pack copy: `bundle://proof/SB10/scenario-pack/multi-goods-elite/`
- Preserved headless output: `bundle://proof/SB10/headless-run/`
- Headless transcript: `bundle://proof/SB10/headless-run.txt`
- Explicit gaps list: `bundle://proof/SB10/gaps-list.md`
- Metrics summary: `bundle://proof/SB10/metrics-summary.json`
- Scenario pack and headless tests: `bundle://proof/SB10/scenario-pack-headless-tests.txt`
- Strict tamper tests: `bundle://proof/SB10/scenario-pack-tamper-tests.txt`
- SimulationSandbox build: `bundle://proof/SB10/simulationsandbox-build.txt`
- Source assertions: `bundle://proof/SB10/transcripts/source-assertions.txt`
- Changed-file hashes: `bundle://proof/SB10/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB10/transcripts/anti-stub-audit.txt`
- Bundle validator transcript: `bundle://proof/SB10/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB10/semantic-invariants.md`

## Closure

SB10 passes. The committed `multi-goods-elite` pack validates under strict hash checks, the headless runner emits a `headless-valid` readiness report, and the metrics artifact records the required six signals: HHI `0.330774`, top wealth share `0.489362`, Gini-like equity concentration proxy `0.6250`, liquidity volume `14`, unmet demand `1`, and elite capital dependency `71`. The readiness projection band records bridge-bound visual mapping diagnostics; this is explicitly listed in `gaps-list.md` and remains owned by SB12 visualization genericity.
