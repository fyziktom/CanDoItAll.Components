# SB09 — Economy: canonical scenario model and alias cleanup

## Problem

`SimulationScenarioDefinition` currently has alias collections such as `Entities/Actors`, `Places/Locations`, `Stores/InitialStores`. This can drift.

## Tasks

1. Decide canonical collections.
2. Mark alias collections as compatibility-only or move aliases into adapter DTOs.
3. Ensure normalizer:
   - fills aliases from canonical data only
   - detects divergent alias values
   - emits warnings or errors
4. Hash only canonical normalized content.
5. Add migration/round-trip tests.

## Done criteria

- There is one source of truth for actors/locations/stores.
- Hash cannot silently change due to alias ordering or duplicate state.
