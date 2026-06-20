# Proof manifest — SB04

Status: completed

Required artifacts:
- `proof/SB04/transcripts/readiness-v2-tests.txt`
- `proof/SB04/artifacts/readiness-v2-sample.json`

Additional proof:
- `proof/SB04/transcripts/readiness-v2-tests-failing-first.txt`
- `proof/SB04/transcripts/readiness-v2-hardening-tests.txt`
- `proof/SB04/transcripts/readiness-v2-sample-export.txt`

Closure summary:
- Economy readiness reports now emit schema `economy-experiment-readiness/v2`.
- Reports include machine-readable `status`, `researchReady`, oracle and browser-observer bands, hard-gate results, and warning-budget results.
- `researchReady` is false unless headless economic gates, oracle proof, browser observer proof, and zero-warning budget all pass.
- Warning rollups count unique warning messages so allowlist and budget decisions are evidence-based.
