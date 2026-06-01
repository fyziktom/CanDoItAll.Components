# SB09 - Economy snapshot analysis facets

## Goal

Turn snapshot analysis into reusable services.

## Tasks

- Add analyzers/facets for:
  - admin burden,
  - active issue pressure,
  - resource concentration,
  - relationship stress,
  - pending event pressure,
  - visual stage pressure,
  - fallback/diagnostic pressure.
- Keep them generic.
- Avoid hard-coded water/well/farmer/land logic.

## Acceptance

- Shared-resource and finite-resource snapshots both produce useful analysis.
- Analysis explains visual symptoms from data, not from WebGL internals.
