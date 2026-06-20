# SB10 Semantic Invariants

## Invariants

- The third scenario pack must be structurally distinct from shared-well and farmer-land by including multiple goods categories, producers, a consumer household, an elite investor, and a policy institution.
- Exchange, capital funding, equity-like claims, fees, unmet demand, and dependency must be represented with generic registered event kinds instead of scenario-specific handlers.
- Metric-producing events must be present in the final headless frame, because the current metric artifact evaluates final-frame flows and stores.
- The pack must carry real document hashes and a canonical input-pack hash; placeholder hashes are not acceptable.
- The headless run must produce a `headless-valid` readiness report and preserve the emitted metrics/invariants artifact.
- The six requested scenario metrics must be emitted with non-zero or bounded values: HHI, top wealth share, Gini-like proxy, liquidity, unmet demand, and elite capital dependency.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `experiment.multi-goods-elite.exchange-investment.v1` pack | `multi-goods-elite/experiment.json` plus companion documents | `SimulationExperimentInputPackLoader`, strict pack validator, headless runner | Document hashes are computed with `SimulationExperimentInputPackValidator.HashFile`; canonical pack hash is `sha256:37be238a1d086d4c5fbf07c00c40ad3e89869b54da0a4591f81e204266ff77d9`; copied to `proof/SB10/scenario-pack/` | `scenario-pack-tamper-tests.txt` passes document tamper and pack metadata tamper rejection |
| Generic exchange/investment/policy events | `scenario.definition.json` scheduled events using `actor.trade.sell`, `actor.resource.transfer`, `actor.tax-fee.pay`, `rule.violation.detect`, `relationship.trust.change` | Simple-accounts handlers and event compiler | Final-step ordered events produce frame flows/issues/relationships without scenario-specific handlers | Headless metric assertions fail if resource flows disappear; tamper tests fail stale hashes |
| Six third-scenario metrics | `expected.invariants.json` metric definitions | `SimulationFrameMetricEvaluator`, invariant evaluator, `metrics-invariants.json`, tests | Emits HHI, top wealth share, equity-share HHI proxy, liquidity, unmet demand, and elite-capital dependency from the preserved headless run | `MultiGoodsEliteScenarioPack_LoadsRunsAndReportsReadinessMetrics` asserts all six metric values and readiness status |
| Headless readiness report | `EconomyHeadlessExperimentRunner` | `readiness-report.json`, `headless-run-manifest.json`, proof manifest | Run status is `headless-valid`; artifacts include event stream, frames, hashes, metrics, warnings, summary, readiness, manifest | Missing/invalid pack hashes or missing final-frame flows cause the test or runner to fail |

## Proof Links

- `bundle://proof/SB10/scenario-pack/multi-goods-elite/`
- `bundle://proof/SB10/headless-run/readiness-report.json`
- `bundle://proof/SB10/headless-run/metrics-invariants.json`
- `bundle://proof/SB10/gaps-list.md`
- `bundle://proof/SB10/metrics-summary.json`
- `bundle://proof/SB10/scenario-pack-headless-tests.txt`
- `bundle://proof/SB10/scenario-pack-tamper-tests.txt`
- `bundle://proof/SB10/simulationsandbox-build.txt`
- `bundle://proof/SB10/transcripts/source-assertions.txt`
