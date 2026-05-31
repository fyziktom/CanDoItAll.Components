# SB16 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB16-INV-001 | Visual actions are stable, ordered, deduplicated, and canonicalized before mapper output. | Emitting nested sequence steps again as top-level actions. | `VisualActionNormalizer_RemovesNestedChildrenUnlessMarkedStandalone` |
| SB16-INV-002 | Shared-well visual readiness includes move/use/admin/return sequencing without bridge implementation. | Adding direct Components references from Economy. | `SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB16/manifest.md`.
