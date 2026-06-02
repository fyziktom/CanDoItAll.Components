# SB12 — Final cross-repo red-team closure

## Objective

Run final QA, red-team, package, browser, and documentation closure across both repositories.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- All files touched by SB01-SB11.
- `repo://CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_Bundle_v2/` if copied into repo.
- `repo://CanDoItAll.Economy/codex/bundles/WebGlEngine_Economy_Followup_Hardening_Bundle_v2/` if copied into repo.

## Deliverables

- Final report says which requirements are solved, partial, blocked, or deferred.
- No critical proof manifest is missing or empty.
- Package/browser/build/test results agree.
- Follow-up backlog is explicit and not hidden in prose.

## Dependency Impact

Critical closure. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- `dotnet build` both solutions.
- Focused tests for WebGlLib, WebGlRunLib, Economy simulation/WebGlBridge.
- Browser proof for generic and Economy routes.
- Package-mode proof with isolated cache.
- Red-team notes and final QA inspector sign-off.

## Implementation Steps

- Rerun full focused validation matrix from this bundle.
- Validate every critical subbundle proof manifest.
- Run cross-repo boundary audits.
- Run package-mode and project-reference-mode validation.
- Produce final execution report, traceability closure, and follow-up backlog for deliberately deferred work.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB12 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- `dotnet build` both solutions.
- Focused tests for WebGlLib, WebGlRunLib, Economy simulation/WebGlBridge.
- Browser proof for generic and Economy routes.
- Package-mode proof with isolated cache.
- Red-team notes and final QA inspector sign-off.

Critical subbundles must also create/update `proof/SB12/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB12 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB12/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
