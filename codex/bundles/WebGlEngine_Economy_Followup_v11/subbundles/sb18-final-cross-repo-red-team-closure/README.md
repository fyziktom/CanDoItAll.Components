# SB18 — Final cross-repo red-team closure

## Priority

P0

## Repository scope

Both

## Objective

Run final audits, builds, focused tests, browser proof, headless runs for all scenarios, proof integrity validator, and update docs with honest readiness status.

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
- `proof/SB18/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- No empty transcripts, no TODO stubs, no uncited claims, no research-ready status without artifact proof.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB18_FocusedContractTests`
- `SB18_BoundaryAuditTests`
- `SB18_ProofArtifactValidationTests`
