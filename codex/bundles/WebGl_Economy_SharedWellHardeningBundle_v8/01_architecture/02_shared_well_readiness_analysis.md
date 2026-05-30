# Shared-well readiness analysis

The shared-well community is a good acceptance scenario because it stresses both simulation and visualization without forcing domain logic into WebGL.

## Minimum scenario facts

- A shared water source exists at a location.
- Households have home locations at varying distances.
- Every household consumes water periodically.
- Some households are closer to the source.
- A close household can collect extra water and resell it to distant households.
- Resale creates rules, fees, paperwork, tax/admin actions, and potential conflict.
- The community has governance actors or rule institutions.
- Well stock and well health can change over time.

## Required simulation capabilities

- Place/location model.
- Actor home/work/source target anchors.
- Resource store with capacity and owner.
- Actor inventory/carry capacity.
- Distance/cost calculator.
- Event expansion from high-level intent:
  - `need.water.daily` -> `travel`, `collect/use`, `return`;
  - `resell.water` -> `collect`, `trade`, `admin`, `tax/fee`, `relationship-change`;
  - `rule.violation` -> `show conflict`, `admin`, `enforcement`.
- Rule evaluation that can produce additional events.
- Deterministic materialization.
- Scenario loader/serializer round-trip.

## Required visualization capabilities

- Stable actor/object bindings.
- Actor movement to target anchors.
- Return to home anchor.
- Pose/state change:
  - walking;
  - carrying;
  - admin-writing;
  - trading;
  - working/repairing.
- Symbols:
  - water;
  - document/admin;
  - conflict;
  - rule;
  - tax/fee;
  - scarcity.
- Sequenced actions that are not coalesced incorrectly.
- Batch application that preserves stage boundaries.

## Current gap level

Current implementation is approximately **60–70% ready for a static proof** and **35–45% ready for a generic scenario-run proof**.

The biggest missing pieces are not rendering; they are canonical scenario semantics, behavior/rule expansion, ordered action stages, and bridge-ready visual actions.
