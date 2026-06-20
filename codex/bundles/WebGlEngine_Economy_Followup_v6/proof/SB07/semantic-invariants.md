# Semantic Invariants for SB07

## Invariant SB07-metrics-and-invariants-use-registered-kinds

Source: metric evaluator, invariant evaluator, and input-pack loader.

Expected behavior: strict runs reject unknown metric and invariant kinds and reject invariant references to missing metric ids.

Passing result: `MetricAndInvariantRegistry_RejectsUnknownKindsAndMissingMetricInStrictMode` passed in the focused suite.

Why this prevents simulator-noise contamination: unsupported measurements cannot quietly fall back to zero or pass while economic conclusions depend on them.

