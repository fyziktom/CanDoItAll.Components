# SB11 — Package versioning and consumer proof hardening

## Status

Prepared / not started.

## Objective

Harden package versioning and consumer proof beyond local 0.1.0 cache behavior.

## Covered Inputs

- Raw user request in `inputs/raw-user-request.md`.
- Current-state analysis in `analysis/01-current-state-after-v2.md`.
- Critical findings mapped in `analysis/02-critical-findings.md`.

## Prerequisites

All upstream subbundles listed in `plan/01-phase-plan.md` must be completed and their proof manifests reviewed.

## Exact Source References

- `repo://CanDoItAll.Components/Directory.Build.props`
- `repo://CanDoItAll.Components/README.md`
- `repo://CanDoItAll.Economy/README.md`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/CanDoItAll.Economy.Components.csproj`

## Scope

- Define local proof versioning convention for Components packages.
- Avoid static 0.1.0 collision in proof feeds.
- Add package consumer proof for WebGlLib-only, WebGlRunLib, Economy.WebGlBridge, and Economy.Components.
- Document required flags and fail conditions.

## Dependency Impact

This subbundle affects downstream phases according to `plan/01-phase-plan.md`. If its progression gate fails, do not continue to dependent subbundles. Update the bundle with the new blocker and reopen upstream work as needed.

## Validation Depth

Standard hardening. Requires source-level assertions, targeted unit tests, relevant build/audit commands, and browser proof when the subbundle touches user-visible or runtime-visible behavior.

## Implementation Steps

1. Use a unique prerelease package version for package proofs.
2. Run restore/build in isolated NUGET_PACKAGES cache.
3. Scan dependency graphs for unwanted WebGlRunLib in WebGlLib-only sample.
4. Record package content listing and hashes.

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

- Fresh package feed has unique package versions.
- Package consumer builds fail against stale feed and pass against fresh feed.
- No accidental project references in package-mode proof.

## Browser Validation Logging

Record route, viewport, Playwright/browser actions, diagnostics JSON, console log review, screenshot paths, and result. Use `N/A` only if no browser-visible or runtime-visible behavior changed.

## Progression Gate

The subbundle can close only when all acceptance checklist items are complete, `proof/SB11/manifest.md` is updated, semantic invariants are updated for critical subbundles, and downstream dependency impacts are reviewed.

## Suggested Agent Prompt

You are implementing `SB11` from the CanDoItAll WebGL/Economy v3 follow-up hardening bundle. Read this README, the phase plan, traceability, and source references. Work only inside this subbundle scope. Produce failing-first and passing proof, update manifests, and stop at the progression gate before starting downstream work.
