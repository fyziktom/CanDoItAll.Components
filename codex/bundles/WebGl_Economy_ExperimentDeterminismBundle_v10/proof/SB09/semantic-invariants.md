# SB09 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB09-INV-001 | Actors, locations, and initial stores are the canonical source of truth. | Planning against legacy `Entities`, `Places`, or `Stores` as independent state. | `ScenarioNormalizer_UsesCanonicalCollectionsAndReportsAliasConflicts` |
| SB09-INV-002 | Canonical hash uses normalized content and stable ordering. | Hash changes caused by alias collection ordering. | `DeterministicHashes_IgnoreUiPlaybackSpeedAndCollectionOrder` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB09/manifest.md`.
