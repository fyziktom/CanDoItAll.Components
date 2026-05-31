# Shared-well readiness probe

This is not the final demo. Use it as a coverage probe.

A generic pipeline must support:

- actors with resource requirements
- resource stores and capacity
- spatial placement
- distance/travel cost
- carry capacity
- inventory build
- direct use from shared source
- transfer/resale
- tax/fee/admin burden
- rule violation detection
- trust/conflict relationship change
- visual sequence:
  - move to target
  - pose/symbol
  - transfer/carry
  - admin writing
  - return home

Pass criteria:
- no hardcoded `water` or `well` in bridge or generic abstractions.
- all resource-specific behavior comes from input JSON and generic resource ids.
