# SB10 — Economy: typed references and event taxonomy

## Problem

Events use string IDs for source/target and duplicate event-kind vocabularies.

## Tasks

1. Add `SimulationRef`:
   - `Kind`: actor, resource, store, location, object, rule, market, institution
   - `Id`
2. Add typed source/target/participants while preserving string compatibility fields.
3. Add canonical event taxonomy:
   - resource.collect
   - resource.use
   - resource.transfer
   - actor.travel
   - actor.return
   - actor.work
   - actor.admin
   - market.trade
   - market.loan
   - market.repayment
   - rule.check
   - rule.violation
   - rule.enforcement
   - relationship.change
4. Keep old names as aliases only.
5. Add validation and tests.

## Done criteria

- Event mapping can distinguish actor `water` from resource `water`.
- Alias kinds do not pollute new logic.
