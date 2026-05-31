# Shared-well probe analysis

The shared-well scenario is useful because it forces the architecture to handle multiple generic concerns at once:

- actors have locations and distances to a resource;
- actors have resource needs;
- actors have carry capacity and storage capacity;
- actors can build inventory;
- nearby actors may resell surplus to far actors;
- rules impose fair-use constraints;
- rule violations trigger enforcement;
- trade triggers fees/taxes/admin burden;
- administration creates visual actions such as writing with paper/quill symbol;
- all inputs must be explicit, versioned, hashed, and reproducible.

What is still missing for a future demo:

1. A generic resource requirement model. `DailyWaterNeed` must become a resource-scoped need, e.g. `requirements[{ resourceId, quantityPerStep, priority }]`.
2. A generic actor capacity model. Carry/storage/travel/admin must not be fixed to water.
3. A deterministic route/travel-cost resolver using placement/topology input files.
4. A generic inventory policy that can build stock, consume stock, and sell surplus.
5. A generic market/trade event expansion policy that emits trade, fee, tax, admin, and relationship events.
6. A rule/policy interpreter that uses input `institution.rules.json`, not hardcoded event switch logic.
7. A stage-aware visual action plan that maps the same logical event into ordered visual steps.
8. Invariants: no negative stores unless allowed; max draw rules; fees/taxes posted; event stream hash stable; result metrics interpretable.

Do not build the demo yet. Implement only generic primitives and proof fixtures that make this demo possible later without special-case code.
