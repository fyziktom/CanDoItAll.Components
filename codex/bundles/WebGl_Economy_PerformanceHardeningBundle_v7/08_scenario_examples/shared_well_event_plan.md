# Shared well event/action plan

This is an example only. The engine must stay generic.

## Scenario entities

- Actors:
  - `citizen.near`
  - `citizen.far`
  - `well-keeper`
  - `rule-council`
- Objects:
  - `home.near`
  - `home.far`
  - `resource.well`
  - `admin.desk.near`
- Resources:
  - `water`
  - `time`
  - `admin-work`
  - `trust`
  - `cash`

## Simulation events

```text
actor.resource.collect(citizen.near -> resource.well, water, 10)
actor.resource.transfer(citizen.near -> citizen.far, water, 5)
actor.trade.sell(citizen.near -> citizen.far, water, cash)
actor.admin.perform(citizen.near -> admin.desk.near, reason=stock-ledger)
relationship.trust.change(citizen.far -> citizen.near, +0.1)
```

## Visual intentions

```text
move-to-target(citizen.near, resource.well.use)
show-symbol(citizen.near, water)
return-to-anchor(citizen.near, home)
move-to-target(citizen.near, admin.desk.near)
change-pose(citizen.near, admin-writing)
show-symbol(citizen.near, parchment-writing)
move-to-target(citizen.near, citizen.far.trade)
resource-transfer-visual(citizen.near, citizen.far, water)
return-to-anchor(citizen.near, home)
```
