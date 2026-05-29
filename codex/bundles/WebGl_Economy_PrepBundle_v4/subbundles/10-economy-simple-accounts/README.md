# SB10 Economy Simple-Account Backend Preparation

## Status

- Status: Completed

## Objective

- Add a simple-account simulation backend with deterministic toy/community scenarios.

## Covered Inputs

- `bundle://02_subbundles/SB10_economy_simple_accounts_backend_prep.md`
- `bundle://02_subbundles/SB13_economy_scenario_seeds_shared_well_and_entrepreneurs.md`

## Prerequisites

- SB09 abstractions are complete and boundary scan passes.

## Exact Source References

- `bundle://02_subbundles/SB10_economy_simple_accounts_backend_prep.md`
- `bundle://02_subbundles/SB13_economy_scenario_seeds_shared_well_and_entrepreneurs.md`

## Deliverables

- `C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts`
- Tests for frame/delta emission and deterministic scenario hashes.

## Dependency Impact

- Provides non-ledger simulation backend for future visual proof.

## Validation Depth

- Tests prove frames and deltas for shared-well and entrepreneur scenarios.

## Implementation Steps

- Add simple-account records, backend, scenario factory, and deterministic tests.

## Do Not Do

- Do not reference Ledger, BusinessObjects, Sdk, Components, or WebGL.

## Acceptance Checklist

- Shared-well and entrepreneur seeds emit reproducible frames and deltas.

## Proof Required

- Test transcripts, boundary scan, and changed-file hashes.

## Browser Validation Logging

- No browser proof required.

## Progression Gate

- Proceed to SB13/closure when simple-account hashes and boundary scans pass.

## Suggested Agent Prompt

- Prepare deterministic simple-account scenarios without ledger coupling.

