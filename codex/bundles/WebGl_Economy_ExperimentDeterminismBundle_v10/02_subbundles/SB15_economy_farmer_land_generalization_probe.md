# SB15 — Economy: farmer-land generalization probe

## Purpose

Avoid overfitting to shared-well.

## Tasks

Add a non-UI test-only input pack for farmer-land:

- farmers
- finite land parcels
- external buyer demand
- expansion events
- ownership transfers
- anti-oligarchy rules
- concentration metrics

Do not build a full simulation. Add contract/validator tests that prove current generic model can express:

- finite spatial resource
- ownership
- expansion/investment
- external demand
- rule constraints
- concentration metrics/invariants

## Done criteria

- Farmer-land input pack validates.
- Missing model capabilities are recorded as explicit follow-up items, not hidden.
