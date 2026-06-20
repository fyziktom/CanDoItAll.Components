# SB04 — Domain driver contract v2

## Priority

P0

## Repository scope

Both

## Objective

Formalize domain mapping drivers as first-class adapters: domain-specific vocabulary in drivers, generic output in WebGlRunLib, and validation hooks owned by the consuming domain.

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
- `proof/SB04/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Economy driver is a reference implementation; generic library must not know Economy.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB04_FocusedContractTests`
- `SB04_BoundaryAuditTests`
- `SB04_ProofArtifactValidationTests`
