# SB13 — Shared-resource probe, generic form

## Goal
Use the shared well case only as a generic shared-resource readiness probe.

## Must prove
- shared resource location
- actors at different distances
- inventory build
- resale/trade
- fee/tax/admin burden
- rule violation
- issue and relationship change
- visual action sequence
- snapshot analysis

## Forbidden
No new generic code containing `water` or `well`.

## Status
- Completed.

## Prerequisites
- SB08 snapshot builder proof is complete.
- SB09 snapshot analysis proof is complete.

## Exact Source References
- `bundle://08_readiness_probes/shared_resource_probe.md`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests`
- `C:\repositories\CanDoItAll.Economy\src`

## Dependency Impact
- Critical genericity proof for shared-resource simulation, visualization, snapshot, and analysis readiness.

## Validation Depth
- Requires semantic positive probe proof, forbidden-term scan in generic production code, and anti-stub audit.

## Acceptance Checklist
- Probe proves shared resource location, distance, inventory, trade, fees/admin burden, rule violation, issues, relationships, visual sequence, and snapshot analysis.
- No new generic production code contains `water` or `well`.

## Proof Required
- `bundle://proof/SB13/manifest.md`
- `bundle://proof/SB13/semantic-invariants.md`
- Economy test transcript and forbidden-term scan.

## Browser Validation Logging
- Browser validation is not required unless the probe is shown through a rendered route.

## Progression Gate
- SB15 may proceed when shared-resource genericity and analysis proof is recorded.

## Suggested Agent Prompt
- Prove the shared-resource case through generic capabilities only and scan generic production code for forbidden example terms.
