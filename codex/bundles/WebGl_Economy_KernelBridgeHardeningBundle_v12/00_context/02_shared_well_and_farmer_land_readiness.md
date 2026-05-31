# Shared-Well and Farmer-Land Readiness Analysis

These examples are only probes. They must not become special cases in the engine.

## Shared-well probe: expected generic capabilities

A simple shared-well scenario needs:

- deterministic input pack
- explicit actor home locations
- explicit well/resource location
- resource stores
- actor resource requirements
- carry capacity
- storage capacity
- travel speed / travel cost
- rules for daily draw, resale, fee/tax, and admin burden
- event stream:
  - travel to resource
  - collect/use resource
  - inventory build
  - transfer/trade
  - fee/tax/admin event
  - rule check
  - violation/enforcement
  - relationship change
  - return home
- state transition:
  - source stock decreases
  - actor inventory increases/decreases
  - fees/taxes move to collector
  - rule issues are raised/resolved
  - relationships change deterministically
- visual actions:
  - move actor to target
  - change pose/carry/admin
  - show resource/rule/tax/risk symbol
  - return home
  - pulse relationship/link

## Farmer-land probe: expected generic capabilities

A finite-land scenario needs:

- explicit land parcels or land capacity as resource/object/store
- actors with initial land stores
- external buyer/market with demand
- ownership transfer events
- anti-concentration rule parameters
- metrics:
  - top owner share
  - HHI or concentration index
  - rule violation count
- invariants:
  - no actor exceeds cap
  - total land is conserved
- visual actions:
  - land transfer / ownership change
  - rule/institution intervention
  - risk/concentration symbol
  - market demand link

## Missing generic proof

The code should include one shared readiness probe that confirms both scenarios can be expressed by the same abstractions:

- No event handler has `if scenarioId contains shared-well`.
- No visual mapper has `if resourceId == water`.
- No core transition logic uses specific actor IDs from either fixture.
- All example-specific behavior is defined by input JSON, parameters, rule definitions, or handler/plugin selection.
