# SB01 — Current-state and proof integrity audit

## Status

Prepared / not started.

## Objective

Re-audit the pushed v2 implementation and its proof artifacts before changing code.

## Covered Inputs

- Raw user request in `inputs/raw-user-request.md`.
- Current-state analysis in `analysis/01-current-state-after-v2.md`.
- Critical findings mapped in `analysis/02-critical-findings.md`.

## Prerequisites

None. This is the entry gate.

## Exact Source References

- `repo://CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v2/README.md`
- `repo://CanDoItAll.Economy/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v2/README.md`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/README.md`
- `repo://CanDoItAll.Economy/README.md`

## Scope

- Inventory all v2 changed production files and proof artifacts.
- Scan proof transcripts for zero length, placeholder content, command-only logs, and missing failure-first evidence.
- Record exact current head SHAs for both repos.
- Update bundle if any source file moved or changed since preparation.

## Dependency Impact

This subbundle affects downstream phases according to `plan/01-phase-plan.md`. If its progression gate fails, do not continue to dependent subbundles. Update the bundle with the new blocker and reopen upstream work as needed.

## Validation Depth

Critical foundation. Requires source-level assertions, targeted unit tests, relevant build/audit commands, and browser proof when the subbundle touches user-visible or runtime-visible behavior.

## Semantic Adequacy Gate

Required for this critical subbundle:

- Shallow-pass trap must be named.
- Failing-first proof must fail before implementation or be a documented pre-existing failure.
- Passing proof must exercise production code, not only fixtures.
- Anti-stub audit must scan production files touched by the subbundle.
- Raw requirement closure must be updated.


## Implementation Steps

1. Run git status and capture current branch/SHA for both repos.
2. Run a proof transcript hygiene scan over v2 proof folders.
3. Run baseline builds/tests/audits named in the v2 final summary.
4. Produce proof/SB01/current-state-inventory.md and proof/SB01/proof-hygiene-audit.md.
5. Do not edit production code in SB01.

## Scope Exceptions

None planned. If implementation discovers a reason to narrow scope, record the exception in `reviews/01-execution-report.md` before coding around it.

## Do Not Do

- Do not add Economy, ledger, market, production-line, or machine semantics to `CanDoItAll.Components.*`.
- Do not depend on `tests/` paths from runtime UI or Node routes.
- Do not close proof with empty transcript files.
- Do not mark the subbundle complete if browser-visible behavior changed without browser evidence.

## Acceptance Checklist

- [ ] Source references were reopened against current branch heads.
- [ ] Implementation only changed files in this subbundle's scope.
- [ ] Failing-first evidence exists when behavior is changed.
- [ ] Passing proof exercises production path.
- [ ] Boundary audits pass where relevant.
- [ ] Execution report and traceability were updated.
- [ ] Proof manifest has changed-file hashes and command transcript paths.

## Proof Required

- Changed-file inventory with SHA-256 hashes.
- Non-empty transcript audit.
- Build/test/audit transcripts.
- Semantic adequacy gate: shallow-pass trap is a bundle that says completed but has empty proof.

## Browser Validation Logging

Record route, viewport, Playwright/browser actions, diagnostics JSON, console log review, screenshot paths, and result. Use `N/A` only if no browser-visible or runtime-visible behavior changed.

## Progression Gate

The subbundle can close only when all acceptance checklist items are complete, `proof/SB01/manifest.md` is updated, semantic invariants are updated for critical subbundles, and downstream dependency impacts are reviewed.

## Suggested Agent Prompt

You are implementing `SB01` from the CanDoItAll WebGL/Economy v3 follow-up hardening bundle. Read this README, the phase plan, traceability, and source references. Work only inside this subbundle scope. Produce failing-first and passing proof, update manifests, and stop at the progression gate before starting downstream work.
