# SB10 — Asset cache and large-simulation performance budget

## Status

Prepared / not started.

## Objective

Add explicit resource/performance budgets for large simulations.

## Covered Inputs

- Raw user request in `inputs/raw-user-request.md`.
- Current-state analysis in `analysis/01-current-state-after-v2.md`.
- Critical findings mapped in `analysis/02-critical-findings.md`.

## Prerequisites

All upstream subbundles listed in `plan/01-phase-plan.md` must be completed and their proof manifests reviewed.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`

## Scope

- Define budget options and default profiles.
- Emit budget warnings in diagnostics.
- Add degraded rendering mode selection when limits exceed threshold.
- Document limits for Economy and future production-line simulations.

## Dependency Impact

This subbundle affects downstream phases according to `plan/01-phase-plan.md`. If its progression gate fails, do not continue to dependent subbundles. Update the bundle with the new blocker and reopen upstream work as needed.

## Validation Depth

Standard hardening. Requires source-level assertions, targeted unit tests, relevant build/audit commands, and browser proof when the subbundle touches user-visible or runtime-visible behavior.

## Implementation Steps

1. Add WebGlRuntimeBudgetOptions or extend WebGlRuntimeOptions safely.
2. Add JS diagnostics and C# parity fields.
3. Create synthetic large scene/run tests.
4. Add browser stress proof.

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

- Large scene emits budget warning instead of failing.
- Primitive fallback/degraded profile activates when configured.
- No unbounded asset cache growth in repeated load/dispose proof.

## Browser Validation Logging

Record route, viewport, Playwright/browser actions, diagnostics JSON, console log review, screenshot paths, and result. Use `N/A` only if no browser-visible or runtime-visible behavior changed.

## Progression Gate

The subbundle can close only when all acceptance checklist items are complete, `proof/SB10/manifest.md` is updated, semantic invariants are updated for critical subbundles, and downstream dependency impacts are reviewed.

## Suggested Agent Prompt

You are implementing `SB10` from the CanDoItAll WebGL/Economy v3 follow-up hardening bundle. Read this README, the phase plan, traceability, and source references. Work only inside this subbundle scope. Produce failing-first and passing proof, update manifests, and stop at the progression gate before starting downstream work.
