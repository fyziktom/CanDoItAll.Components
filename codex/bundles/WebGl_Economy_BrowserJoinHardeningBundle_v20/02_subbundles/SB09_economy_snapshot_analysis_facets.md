# SB09 - Economy snapshot analysis facets

## Status

Completed. Closure gate passed.

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

## Prerequisites

- SB08 persistence state can provide snapshots for analysis.

## Owned Requirements

- R09 Snapshot analysis facets.

## Dependency Impact

Analysis output feeds the sandbox page and SB11 snapshot-analysis proof.

## Validation Depth

Tests must exercise shared-resource and finite-resource snapshots and prove analyzers are generic rather than hard-coded to scenario names.

## Proof Required

- Snapshot analysis tests.
- Domain-term scan for reusable analyzer production paths.
- `bundle://proof/SB09/manifest.md`

## Browser Validation Logging

N/A for service tests. SB11 records visible analysis panel proof.

## Progression Gate

Pass only when both probe families produce useful generic analysis categories.
