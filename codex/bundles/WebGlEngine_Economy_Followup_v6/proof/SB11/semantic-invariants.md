# Semantic Invariants for SB11

## Invariant SB11-headless-budgets-are-hard

Source: `EconomyExperimentPerformanceBudgetEvaluator` and readiness performance band.

Expected behavior: headless deterministic budget overruns are hard failures, while visual/browser overruns are warnings unless explicitly classified as headless.

Passing result: `PerformanceBudgets_HeadlessFailuresAreHardAndVisualFailuresInReadiness` passed in the focused suite.

Why this prevents simulator-noise contamination: expensive or unstable headless model paths cannot be hidden behind warning-only performance observations.

