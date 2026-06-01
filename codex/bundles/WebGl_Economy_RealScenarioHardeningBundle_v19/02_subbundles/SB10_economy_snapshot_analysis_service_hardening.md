# SB10 - Economy snapshot analysis service hardening

Move all useful test-local snapshot analysis into reusable services.

Generic analyzers:

- active issue pressure,
- admin/work burden,
- resource concentration,
- resource scarcity,
- pending event pressure,
- relationship stress,
- visual stage pressure,
- unresolved visual mapping pressure.

No analyzer may hard-code water/well/farmer/land.

## Status

Completed.

## Goal

Move reusable snapshot analysis facets into production services that remain generic across scenario domains.

## Prerequisites

- SB09 snapshot runtime attachment must be available for visual-stage pressure inputs.

## Owned Requirements

- R10 Snapshot Analysis Services.

## Dependency Impact

Supplies SB12 readiness-report analysis and SB14 analyzer performance proof.

## Validation Depth

Service-level tests plus source scans for forbidden domain terms in production analyzer flow.

## Proof Required

- Economy analyzer test transcript.
- Source assertions for every required analyzer category.
- Domain-term scan transcript.
- Proof manifest.

Proof captured in `bundle://proof/SB10/manifest.md`.

## Progression Gate

Pass only when analyzers are reusable services and no required analyzer is test-local or domain-hard-coded.

Gate result: Passed. Production analyzers cover active issue pressure, admin/work burden, resource concentration, resource scarcity, pending events, relationship stress, visual stage pressure, and unresolved visual mapping pressure, with a production source scan proving no water/well/farmer/land terms.
