# SB01 — Current-state and v10 closure audit

## Priority

P0

## Repository scope

Both

## Objective

Audit what v10 actually changed across Components and Economy, including proof transcripts, changed file hashes, and open v10 claims.

## Required implementation work

- Start with a failing-first proof or source assertion that demonstrates the current gap.
- Implement the minimal robust fix, then refactor the touched area before moving on.
- Keep generic code domain-neutral; domain-specific terms must be placed in driver packages or driver-owned validation config.
- Update or add focused tests for the exact contract being changed.
- Update documentation if public behavior, readiness status, CLI output, or package boundary changes.

## Required proof

- Build output for touched solution(s).
- Focused unit/integration tests.
- Source scans for domain boundary where relevant.
- Browser proof for runtime/UI subbundles.
- Headless run artifacts for Economy subbundles.
- `proof/SB01/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- No implementation changes before baseline evidence exists; produce a factual closure matrix for every v10 recommendation.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB01_FocusedContractTests`
- `SB01_BoundaryAuditTests`
- `SB01_ProofArtifactValidationTests`
