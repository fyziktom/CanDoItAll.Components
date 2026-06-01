# Probe examples as generic readiness checks

## Shared-resource community probe

Example story:
- actors need a shared resource,
- distance to resource differs,
- nearby actor can stockpile and resell,
- rules create fees, taxes, admin actions, and possible rule violation.

Generic engine requirements:
- actor/resource/location topology,
- resource stores and capacity,
- distance/cost policy,
- event stream,
- visual actions: move, collect/use, trade, admin pose, status symbol, return,
- metrics: concentration, admin burden, active issues,
- snapshots with visual state.

## Finite-resource market probe

Example story:
- actors compete over finite spatial resource,
- external demand encourages expansion,
- rules prevent concentration/oligarchy.

Generic engine requirements:
- finite resource capacity,
- ownership transfer,
- rule enforcement,
- concentration metrics,
- visual actions: transfer ownership, move to market/institution, show warning, pulse relationship/link.

## Why both probes matter

If the bridge works for both probes without hard-coded terms, the architecture is much less likely to be overfit to the well example.
