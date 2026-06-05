# SB17 — Non-economy generic driver canary

## Priority

P1

## Repository scope

Components

## Objective

Add a minimal non-economy domain driver/sample, such as production-line or logistics, to prove generic Components/WebGlRunLib are not coupled to Economy assumptions.

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
- `proof/SB17/manifest.md` updated with artifact paths, commands, and pass/fail status.

## Semantic invariants

- Generic engine must be usable by a second domain driver without Economy references.
- No TODO/stub-only implementation is acceptable.
- No empty transcript may be counted as proof.
- If a failure remains, the subbundle must explicitly mark it as unresolved and block downstream research-ready claims.

## Suggested test names

- `SB17_FocusedContractTests`
- `SB17_BoundaryAuditTests`
- `SB17_ProofArtifactValidationTests`
