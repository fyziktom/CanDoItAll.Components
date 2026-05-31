# SB14 - Snapshot-driven analysis probe

Goal:
- Validate the user workflow.

Probe:
1. Materialize shared-resource input pack.
2. Take snapshot at a step where admin/rule burden is visible.
3. Evaluate:
   - number of actors doing admin,
   - active issues,
   - store quantities,
   - trust/conflict relationships,
   - top resource holder share.
4. Export snapshot JSON.
5. Re-import and verify hash.

Acceptance:
- The snapshot can answer "why does the visual state look bad?" without manually inspecting runtime internals.

## Status

Completed.

## Prerequisites

SB08 snapshot store/export and SB09 visual snapshot attachment.

## Validation Depth

Add or verify a snapshot analysis probe that materializes a shared-resource input pack, snapshots a rule/admin burden step, evaluates metrics/issues/stores/relationships/resource holder share, exports JSON, re-imports, and verifies hash.

## Progression Gate

SB15 may proceed only after analysis proof demonstrates the pause/analyze question can be answered from snapshot data and optional visual attachment, not runtime internals.
