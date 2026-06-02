# SB05 — Patch transaction mode semantics

## Objective

Name, document, and test strict and permissive patch application modes across JS and C#.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/35-webgl-scene-patch-validation.js`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/`

## Deliverables

- Strict mode never mutates scene after invalid endpoint/object/scene id/revision failure.
- Permissive invalid-link mode skips invalid link additions, records warning, and identifies affected/non-affected ids.
- Command result metadata includes mode and classification.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Failing-first partial mutation test for strict invalid patch.
- Passing strict and permissive mode tests in C#.
- Browser Playwright proof for bad-link strict and warning modes.
- JS audit still passes.

## Implementation Steps

- Define metadata or typed option for patch transaction behavior: strict all-or-none, warning skip invalid links, or other explicit mode.
- Align C# reducer and JS runtime behavior.
- Preserve existing preflight validation benefits.
- Ensure warning-mode partial application is intentional and observable in command results.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB05 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Failing-first partial mutation test for strict invalid patch.
- Passing strict and permissive mode tests in C#.
- Browser Playwright proof for bad-link strict and warning modes.
- JS audit still passes.

Critical subbundles must also create/update `proof/SB05/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB05 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB05/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
