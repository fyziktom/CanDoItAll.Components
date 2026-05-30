# Shared-well readiness gap analysis

This is not a request to implement the demo. It is a readiness probe.

## What is already present

- Actors/locations/resources/stores for the shared-well scenario.
- Scheduled events for travel, water use, rule violation, maintenance, enforcement, administration.
- Frames include events.
- Visual actions include move/return/sequence/show-symbol/change-state style concepts.
- WebGlRun actions include move-to-object, return-to-anchor, set-pose and show-symbol style concepts.

## What still blocks a clean generic demo

1. Behavior-driven event generation:
   The scenario still needs manual scheduled events. We need generic behavior/rule expansion.

2. State transition:
   Stores are still mostly authored per frame. We need event application to update stores, inventory, issues and relationships.

3. Distance:
   Locations exist, but no generic travel duration/cost policy exists yet.

4. Inventory/carry capacity:
   A closer household cannot yet stockpile water generically.

5. Trade/resale:
   There is no generic offer/sell/buy/tax/admin burden model.

6. Rule enforcement:
   Rule violation and enforcement events exist, but not as generic consequences of policy evaluation.

7. Visual stage semantics:
   The action sequence must preserve ordering and multiple motions for the same actor.

8. Binding:
   Target locations/places should resolve explicitly, not by accidental resource-node matching.

## Minimal demo readiness target

Before building the demo, the generic pipeline should support:

```text
actor has home location
well has resource source location
actor has inventory capacity
resource need event is expanded into travel/use/return
near actor can collect surplus
trade event transfers water/tokens
tax/admin event follows trade
visual action sequence preserves order
WebGL action stages keep multiple motions for one actor
```
