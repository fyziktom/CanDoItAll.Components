# SB04 — Revision and runtime option reset policy

## Objective

Make scene revision normalization and browser reset runtime-option behavior explicit, consistent, and tested.

## Status

Completed 2026-06-02.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentNormalizer.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`
- `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md`

## Deliverables

- Canonical revision policy documented in Components docs and tests.
- Browser apply adapter reset policy documented and covered by tests.
- No ambiguous divergence between `Scene.Revision` and `UiState.Revision` after normalize/serialize/patch.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Failing-first conflicting-revision test.
- Passing C# serializer/reducer tests.
- Browser or fake-runtime adapter test proving runtime options are intentionally ignored/applied.
- Update source assertions in proof manifest.

## Implementation Steps

- Fix `WebGlSceneRevisionPolicy.Normalize` to keep top-level and UI revisions consistent or explicitly document UI-excluded behavior.
- Add document serializer tests for conflicting revisions, UI-excluded serialization, import/export, and patch commit.
- Decide whether `WebGlSceneDocument.RuntimeOptions` are ignored or applied during `WebGlRunBrowserApplyAdapter` scene reset.
- Add tests and docs for the chosen runtime options policy.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB04 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for critical behavior changes.
- [x] Passing proof exercises production code paths, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Failing-first conflicting-revision test.
- Passing C# serializer/reducer tests.
- Browser or fake-runtime adapter test proving runtime options are intentionally ignored/applied.
- Update source assertions in proof manifest.

Critical subbundles must also create/update `proof/SB04/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Completion Notes

- `WebGlSceneRevisionPolicy.Normalize` now uses the canonical `Commit` path so `Scene.Revision` and `UiState.Revision` are mirrored when UI state is included.
- UI-excluded serialization keeps the canonical scene revision and resets UI state to defaults, avoiding ambiguous dual-source comparisons.
- `WebGlRunBrowserApplyAdapter` treats document runtime options as external during scene reset, imports a scene-only reset document with default runtime options, and warns when non-default reset options were stripped.
- Proof is recorded in `bundle://proof/SB04/manifest.md` and `bundle://proof/SB04/semantic-invariants.md`.

## Browser Validation Logging

N/A unless this subbundle changes browser-visible runtime or UI. If browser-visible behavior is touched, record route, viewport, actions, assertions, screenshot paths, console log, and result.

## Progression Gate

SB04 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB04/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
