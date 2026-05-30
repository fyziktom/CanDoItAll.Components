# SB12 - Economy: visual action ordering and binding index

## Problem
Visual actions are currently vulnerable to event-id ordering and loose node binding.

## Tasks
- Sort events by step, timing offset, duration, declared order, then event id.
- Add `EconomyVisualBindingIndex` dictionary-based resolver.
- Add explicit visual nodes for locations/places or a target binding table for them.
- Sequence actions should not be emitted both as parent and top-level child unless child actions are marked `IsInternalStep`.

## Tests
- Shared-well travel/use/return/admin events produce ordered visual actions.
- Location target `shared-well` resolves without relying on first matching resource node.
- Sequence child steps are not double-executed by consumers.
