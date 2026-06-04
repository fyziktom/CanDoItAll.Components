# Proof manifest — SB06

Status: completed

Required artifacts:
- `proof/SB06/transcripts/store-resolution-policy-tests.txt`
- `proof/SB06/artifacts/flow-resolution-sample.json`

Additional proof:
- `proof/SB06/transcripts/store-resolution-policy-tests-failing-first.txt`
- `proof/SB06/transcripts/store-resolution-hardening-tests.txt`
- `proof/SB06/transcripts/flow-resolution-sample-export.txt`

Closure summary:
- Explicit store-resolution policies now flow into per-flow metadata for source/target decisions.
- Research strict transfers fail ambiguous store selection instead of using incidental ordering.
- Rejected transfers retain resolver metadata and emit `zero-accepted-transfer` when no quantity is accepted.
- The flow-resolution sample contains both accepted and capacity-rejected flows with resolution metadata.
