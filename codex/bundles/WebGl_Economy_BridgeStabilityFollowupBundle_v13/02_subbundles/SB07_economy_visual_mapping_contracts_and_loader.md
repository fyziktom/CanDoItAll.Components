# SB07 — Economy visual mapping contracts and loader

## Problem
The bridge should not hardcode asset, pose, symbol, or anchor decisions.

## Tasks
- Harden `EconomyVisualMappingDefinition`.
- Add loader/validator for `visual.mapping.json`.
- Validate:
  - category -> asset mapping
  - action kind -> pose/symbol binding
  - anchor aliases
  - fallback strategy
- Add domain-neutral test fixtures.

## Tests
- shared-well and farmer-land use the same schema with different input JSON.
- missing pose/symbol produces warning and optional no-op fallback, not hidden success.
