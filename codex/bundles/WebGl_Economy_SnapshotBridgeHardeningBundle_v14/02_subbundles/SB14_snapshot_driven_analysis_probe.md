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
