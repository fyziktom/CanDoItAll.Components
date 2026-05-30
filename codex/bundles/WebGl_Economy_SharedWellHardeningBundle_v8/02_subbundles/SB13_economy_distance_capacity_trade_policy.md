# SB13 — Economy: distance, capacity, and trade policy

## Shared-well acceptance gap
The example needs distance-aware behavior.

## Required work
- Add generic distance calculator for actor home/location to target location.
- Add inventory/carry capacity fields or metadata policy.
- Add simple trade policy:
  - closer actor can collect surplus;
  - farther actor can buy from closer actor;
  - trade creates admin/tax/fee events.
- Add tests proving:
  - closer actor has lower collection cost;
  - surplus creates transfer/sale events;
  - admin/tax events are emitted generically;
  - the behavior is deterministic.
