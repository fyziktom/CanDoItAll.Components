# SB05 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB05-INV-001 | C# and JS normalizers agree on shared fixtures. | Separate implementations drift while local tests pass. | `Batch_normalizer_matches_shared_fixture_expectations`; `npm run webgllib:audit-command-batch-parity` |
| SB05-INV-002 | 1000-item command batches retain deterministic metrics. | Small fixtures hide performance bottlenecks. | `Batch_normalizer_handles_1000_patch_motion_items_with_deterministic_metrics`; performance audit |

## Production Behavior Artifact Matrix

See `bundle://proof/SB05/manifest.md`.
