# Economy review findings

## Good progress

Economy now has:
- `Simulation.Abstractions`;
- `Simulation.SimpleAccounts`;
- `Simulation.Ledger`;
- `Simulation.Visualization`.

Scenario definitions, event streams, validators, serializers, visual actions, and materializers exist.

## Key risks

### 1. Scenario definition contract is too broad and has aliases

`SimulationScenarioDefinition` has both `Entities` and `Actors`, both `Places` and `Locations`, both `Stores` and `InitialStores`. This is useful during migration, but it creates ambiguity.

The next wave should introduce a canonical normalized scenario definition and keep aliases as import compatibility only.

### 2. Event kind taxonomy is duplicated

`SimulationEventKind` contains both dotted canonical names and legacy/human short names. This is currently useful, but must be normalized through one registry:
- canonical `actor.resource.use`;
- alias `resource-use`.

Internal logic should use canonical names only after loading.

### 3. Event compiler is still basic

`SimulationScenarioEventCompiler` only compiles scheduled events. It does not yet expand behaviors/rules into derived events such as:
- walk/travel;
- collect/use resource;
- admin paperwork;
- tax/fee payment;
- rule check;
- rule violation;
- enforcement;
- trade.

### 4. Shared-well case is not fully represented

Current shared-well scenario includes travel, resource use, return, maintenance, violation, enforcement, and admin events. Missing or incomplete:

- distance-based travel duration/cost;
- actor inventory/carry capacity;
- local trading because one actor is closer;
- price formation or negotiated exchange;
- tax/fee/admin burden caused by trading;
- rule checks around resale/hoarding/fair use;
- repeated daily needs;
- scarcity feedback and well depletion/recharge;
- actor strategy/behavior definitions instead of hardcoded frames.

### 5. Visual action mapper can duplicate and misorder actions

`EconomyVisualActionMapper` currently creates sequences and sometimes also adds sequence steps as top-level actions. This can cause double application when bridged to WebGL.

It also sorts events by `EventId` instead of using `Timing.StepIndex`, `Timing.Offset`, and a stable event order.
