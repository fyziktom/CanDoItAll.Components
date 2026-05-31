# SB14 — Finite-resource market probe, generic form

## Goal
Use the farmer/land case only as a generic finite-resource concentration probe.

## Must prove
- finite resource capacity
- actor expansion
- external demand
- concentration metric
- anti-concentration rule
- fee or enforcement
- snapshot diff before/after policy change

## Forbidden
No new generic code containing `farmer`, `land`, `parcel`, or `oligarchy`.

## Status
- Completed. Proof recorded under `bundle://proof/SB14/manifest.md`.

## Prerequisites
- SB08 snapshot builder proof is complete.
- SB09 snapshot analysis proof is complete.

## Exact Source References
- `bundle://08_readiness_probes/finite_resource_probe.md`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests`
- `C:\repositories\CanDoItAll.Economy\src`

## Dependency Impact
- Critical genericity proof for concentration, policy, snapshot diff, and analysis readiness.

## Validation Depth
- Requires semantic positive probe proof, policy-change diff proof, forbidden-term scan in generic production code, and anti-stub audit.

## Acceptance Checklist
- Probe proves finite capacity, expansion, demand, concentration metric, anti-concentration rule, fee/enforcement, and before/after snapshot diff.
- No new generic production code contains `farmer`, `land`, `parcel`, or `oligarchy`.

## Proof Required
- `bundle://proof/SB14/manifest.md`
- `bundle://proof/SB14/semantic-invariants.md`
- Economy test transcript and forbidden-term scan.

## Browser Validation Logging
- Browser validation is not required unless the probe is shown through a rendered route.

## Progression Gate
- SB15 may proceed when finite-resource genericity and snapshot-diff proof is recorded.

## Suggested Agent Prompt
- Prove the finite-resource market case through generic capabilities only and scan generic production code for forbidden example terms.
