# SB11 - Domain leakage audit

## Goal
Avoid overfitting the kernel to shared-well or farmer-land examples.

## Forbidden in generic production code

- water
- well
- farmer
- land
- parcel
- oligarchy
- near-household
- far-household
- shared-well

Allowed locations:

- test fixtures,
- readiness probe tests,
- example scenario factories,
- docs explicitly describing examples.

## Required actions

1. Extend boundary audit to scan all generic production simulation packages.
2. Add allowlist with path-level reasons.
3. Fail on domain terms in abstractions, visualization, bridge generic logic or Components.
4. Add proof transcript.
