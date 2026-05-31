# SB09 - Economy Generic Model Leakage Audit

## Problem

Some example-specific concepts can still leak into generic abstractions through class names, properties, metadata conventions, or constants.

## Goal

Run a targeted audit and refactor remaining leaks.

## Current candidates

- `SimulationFeeTaxAdminBurdenPolicy` can remain in Economy but should be isolated under a policy namespace/file, not mixed with generic scenario helpers.
- `SimulationScenarioPolicies.cs` currently mixes distance, capacity, surplus, trade, fee/tax/admin burden, and metrics. Split it.
- Avoid resource-specific properties in generic contracts. Prefer:
  - `SimulationResourceRequirement`
  - `SimulationResourceLimit`
  - `SimulationCostModel`
  - `SimulationConstraintDefinition`
- Keep `tax`, `fee`, `admin`, `trade` as generic economic event/policy concepts, not shared-well-specific behavior.

## Required audit

Search new generic projects for domain example terms:

```text
water, well, farmer, land, parcel, oligarchy, shared-well, near-household, far-household
```

Allowed only in:
- test fixtures
- example input packs
- docs / bundle files
- scenario factories
- tests that explicitly probe examples

Not allowed in:
- generic engine classes
- generic validators
- WebGlLib
- WebGlRunLib
