# SB07 - Economy: canonical scenario definition normalizer

## Problem
The scenario definition contains aliases:
`Actors`/`Entities`, `Locations`/`Places`, `Stores`/`InitialStores`.

## Tasks
- Add `SimulationScenarioDefinitionNormalizer`.
- It must not mutate the input instance.
- It should produce canonical collections and copy aliases deterministically.
- Detect conflicts between aliases.
- Use normalizer before validation, hashing, event compiling and materialization.

## Tests
- Alias-only definition becomes valid canonical definition.
- Conflicting actor/entity names produce validation warning/error.
- Hashing is stable before/after normalization.
