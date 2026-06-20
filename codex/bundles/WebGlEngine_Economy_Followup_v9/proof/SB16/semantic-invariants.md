# SB16 Semantic Invariants

Status: completed

## Invariants

- Headless performance-budget overages must set `HardFailure=true`, make the report not comparable, and drive readiness to `not-comparable` when economic truth is otherwise valid.
- Browser and visual budget overages must be observer/runtime warnings only; they must not create headless hard failures.
- `multi-goods-elite` performance proof must measure materialization, projection, metric evaluation, snapshot build, snapshot serialization, and browser batch settle.
- Large-run stress proof must keep budget measurements machine-readable and include the stress shape used for comparison.
- Components WebGlRun playback budget proof must remain generic and domain-free.

## Boundary

SB16 changes Economy tests/proof only and runs an existing Components budget test. It does not modify Components runtime behavior, browser observer semantics, or visual mapping output.

## Proof

- `bundle://proof/SB16/performance-budget-report.json`
- `bundle://proof/SB16/large-run-stress-proof.json`
- `bundle://proof/SB16/webglrun-large-playback-budget-metrics.json`
