# Semantic Invariants for SB10

## Invariant SB10-readiness-bands-do-not-cross-contaminate

Source: `EconomyExperimentReadinessReporter`.

Expected behavior: model bands decide economic validity, projection/runtime/UI bands report visualization readiness, and semantic warnings prevent L4/L5 confidence.

Passing result: `ReadinessReport_L4HeadlessBrowserFailureRuntimeOnlyAndSemanticWarningsPreventL4` passed, and the Economy sandbox build passed with zero warnings and zero errors.

Why this prevents simulator-noise contamination: browser failures are no longer misreported as economic model failures, while model warnings still block serious-confidence levels.

