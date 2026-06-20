# SB03 — Residual generic vocabulary hardening

## Priority

P0

## Repository scope

Components

## Objective

Search and rename/remove any remaining domain-shaped generic action names, diagnostics, comments, fixtures, public constants, or asset IDs that imply economy-only semantics.

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
- `proof/SB03/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- No generic API should be named around specific domains; use directed-flow/value-flow/actor/object/agent style terms only.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB03_FocusedContractTests`
- `SB03_BoundaryAuditTests`
- `SB03_ProofArtifactValidationTests`
