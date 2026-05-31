# Finite-resource market readiness probe

Do not implement a "farmer land" demo in this bundle. Use this as a generic probe.

## Generic scenario

- A finite resource has total capacity.
- Actors trade/accumulate it.
- External demand encourages concentration.
- Rule prevents runaway concentration.
- Metrics detect top-owner share / concentration.
- Invariants fail if concentration exceeds cap.
- Snapshot diff shows before/after rule effects.

## Pass criteria

- No generic code contains `farmer`, `land`, `parcel`, or `oligarchy`.
- Resource id may be arbitrary, e.g. `resource.finite`.
- Metric/invariant evaluator is generic.
- Snapshot diff includes resource stores and metrics.
