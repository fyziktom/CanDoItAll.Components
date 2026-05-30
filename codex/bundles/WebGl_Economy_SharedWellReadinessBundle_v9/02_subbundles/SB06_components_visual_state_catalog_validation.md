# SB06 - Components: visual state catalog validation

## Problem
Pose/symbol/action-binding catalogs need validation before we map economy visual actions.

## Tasks
- Add validator for `WebGlVisualStateCatalog`.
- Validate:
  - duplicate pose/symbol/action-binding keys;
  - pose asset/variant references exist in scene asset catalog;
  - symbol asset references exist;
  - action binding points to known pose/symbol.
- Add no-op fallback pose and symbol behavior.

## Shared-well readiness
Validate these generic states:
- carry / resource collection;
- admin-writing;
- working / maintenance;
- speaking / enforcement;
- risk/conflict symbol;
- water/resource symbol.
