# SB09 — Browser observer proof real-state validation

## Priority

P0

## Repository scope

Components

## Objective

Require browser proof to load/export actual document and scene state, compare document hash, scene content hash, runtime idle state, final object positions and completed stage IDs.

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
- `proof/SB09/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Observer proof compares expected vs live browser runtime state, never expected vs expected.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB09_FocusedContractTests`
- `SB09_BoundaryAuditTests`
- `SB09_ProofArtifactValidationTests`
