# SB09 — Remove example-specific leakage from generic code

## Goal
Ensure generic code remains reusable.

## Tasks
- Extend audit to scan:
  - Simulation.Abstractions
  - Simulation.SimpleAccounts
  - Simulation.Visualization
  - Simulation.WebGlBridge
  - Components WebGlRunLib/WebGlLib
- Prohibit example terms outside examples/fixtures/docs/tests:
  - water, well, shared-well, farmer, land, parcel, oligarchy, near-household, far-household
- Replace any generic API leaking example-specific fields with resource-scoped metadata or typed parameter objects.

## Tests
- audit fails on deliberate forbidden term in generic source.
- fixtures and example docs are allowlisted.
