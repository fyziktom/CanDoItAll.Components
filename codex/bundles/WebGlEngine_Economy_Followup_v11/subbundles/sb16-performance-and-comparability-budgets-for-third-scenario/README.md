# SB16 — Performance and comparability budgets for third scenario

## Priority

P1

## Repository scope

Economy

## Objective

Create small/medium/large performance budget rows for multi-goods-elite and assert not-comparable when headless budgets or artifact sizes exceed profile.

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
- `proof/SB16/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Performance noise must not masquerade as economic effect.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB16_FocusedContractTests`
- `SB16_BoundaryAuditTests`
- `SB16_ProofArtifactValidationTests`
