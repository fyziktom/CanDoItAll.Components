# Entrepreneur community event/action plan

This is an example only. The engine must stay generic.

## Simulation events

```text
actor.trade.buy(baker -> farmer, inventory, cash)
actor.work.perform(baker, production)
actor.resource.transfer(baker -> customer-group, goods)
actor.resource.transfer(customer-group -> baker, cash)
actor.admin.perform(baker, invoice-recording)
actor.resource.transfer(local-fund -> carpenter, cash)
relationship.trust.change(customer-group -> baker, +0.05)
relationship.obligation.change(baker -> local-fund, -12)
```

## Visual intentions

```text
move-to-target(baker, farmer.trade)
resource-transfer-visual(farmer, baker, inventory)
return-to-anchor(baker, bakery)
change-pose(baker, work)
show-symbol(baker, goods)
move-to-target(customer-group, bakery.counter)
resource-transfer-visual(customer-group, baker, cash)
change-pose(baker, admin-writing)
show-symbol(baker, parchment-writing)
```
