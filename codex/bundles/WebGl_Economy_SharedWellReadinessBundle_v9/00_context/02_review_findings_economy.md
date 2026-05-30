# Economy review findings

## Good progress

- `Simulation.Abstractions` is split into Scenario, Events, Frame, Run, Hashing and Backends folders.
- `Simulation.SimpleAccounts` is split into scenario factories, materializer, backend, delta builder and consistency validator.
- `Simulation.Ledger` is split into backend, projector, validators and mappers.
- `Simulation.Visualization` has visual frame, visual action, mapping policy, layout and symbol-related contracts.
- Shared-well scenario now includes actors, locations, resources, stores, relationships and scheduled events.
- Frames now include simulation events, and deltas include added events.
- Tests cover scenario load/validate/materialize, event hash participation, visual actions and ledger delta behavior.

## Main concerns

1. Scenario definitions still contain alias pairs:
   `Entities` vs `Actors`, `Places` vs `Locations`, `Stores` vs `InitialStores`.
   This is useful for compatibility but dangerous unless normalized into one canonical model.

2. Events still contain alias/duplicate fields:
   `Kind` vs `EventKind`, `ActorId` vs `ActorIds`, `Quantity` vs `Magnitude`,
   `Duration` vs `Timing.Duration`, `PlaceId` vs `SourceLocationId`/`TargetLocationId`.
   Add a canonical event normalizer and conflict validator.

3. SimpleAccounts materializer still switches by exact scenario id.
   This is not generic enough. It should use a registry of materializers/rule handlers/event handlers.

4. Shared-well is still partly hardcoded as precomputed frames.
   The next step is not the visual demo, but a generic state transition engine:
   initial stores + event stream + rules -> frames/deltas.

5. Economy visual action mapping currently sorts by event id rather than event timing.
   This can break sequences. Sort by step, offset, scheduled order, then stable event id.

6. Visual actions include sequences and child actions together. Mark child actions as internal or flatten exactly once.
   Do not let a bridge execute both the sequence and its steps.
