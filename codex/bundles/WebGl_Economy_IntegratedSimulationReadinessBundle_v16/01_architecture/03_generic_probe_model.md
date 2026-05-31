# Generic probe model

Use examples only as probes, not as hardcoded features.

## Probe A: shared finite resource access

Generalized form:

- actors have locations and needs for a resource,
- a shared source has finite stock or throughput,
- distance affects acquisition cost/time,
- actors can build inventory,
- actors may trade surplus,
- institutions may impose fees, taxes, caps, admin burden or enforcement,
- relationships change when access/trade/rules are perceived unfair.

Do not hardcode `water`, `well`, `near-household` or `far-household` outside fixture/test data.

## Probe B: constrained spatial resource

Generalized form:

- actors control shares of a finite spatial resource,
- external demand creates growth pressure,
- actors may trade/acquire resource units,
- concentration metrics are computed,
- anti-concentration rules create caps/fees/enforcement,
- output/profit incentives can conflict with commons governance.

Do not hardcode `farmer`, `land`, `parcel` or `oligarchy` outside fixture/test data.

## Generic readiness criterion

A new probe should be expressible by changing:

- experiment input JSON,
- scenario definition JSON,
- placement JSON,
- parameters JSON,
- rules JSON,
- visual mapping JSON,
- expected invariants JSON.

It should not require modifying Components or generic Economy kernel code.
