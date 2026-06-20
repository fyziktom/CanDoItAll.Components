# Economic simulation readiness assessment

## Can we run simulations now?

Yes, for:

- exploratory scenario authoring,
- UI/visualization smoke tests,
- proving that the WebGL bridge can project frames,
- debugging scenario manifests and pipeline composition,
- generating preliminary performance observations,
- small demo experiments where outcomes are inspected manually.

## Should we trust simulation outcomes as economic conclusions now?

Not yet.

The current system is close enough to run scenarios, but not yet close enough to claim that a bad outcome is caused by the economic model rather than simulator noise. The main risk is not one giant bug; it is the accumulation of implicit policy choices and warning-level failures.

## Minimum quality bar before serious experiments

A run should be considered scientifically usable only when it produces an `ExperimentReadinessReport` with these independent pass bands:

1. `scenarioValidity = pass`
2. `semanticValidity = pass`
3. `engineDeterminism = pass`
4. `oracleCoverage = pass`
5. `metricInvariantValidity = pass`
6. `projectionValidity = pass`
7. `runtimeVisualizationValidity = pass or skipped`
8. `performanceBudget = pass`
9. `warnings = zero or explicitly allowed`
10. `provenance = complete`

If any model-adjacent warning exists, the run should be marked "exploratory only".

## Proposed run confidence levels

| Level | Meaning | Allowed use |
| --- | --- | --- |
| L0 Smoke | Code did not crash | Dev only |
| L1 Visual demo | UI and WebGL show movement | Demo only |
| L2 Pipeline valid | Scenario loads and projects | Scenario authoring |
| L3 Deterministic run | Same input produces same hashes | Internal experiments |
| L4 Oracle-backed | Known-answer tests pass for model primitives | Economic comparison |
| L5 Research-grade | Sensitivity, provenance, strict semantics and budgets pass | Serious conclusions |

The current system is roughly L2-L3 for many simple flows, with some L4 building blocks but not a complete L4 gate.
