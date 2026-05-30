# SB14 — Economy: shared-well readiness without final demo

## Tasks

Do not build the final UI/demo. Instead add readiness tests:

1. Load shared-well input pack from JSON.
2. Validate placement and parameters.
3. Compile events.
4. Expand behavior/rules.
5. Run transition engine.
6. Produce frames/deltas.
7. Produce visual actions.
8. Verify:
   - near actor has lower travel cost
   - water stock changes
   - resale creates admin/tax event
   - violation creates issue
   - rule enforcement changes trust/conflict
   - visual action sequence includes move/use/admin/return
   - all hashes are stable

## Done criteria

- Readiness proof shows what the future demo will consume, without implementing the demo UI.
