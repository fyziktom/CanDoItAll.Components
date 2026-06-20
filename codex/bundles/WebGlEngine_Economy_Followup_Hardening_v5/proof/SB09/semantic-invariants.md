# Semantic invariants SB09

Status: completed

## Invariants

- SB09-I1: Large-run performance proof must be machine-readable JSON with schema, workload dimensions, elapsed metrics, allocation metrics, and per-budget pass/fail fields.
- SB09-I2: The stress workload must stay generic: object, frame, stage, motion, patch, reset/recreate, and batching behavior only; no Economy/domain semantics.
- SB09-I3: Budget thresholds must be executable assertions, not prose; a regression must fail the xUnit test.
- SB09-I4: Resource/cache/dispose coverage must remain connected to typed WebGlLib diagnostics and proof snapshot contracts.
- SB09-I5: Browser proof is not required for SB09 because no UI/runtime production code changed; C# tests and diagnostics scans cover the new harness.

## Artifact Matrix

| Invariant | Artifact | Proof |
| --- | --- | --- |
| SB09-I1 | `webglrun-performance-budget/v1` JSON metrics | `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json` |
| SB09-I2 | Generic test source and domain-boundary scan | `bundle://proof/SB09/transcripts/domain-boundary-performance-budget-scan.txt` |
| SB09-I3 | xUnit budget assertions | `bundle://proof/SB09/transcripts/webglrun-performance-budget-tests.txt`; `bundle://proof/SB09/transcripts/source-assertion-performance-budget-scan.txt` |
| SB09-I4 | WebGlLib diagnostics tests and resource scan | `bundle://proof/SB09/transcripts/webgllib-resource-diagnostics-tests.txt`; `bundle://proof/SB09/transcripts/resource-cache-dispose-diagnostics-scan.txt` |
| SB09-I5 | Components build and no UI source edits for SB09 | `bundle://proof/SB09/transcripts/components-build-after-performance-budget.txt`; `bundle://proof/SB09/transcripts/components-sb09-diff-check.txt` |

## Metrics Snapshot

- Workload: 500 objects, 120 frames, 4 stages per frame, 8 motions per stage, 2 recreate iterations.
- Observed: 240 applied frames, 960 stages, 7680 motions, 960 patches, 8400 interop calls avoided.
- Budget result: all budgets passed in `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json`.
