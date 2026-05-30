# SB10 - Economy event stream and materializer

## Goal

Compile scenario definitions into deterministic events, then materialize frames/deltas.

## Required pipeline

```text
SimulationScenarioDefinition
  -> SimulationScenarioEventCompiler
  -> SimulationEventStream
  -> ISimulationFrameMaterializer
  -> SimulationFrame / SimulationFrameDelta
```

## Generic event kinds

- `actor.resource.collect`
- `actor.resource.use`
- `actor.resource.transfer`
- `actor.trade.sell`
- `actor.trade.buy`
- `actor.admin.perform`
- `actor.work.perform`
- `rule.enforcement.apply`
- `relationship.trust.change`
- `relationship.conflict.change`
- `resource.stock.change`
- `resource.scarcity.signal`

## Shared well behavior example

The compiler/materializer must support:
- homes and well as locations/objects,
- actor distance to well,
- closer actor stockpiling water,
- selling water to farther actor,
- additional admin overhead for resale/stock tracking,
- conflict/trust effects.

This must be expressed generically through event kinds and rule parameters, not hardcoded as "well logic" inside engine classes.
