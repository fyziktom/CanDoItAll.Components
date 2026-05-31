# SB18 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB18-INV-001 | Components proof covers 1000 command batch items and staged repeated-motion safety. | Passing only tiny fixtures while claiming scale readiness. | `Batch_normalizer_handles_1000_patch_motion_items_with_deterministic_metrics`; `npm run webgllib:audit-sharedwell-performance` |
| SB18-INV-002 | Economy proof covers 100 actors x 50 scheduled steps and records counts/durations as warnings/metrics. | Micro-optimizing instead of measuring bottlenecks. | `EconomyPerformanceProof_CompilesMaterializesAndMapsLargeGenericSharedResourceScenario` |
| SB18-INV-003 | Large-screen-only policy remains intact. | Adding responsive/mobile optimization proof. | `npm run webgllib:audit-scene-runtime` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB18/manifest.md`.
