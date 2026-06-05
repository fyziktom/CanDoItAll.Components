# SB02 — Generic boundary term registry and CI normalization

## Priority

P0

## Repository scope

Components

## Objective

Move all domain term lists to explicit audit configs and make CI scan generic source, tests, docs, bundle artifacts, and generated public constants with clear allowlists.

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
- `proof/SB02/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Generic Components must not ship economic/production-line/scenario terms as production behavior.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB02_FocusedContractTests`
- `SB02_BoundaryAuditTests`
- `SB02_ProofArtifactValidationTests`
