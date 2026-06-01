# SB12 — Domain leakage and genericity audit

## Goal

Prevent example-specific terms leaking into generic kernel.

## Forbidden in generic production code

In generic libraries, scan for:

- `water`
- `well`
- `shared-well`
- `near-household`
- `far-household`
- `farmer`
- `land`
- `parcel`
- `oligarchy`

Allowed places:

- test fixtures;
- specific scenario factories;
- README/probe docs;
- intentionally named example files.

## Required validation

Extend or harden existing boundary audit to enforce this and generate a transcript.

## Design check

Any generic feature required by the well/land probes must be named by generic capability:

- shared resource
- resource store
- finite resource
- resource requirement
- travel/distance/cost
- inventory/capacity
- rule enforcement
- admin burden
- concentration metric
