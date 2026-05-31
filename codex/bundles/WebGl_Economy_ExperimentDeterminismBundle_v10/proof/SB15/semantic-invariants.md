# SB15 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB15-INV-001 | Generic contracts can express finite spatial resource ownership, expansion/investment, external demand, rule constraints, and concentration metrics. | Overfitting the new model to shared-well only. | `FarmerLandProbe_ValidatesGenericContractsWithoutBuildingFullSimulation` |
| SB15-INV-002 | Missing runtime capabilities remain explicit follow-up surface, not hidden in fixture-specific code. | Building a one-off farmer-land simulation branch. | `FarmerLandProbe_ValidatesGenericContractsWithoutBuildingFullSimulation` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB15/manifest.md`.
