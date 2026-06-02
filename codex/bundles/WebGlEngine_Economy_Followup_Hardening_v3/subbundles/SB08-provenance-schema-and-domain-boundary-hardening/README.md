# SB08 — Provenance schema and generic/domain boundary hardening

## Status

Prepared / not started.

## Objective

Harden provenance and generic/domain boundary without losing Economy traceability.

## Covered Inputs

- Raw user request in `inputs/raw-user-request.md`.
- Current-state analysis in `analysis/01-current-state-after-v2.md`.
- Critical findings mapped in `analysis/02-critical-findings.md`.

## Prerequisites

All upstream subbundles listed in `plan/01-phase-plan.md` must be completed and their proof manifests reviewed.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`

## Scope

- Define typed or policy-bounded source provenance.
- Limit `source.*` values by length and semantic use.
- Disallow `source.*` from controlling generic runtime behavior.
- Keep Economy bridge source metadata sufficient for audit.

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

1. Add provenance validation helpers in WebGlRunLib.
2. Update Economy bridge to map source fields consistently.
3. Add negative tests where domain policy is hidden under source metadata.
4. Update boundary audit scripts.

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

- Generic validators allow source identifiers/hashes.
- Generic validators reject executable source policy metadata.
- EconomyWebGlRunValidator still requires source visualActionId/eventId/frameId/hash.

## Browser Validation Logging

Record route, viewport, Playwright/browser actions, diagnostics JSON, console log review, screenshot paths, and result. Use `N/A` only if no browser-visible or runtime-visible behavior changed.

## Progression Gate

The subbundle can close only when all acceptance checklist items are complete, `proof/SB08/manifest.md` is updated, semantic invariants are updated for critical subbundles, and downstream dependency impacts are reviewed.

## Suggested Agent Prompt

You are implementing `SB08` from the CanDoItAll WebGL/Economy v3 follow-up hardening bundle. Read this README, the phase plan, traceability, and source references. Work only inside this subbundle scope. Produce failing-first and passing proof, update manifests, and stop at the progression gate before starting downstream work.
