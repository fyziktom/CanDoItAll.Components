# SB13 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB13-INV-001 | Generic event handlers update actors, stores, issues, relationships, obligations, and metrics through ordered events. | Using only seeded frame factories as proof. | `SimpleStateTransitionEngine_AppliesCollectTradeFeeAndRuleEvents` |
| SB13-INV-002 | Shared store resolution consumes the source location/store before actor-local credited state is updated. | Crediting actors without reducing the shared source. | `SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB13/manifest.md`.
