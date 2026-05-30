# SB13 — Economy: generic simple state transition engine

## Problem

SimpleAccounts scenarios are still partly hand-materialized.

## Tasks

Implement generic transition pipeline in `Simulation.SimpleAccounts`:

```text
initial state
  + ordered event stream
  + parameter set
  + rule set
  -> frames/deltas
```

Required generic state:

- actors
- stores
- inventories
- resources
- relationships
- obligations
- issues
- metrics

Required generic event handlers:

- travel
- return-home
- resource-use
- resource-transfer
- work/admin
- trade
- tax/fee
- rule-check
- rule-violation
- enforcement
- relationship-change

## Done criteria

- Shared-well frames can be produced by the transition engine for at least one path, while the old seeded frame factory remains only as compatibility/test fixture.
