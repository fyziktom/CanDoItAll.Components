# Implementation Prompt

Implement the selected subbundle only.

## Required Start

- Reopen `bundle://README.md`, `bundle://plan/01-phase-plan.md`, `bundle://requirements/01-normalized-requirements.md`, this subbundle README, and `bundle://reviews/01-execution-report.md`.
- Confirm prerequisites and checkpoint gates before editing source.
- Retry the `candoitall_components` MCP for relevant component usage examples before layout refactors. If it is unavailable, record the fallback source inspection in the proof manifest.
- Run or inspect the current relevant baseline before changing behavior.

## Work Rules

- Preserve all current functionality unless this subbundle explicitly owns a proven bug fix.
- Do not edit WebGL implementation, packages, docs, or tests.
- Prefer shared BaseLib/OverlayLib/CanvasLib boundaries over page-local one-off wrappers.
- Do not hand-edit generated Canvas asset include components; use `npm run canvaslib:build-assets` and verify with `npm run canvaslib:verify-assets`.
- Keep changes scoped to the subbundle. If later evidence weakens an earlier foundation, reopen that subbundle.

## Proof Rules

- Critical subbundles must write `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md`.
- Behavior changes need failing-first and passing proof for the same invariant.
- UI changes need browser proof with route, viewport, action, assertion, screenshot path, console result, and visual review answers.
- Floating windows and overlays need open-state proof for readable content, clipping, lateral overflow, layering, drag/resize/minimize/restore/hide/reset, and state roundtrip.
- Update `reviews/01-execution-report.md` before requesting closure.

## Stop Conditions

- Stop if WebGL work becomes necessary.
- Stop if public API changes are needed but not documented and approved by the subbundle.
- Stop if screenshots, tests, or asset verification contradict the intended proof.
