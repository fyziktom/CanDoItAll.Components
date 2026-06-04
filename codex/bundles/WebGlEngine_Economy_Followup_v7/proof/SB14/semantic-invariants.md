# Semantic Invariants - SB14

| Invariant | Implementation surface | Guardrail | Proof |
|---|---|---|---|
| Components performance budgets remain domain-neutral | `WebGlRuntimeBudgetProfiles`, WebGlLib diagnostics tests, WebGlLib README | Generic WebGL profiles describe runtime scale only; Economy terms such as market, ledger, policy, scenario, buyer, seller, or price do not enter Components budget code | `proof/SB14/transcripts/source-assertion-performance-budget-scan.txt` |
| Economy performance overages are comparability failures | `EconomyExperimentPerformanceBudgetEvaluator`, `EconomyExperimentReadinessReporter` | Headless budget failures set `HardFailureCount`, `Comparable = false`, and readiness `not-comparable` without changing scenario/simulation/metric validity bands to economic failures | `proof/SB14/transcripts/performance-budget-tests.txt` |
| Performance and economic gates are separate | `BuildPerformanceBudgetGate`, `BuildHeadlessGate`, `ResolveStatus` | Scenario, simulation, and metric failures decide economic model failure; performance budget failures are reported through `performance-budget` and do not masquerade as model defects | `proof/SB14/transcripts/performance-budget-tests.txt` |
| Large-run performance proof is actionable | `EconomyPerformanceProbeTests`, WebGlRun performance tests, generated SB14 report | Results carry profile id, hard/warning counts, top bottlenecks, memory deltas, and named measurements instead of warning-only prose | `proof/SB14/artifacts/performance-budget-report.json` |
| Browser proof includes idle/settled timing | SB14 browser proof script and RunPlayback runtime diagnostics | Browser involvement must include runtime idle status plus browser load and batch settle measurements before performance claims are interpreted | `proof/SB14/browser/performance-budget-browser-proof.json` |

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| Runtime profile id (`small`, `medium`, `large`, `stress`) | Components `WebGlRuntimeBudgetProfiles` and Economy `EconomyExperimentPerformanceProfiles` | Runtime diagnostics, headless runner, proof reports | Selected before measuring; carried into reports so budgets can be interpreted by run size and mode | `proof/SB14/transcripts/performance-budget-tests-failing-first.txt` captures missing profile support |
| Performance measurement categories | Economy budget evaluator and SB14 proof scripts | Readiness metadata, top bottleneck summaries, final report | Each materialization/projection/metric/snapshot/serialization/browser measurement is evaluated against a named budget and category | `proof/SB14/artifacts/performance-budget-report.json` |
| `Comparable` / `HardFailureCount` | `EconomyExperimentPerformanceBudgetReport` | Readiness status and `performance-budget` gate | Headless overages produce hard comparability failures; browser/visual overages are warning-class observer/runtime evidence | `proof/SB14/transcripts/performance-budget-tests.txt` covers hard failure classification |
