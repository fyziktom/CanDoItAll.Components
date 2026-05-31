# Generic probe cases

Do not implement demo-specific logic. Use examples only to verify generic capability.

## Probe A: shared resource access

Natural language example: a community shares a well.

Generic version:

```text
actors use a shared finite or replenishable resource at a location.
distance affects access cost.
some actors can build inventory or resell access.
rules impose limits, fees, taxes, and admin burden.
violations create issues and relationship changes.
```

Required generic capabilities:

- shared resource store at a location
- actor inventory
- resource need / use / collect / transfer
- distance/topology cost
- capacity limits
- trade/resale
- fee/tax/admin event derivation
- issue and rule enforcement
- visual action sequence
- snapshot analysis of admin pressure and resource concentration

## Probe B: finite spatial resource market

Natural language example: farmers on limited land.

Generic version:

```text
actors compete for a finite resource while outside demand may be large.
rules prevent runaway concentration or oligarchy.
concentration metrics and invariants indicate whether the rule works.
```

Required generic capabilities:

- finite resource capacity
- ownership/concentration
- external demand
- transfer/trade
- progressive fee/rule enforcement
- HHI/top-owner-share metrics
- anti-concentration invariant
- snapshot diff before/after rule change

## Probe principle

If a feature is only useful for one probe, it belongs in a fixture or test factory.
If it is useful for both probes, it is probably a generic kernel capability.
