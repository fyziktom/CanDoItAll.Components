# Normalized requirements for follow-up bundle v6

- R01: Prove pause/stop semantics at browser runtime level, including motions, queued motions, stage queue, barriers and stale callbacks.
- R02: Add a runtime-idle/settled-state await contract for WebGlLib and WebGlRun playback.
- R03: Split Economy replay modes into absolute replay, incremental apply and snapshot-anchor replay.
- R04: Introduce strict economic experiment mode and fail on unapproved semantic warnings.
- R05: Replace implicit/heuristic store resolution with explicit resolution policies and ambiguity diagnostics.
- R06: Add a golden oracle economics test suite with known final stores, flows, issues, metrics and hashes.
- R07: Add typed metric and invariant registries; unknown metric/invariant kinds must fail preflight in strict mode.
- R08: Version and expose behavior expansion profiles as explicit scenario policy.
- R09: Separate scenario/simulation/projection/runtime/UI validity bands in a single readiness report.
- R10: Harden scenario pack manifests with per-file hashes, declared pack hash and tamper tests.
- R11: Add hard performance budgets for headless deterministic paths and separate browser/visual budgets.
- R12: Add experiment runner output suitable for comparing simulations without depending on UI.
- R13: Add proof integrity validator that rejects empty or non-substantive transcripts.
- R14: Update docs with confidence levels and "exploratory vs research-grade" guidance.
