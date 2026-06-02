# SB11 — Browser UI and Node route proof

## Objective

Prove the Economy simulation sandbox and generic WebGlRun playback in realistic browser conditions after the provider/package/runtime semantics are fixed.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor*`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/Components/Pages/SimulationSandbox.razor`

## Deliverables

- Browser proof screenshots and JSON diagnostics.
- Large and narrow viewport proof.
- Console log review.
- UI review against readability, overflow, hierarchy, and interactive state checklist.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Playwright route action transcript.
- Browser screenshots.
- Runtime diagnostics JSON.
- No test fixture path assertion captured in browser/session diagnostics.

## Implementation Steps

- Validate `/run-playback` in Components WebGlSandbox.
- Validate `/economy/simulation-sandbox` in Economy Node.
- Use large desktop viewport and narrower responsive viewport.
- Prove Load, Apply frame, Step, First/Last, Snapshot, Analyze, diagnostics, and browser runtime state.
- Prove the scenario provider is not using test fixture path.
- Review UI layout and decide whether raw button/CSS is acceptable sandbox UI or should use BaseLib components.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB11 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Playwright route action transcript.
- Browser screenshots.
- Runtime diagnostics JSON.
- No test fixture path assertion captured in browser/session diagnostics.

Critical subbundles must also create/update `proof/SB11/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB11 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB11/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
