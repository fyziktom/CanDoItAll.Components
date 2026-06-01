# Generic scenario probe analysis

Do not optimize for a single example. Use at least these probes:

## Shared finite resource probe

Equivalent of a shared water source, but generic:

- actors have home locations,
- a finite resource source exists,
- actor distance to source differs,
- actors have resource requirement and carry/storage capacity,
- near actors may build inventory and trade surplus,
- rules may create fee/tax/admin burden,
- violations create issues and relationship changes,
- visualization needs movement to resource, work/admin pose, symbols, return home.

Generic capabilities needed:

- resource requirement model by resource id,
- capacity and store model by actor/resource,
- location/topology/distance model,
- trade event semantics,
- rule/policy expansion,
- visual action mapping from event kind to action sequence,
- pause/snapshot/analysis.

## Finite ownership/resource concentration probe

Equivalent of farmers and land parcels, but generic:

- finite resource objects are owned by actors,
- one actor may try to accumulate resource,
- external demand or market pressure exists,
- institution/rule applies anti-concentration constraint,
- metrics measure top-holder share / concentration,
- visualization shows transfer, rule symbol, institution action, relationship stress.

Generic capabilities needed:

- finite object/resource inventory,
- ownership transfer event,
- concentration metric,
- cap/invariant evaluator,
- rule violation and enforcement event,
- visual pulse/link/action mapping.

## Small producer/community trade probe

Equivalent of entrepreneurs, but generic:

- multiple producers transform inputs into output,
- market demand changes over time,
- rules/taxes/admin burden affect actors,
- actors may specialize, trade, compete,
- visualization needs work pose, movement, transaction flows, issue symbols.

Generic capabilities needed:

- production/transformation events,
- input/output resource flow,
- pricing/offer event,
- admin burden events,
- relationship/trust/conflict metrics.
