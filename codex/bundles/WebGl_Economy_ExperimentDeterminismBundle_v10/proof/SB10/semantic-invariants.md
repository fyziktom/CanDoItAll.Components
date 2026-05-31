# SB10 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB10-INV-001 | Typed source, target, and participant refs coexist with string compatibility fields. | Replacing strings in a breaking way or ignoring typed refs in hashes. | `TypedEventRefs_DistinguishActorWaterFromResourceWater` |
| SB10-INV-002 | New canonical event names normalize through one registry while old names remain aliases. | Duplicating event-kind vocabularies in new logic. | `EventNormalizer_MapsLegacyAliasesAndDetectsConflictsWithoutMutatingSource` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB10/manifest.md`.
