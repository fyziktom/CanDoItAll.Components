# Proof manifest - SB07

Status: completed

Required artifacts:
- `proof/SB07/transcripts/metric-invariant-registry-tests.txt` - passed focused strict registry/provenance test.
- `proof/SB07/artifacts/metric-oracle-results.json` - generated from the real evaluator with seven oracle metrics and strict failure examples.

Additional proof:
- `proof/SB07/transcripts/metric-invariant-registry-tests-failing-first.txt` - failing-first precision/provenance test before implementation.
- `proof/SB07/transcripts/metric-invariant-registry-hardening-tests.txt` - passed `SimulationTransitionAndMetricHardeningTests`.
- `proof/SB07/transcripts/metric-invariant-registry-economic-trust-tests.txt` - passed `SimulationEconomicTrustHardeningTests`.
- `proof/SB07/transcripts/metric-oracle-results-export.txt` - oracle artifact export transcript.

Implemented:
- Explicit metric descriptors with default unit, precision, and required metadata keys.
- Explicit invariant descriptors with required metadata and one-of metadata groups.
- Strict/research errors for unknown or underspecified metric/invariant kinds.
- Deterministic metric rounding and result provenance (`kind`, `unit`, `precision`, `sourceFrameHash`, `evaluatorVersion`).
