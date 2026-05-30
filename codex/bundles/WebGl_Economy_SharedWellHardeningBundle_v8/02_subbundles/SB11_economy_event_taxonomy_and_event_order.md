# SB11 — Economy: event taxonomy and ordering

## Problem
Event kinds have canonical dotted names and legacy short aliases.

## Required work
- Add `SimulationEventKindRegistry`.
- Normalize aliases to canonical names.
- Order events by:
  1. step index;
  2. timing offset;
  3. deterministic sequence/order field;
  4. event id.
- Add explicit event order field if needed.
- Add tests for stable ordering and canonicalization.

## Shared-well requirement
Daily water use, travel, collect/use, trade, admin, tax/fee, rule check, violation, enforcement must be expressible as generic events.
