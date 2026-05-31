# SB11 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB11-INV-001 | Positions, topology, and parameters live in versioned JSON inputs. | Keeping shared-well coordinates or constants hidden inside runtime code. | `PlacementAndParameters_LoadValidateHashAndApplyToScenario` |
| SB11-INV-002 | Placement/parameter changes participate in deterministic hashes. | Loading files but omitting them from input identity. | `PlacementAndParameters_LoadValidateHashAndApplyToScenario` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB11/manifest.md`.
