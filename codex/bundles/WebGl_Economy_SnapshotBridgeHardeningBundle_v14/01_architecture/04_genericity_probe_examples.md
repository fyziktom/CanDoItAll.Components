# Genericity Probe Examples

## Probe A: shared resource access

Generic interpretation, not "well-specific":
- actors have locations,
- a resource exists at a source location,
- actors have resource needs,
- distance influences collection cost,
- some actors may build inventory,
- trade/resale may occur,
- rules may impose fees/admin burden,
- violations may change trust/conflict.

Required generic primitives:
- actor,
- location,
- resource,
- store,
- action/event,
- policy/rule,
- relationship,
- issue,
- metric/invariant.

## Probe B: finite spatial resource concentration

Generic interpretation, not "farmer-specific":
- actors own shares of a finite resource,
- external demand creates pressure to expand,
- trades transfer ownership,
- rule caps prevent concentration,
- fees or sanctions influence behavior,
- metrics evaluate concentration.

Required generic primitives:
- finite resource,
- ownership store,
- trade event,
- institution/rule,
- concentration metric,
- invariant evaluator.

## Design rule

If a feature only works for water/well/farmer/land, it is not done.
The generic model should work for both probes without adding new core fields named after example concepts.
