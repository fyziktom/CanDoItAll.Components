# SB16 — Economy: visual action ordering and policy

## Problem

`EconomyVisualActionMapper` must not emit duplicate top-level steps and sequence steps that later get executed twice.

## Tasks

1. Add `EconomyVisualActionNormalizer`.
2. Define action containment rules:
   - nested sequence steps are not also top-level unless marked `emitAsStandalone`
   - start/duration must be canonical
   - target bindings must be typed
3. Map generic events to visual actions through policy tables, not switch sprawl.
4. Add tests:
   - shared-well resource-use event -> sequence only
   - admin event -> admin pose/symbol
   - trade/resale -> transfer + admin + tax/fee visual intentions
   - farmer expansion -> move/work/resource-transfer/rule-check visual intentions

## Done criteria

- Visual actions are stable, ordered, deduplicated, and generic.
