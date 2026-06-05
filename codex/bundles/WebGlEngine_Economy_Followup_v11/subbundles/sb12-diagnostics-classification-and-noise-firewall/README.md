# SB12 — Diagnostics classification and noise firewall

## Priority

P0

## Repository scope

Economy

## Objective

Classify every diagnostic code as economic-model, scenario-input, projection, browser-runtime, UI, performance, or proof-noise; readiness must not mix categories.

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
- `proof/SB12/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- A failed browser proof must not be reported as economic-model failure, and model warnings must block research readiness.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB12_FocusedContractTests`
- `SB12_BoundaryAuditTests`
- `SB12_ProofArtifactValidationTests`
