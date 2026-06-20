# Proof manifest SB16

Status: pass

Required proof: Performance warnings do not mutate economic validity; headless hard budget failures mark not-comparable.

Artifacts attached:
- `economy-sb16-performance-tests.txt` - focused Economy transcript, 3 passed after `dotnet build-server shutdown` cleared a compiler lock.
- `sb16-performance-budget-report.json` - multi-goods-elite performance report and comparability policy proof.
- `source-scan-performance-comparability.txt` - source scan for SB16 report writer, negative headless failure policy, browser-only warning policy, and readiness status assertions.
- `changed-file-hashes.txt` - SHA-256 hashes for performance/readiness proof source.
- `anti-stub-scan.txt` - anti-stub scan for performance/readiness proof source.

Result:
Pass. The multi-goods-elite large-profile budget report remains comparable with zero hard failures. Negative policy proof shows a headless budget overage produces `not-comparable`, while a browser-only overage remains comparable and marks only the performance observer band as `warning`.

Production Behavior Artifact Matrix:

| Behavior | Production artifact | Proof artifact |
| --- | --- | --- |
| Multi-goods-elite performance budget is measured across headless, visual, snapshot, and browser-settle proxy signals | `EconomyPerformanceProbeTests.MultiGoodsElitePerformanceBudgetProbe_WritesSb16Report` | `economy-sb16-performance-tests.txt`, `sb16-performance-budget-report.json` |
| Headless hard budget failure marks readiness not-comparable | `EconomyExperimentReadinessReporter`; `EconomyExperimentPerformanceBudgetEvaluator` | `source-scan-performance-comparability.txt`, `sb16-performance-budget-report.json` |
| Browser-only budget overage stays an observer warning and does not mutate economic validity/comparability | `EconomyPerformanceProbeTests.MultiGoodsElitePerformanceBudgetProbe_WritesSb16Report` | `sb16-performance-budget-report.json` |
