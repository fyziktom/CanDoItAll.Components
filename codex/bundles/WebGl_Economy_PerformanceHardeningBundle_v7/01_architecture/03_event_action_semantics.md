# Generic event/action semantics

The engine and simulator must not be built around a single "well" example. The well example is only a test case for generic primitives.

## Generic simulation event examples

- `actor.resource.use`
- `actor.resource.collect`
- `actor.resource.transfer`
- `actor.work.perform`
- `actor.admin.perform`
- `actor.trade.sell`
- `actor.trade.buy`
- `relationship.trust.change`
- `relationship.conflict.change`
- `resource.scarcity.change`
- `rule.enforcement.apply`
- `infrastructure.maintenance.perform`

## Economy visual intention examples

- `move-to-target`
- `return-to-anchor`
- `perform-at-target`
- `change-pose`
- `show-symbol`
- `hide-symbol`
- `pulse-link`
- `resource-transfer-visual`
- `wait`
- `apply-scene-patch`

## WebGL run action examples

- `MoveToObject`
- `ReturnToAnchor`
- `ChangePose`
- `ShowSymbol`
- `ResourceTransferVisual`
- `Wait`
- `ApplyScenePatch`

## Shared well scenario must compile into generic semantics

Example:

```text
citizen-1 uses well
```

should become simulation events:

```text
actor.resource.collect(subject=citizen-1, target=well-1, resource=water)
actor.admin.perform(subject=citizen-1, target=home-1, reason=stock-accounting)
actor.trade.sell(subject=citizen-1, target=citizen-4, resource=water)
```

then visual intentions:

```text
move-to-target(citizen-1 -> well-1.use)
show-symbol(citizen-1, water)
return-to-anchor(citizen-1 -> home)
change-pose(citizen-1, admin-writing)
show-symbol(citizen-1, parchment-writing)
move-to-target(citizen-1 -> citizen-4.trade)
resource-transfer-visual(citizen-1 -> citizen-4, water)
return-to-anchor(citizen-1 -> home)
```

then WebGL actions and command batches.
