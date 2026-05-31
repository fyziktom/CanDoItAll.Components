# SB11 - Economy Transition Engine State and Diagnostics

## Goal

Make the simple transition engine safer and easier to debug.

## Required improvements

- `SimpleSimulationStepResult.Messages` must be populated with warnings/errors from:
  - event normalization
  - missing store
  - missing actor
  - capacity rejection
  - insufficient stock
  - unknown handler
  - negative stock prevented or allowed
- Add options:
  - allowNegativeStores
  - clampToCapacity
  - failOnUnknownEvent
  - failOnInsufficientStock
- Add deterministic rejected quantity flows or diagnostics.
- Add indexed lookup for:
  - store by id
  - store by actor/resource
  - store by location/resource
  - actor by id
  - relationship by id
- Ensure indexes rebuild after any store owner/resource/location change.

## Tests

- capacity overflow is reported
- insufficient source stock is reported
- unknown event kind is reported
- no silent state mutation without diagnostics
