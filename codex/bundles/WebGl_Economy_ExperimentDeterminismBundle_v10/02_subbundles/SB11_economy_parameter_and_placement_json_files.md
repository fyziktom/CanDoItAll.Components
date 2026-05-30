# SB11 — Economy: parameter and placement JSON files

## Problem

Locations and parameters must be versioned as experiment inputs, not hidden constants.

## Tasks

1. Add `SimulationPlacementDefinition`:
   - actor positions
   - object/resource-site positions
   - store positions
   - topology edges
   - optional generated-by metadata
2. Add `SimulationParameterSetDefinition`:
   - actor needs
   - carry capacity
   - travel speed/cost
   - storage capacity
   - resale markup
   - tax/fee rates
   - admin time/cost
   - external demand
   - rule enforcement thresholds
3. Add JSON serializer/loader/validator.
4. Add sample JSON under test fixtures, not production hardcoded constants.

## Done criteria

- Shared-well placement and parameters can be saved, loaded, hashed.
- Simulation consumes loaded inputs, not hidden random state.
