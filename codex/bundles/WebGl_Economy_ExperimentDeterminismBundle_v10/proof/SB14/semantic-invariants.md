# SB14 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB14-INV-001 | Shared-well readiness is proven through input pack, event stream, transition frames/deltas, visual actions, and stable hashes. | Treating a seeded UI/demo screenshot as readiness proof. | `SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` |
| SB14-INV-002 | The readiness flow checks travel cost, stock movement, admin/tax, issue/enforcement, and move/use/admin/return visual ordering. | Only loading JSON without semantic assertions. | `SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB14/manifest.md`.
