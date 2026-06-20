# SB15 — Design matrix factor registry v3

## Priority

P1

## Repository scope

Economy

## Objective

Extend factor bindings beyond magnitude/store quantity/metadata/behavior/seed with validated typed bindings for policy parameters, event enablement, resource prices, capacity, and actor initial endowment.

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
- `proof/SB15/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Design factors must mutate the scenario intentionally and record effective definition hash.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB15_FocusedContractTests`
- `SB15_BoundaryAuditTests`
- `SB15_ProofArtifactValidationTests`
