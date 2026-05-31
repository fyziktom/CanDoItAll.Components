# SB14 finite-resource source map

## Test proof

- `repo://tests/CanDoItAll.Economy.Tests/FiniteResourceMarketProbeTests.cs` proves the fixture is consumed through generic APIs only:
  - `SimulationExperimentInputPackLoader`
  - `SimpleSimulationStateTransitionEngine`
  - `SimulationFrameMetricEvaluator`
  - `SimulationInvariantEvaluator`
  - `SimulationSnapshotBuilder`
  - `SimulationSnapshotDiff`

## Semantic coverage

- Finite capacity: asserts the finite resource capacity and final resource total stay within the capacity.
- Actor expansion: asserts the expanding actor's store increases while the other actor's store decreases.
- External demand: asserts the external market actor, external cash store, and market-targeted scheduled demand.
- Concentration metric: asserts HHI and top-owner-share metrics exceed concentration thresholds.
- Anti-concentration rule: asserts the anti-concentration rule metadata and failed invariant.
- Fee/enforcement proof: asserts the compiled event stream emits the generic tax/fee event.
- Snapshot diff proof: compares production-built before/after snapshots and asserts resource store, metric, invariant, applied-event, and pending-event diffs.

## Production genericity boundary

- `bundle://proof/SB14/source-assertions/generic-production-forbidden-term-scan.txt` scans generic production layers only: simulation abstractions, visualization, WebGL bridge, and sandbox orchestration.
- Existing sample factories in `Simulation.SimpleAccounts` are intentionally outside this genericity scan because they contain older domain sample vocabulary and were not changed by SB14.
