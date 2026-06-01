# Real Probe Expectations

## Shared-resource probe

Use the well scenario only as a generic shared-resource case.

The generic concepts are:

- actors
- shared resource source
- actor home locations
- distance/cost
- resource stores
- collection/use/transfer events
- rule check / issue / enforcement events
- admin burden events
- visual actions: move, perform, pose, symbol, return

The probe must prove:

- different distances create different travel/action costs
- resource collection updates stores
- trade/transfer events create flows
- rule/admin events create visible actions and snapshot metrics
- WebGL run stages remain executable and traceable

## Finite-resource concentration probe

Use the farmer-land scenario only as a generic finite-resource / concentration case.

The generic concepts are:

- finite resource units
- ownership stores
- transfer/trade events
- anti-concentration rule
- market pressure / external demand
- metrics for concentration
- visual actions: transfer, symbol, relationship pulse

The probe must prove:

- generic resource ownership and transfer logic works without shared-resource assumptions
- anti-concentration rules can be evaluated via generic metrics/invariants
- visual mapping is not specialized to wells/water/farms

## Acceptance

Both probes must use the same pipeline and differ only by input JSON and fixture-specific data.
