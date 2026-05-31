# SB12 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB12-INV-001 | Randomness is allowed only before the run and is recorded with generator id/version/seed/hash metadata. | Calling random APIs inside transition or playback execution. | `RandomInputGenerator_WritesReplayablePlacementJson` |
| SB12-INV-002 | The same saved generated JSON produces the same deterministic hash. | Replaying from the generator instead of the file. | `RandomInputGenerator_WritesReplayablePlacementJson` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB12/manifest.md`.
