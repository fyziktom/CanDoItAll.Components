# Proof manifest — SB05

Status: completed

Required artifacts:
- `proof/SB05/transcripts/research-strict-mode-tests.txt`
- `proof/SB05/artifacts/strict-mode-failure-examples.json`

Additional proof:
- `proof/SB05/transcripts/research-strict-mode-tests-failing-first.txt`
- `proof/SB05/transcripts/research-strict-hardening-tests.txt`
- `proof/SB05/transcripts/strict-mode-failure-examples-export.txt`

Closure summary:
- Added `SimulationExperimentPolicy.ResearchStrict` as the first-class policy for research claims.
- Threaded policy through input loading, SimpleAccounts backend materialization, headless runner options, readiness metadata, warning budget, and `researchReady` gating.
- Kept `SimulationExperimentPolicy.ExploratoryDemo` available while ensuring demo/permissive readiness remains `exploratory` and not research-ready.
- Captured concrete strict failure examples for unknown handlers, insufficient stock, ambiguous stores, unknown metrics/invariants, and permissive demo status.
