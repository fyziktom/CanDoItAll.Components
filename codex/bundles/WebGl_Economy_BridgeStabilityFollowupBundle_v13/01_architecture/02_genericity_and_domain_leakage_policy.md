# Genericity and domain-leakage policy

## Allowed in generic layers

Generic layers may use:

- actor
- resource
- store
- relationship
- event
- rule
- institution
- location
- object
- metric
- invariant
- treatment
- placement
- parameter
- visual action
- pose
- symbol
- run
- frame
- stage
- command batch

## Prohibited in generic layers

Generic layers must not use example-specific terms:

- water
- well
- shared-well
- household-north
- near-household
- farmer
- land
- parcel
- oligarchy
- baker
- market-stall if used as a fixed scenario id
- any scenario-specific actor id

These may appear only in test fixtures, scenario input files, example factories, or documentation explicitly marked as examples/probes.

## Required pattern

Use resource-scoped parameters:

```text
resourceRequirement.{resourceId}.quantity
resourceLimit.{resourceId}.maxQuantity
carryCapacity.{resourceId}
storageCapacity.{resourceId}
metric.resourceId
```

Do not add new generic properties like `DailyWaterNeed`, `LandCap`, `WellDistance`, etc.
