# Canvas And Floating Windows Publishing Readiness v1

Bundle id: `CanDoItAll.Components.CanvasFloatingWindows.PublishingReadiness.v1`  
Created local date: `2026-06-29`  
Profile: `initiative`  
Repository: `C:\repositories\CanDoItAll.Components`

## Mission

Prepare the CanvasLib and floating-window surfaces for future open-source publishing by inventorying the real implementation, tightening ownership boundaries, planning safe refactors, adding missing contract and runtime proof, expanding sandbox and Playwright validation, and aligning package/API/docs evidence without changing WebGL.

## Hard Scope Rules

- In scope: `CanDoItAll.Components.CanvasLib`, `CanDoItAll.Components.OverlayLib`, Canvas and overlay sandbox routes, Canvas asset tooling, Canvas/Overlay package metadata, tests, docs, and bundle proof.
- Out of scope: WebGL implementation, WebGL runtime refactors, WebGL package/API changes, Economy/app-specific behavior, and renderer replacement work beyond the existing Canvas benchmark evidence.
- Preserve all current public functionality unless a subbundle explicitly proves a bug fix with failing-first and passing evidence.
- Do not hand-edit generated Canvas asset include components; use `tools/canvaslib/build-assets.cjs` and verify with `tools/canvaslib/verify-assets.cjs`.
- Do not close UI work from source inspection only. Canvas and floating-window claims require browser proof, open-state screenshots, and explicit visual review.

## Validation Summary

- Bundle preparation status: `Prepared`
- Bundle readiness gate: `Prepared-stage validator passed on 2026-06-29`
- Execution status: `Not started`
- Subbundle gate review: `Pending implementation`
- Final closure gate: `Pending implementation`
- Browser validation analytics: `Planned for SB05-SB08 with maximized desktop, fixed desktop, tablet, and mobile passes`

## Primary Artifacts

- Original request: `bundle://inputs/00-original-request.md`
- Structured input: `bundle://inputs/02-structured-input.md`
- Current-state analysis: `bundle://analysis/01-current-state.md`
- Assumptions and risks: `bundle://analysis/02-assumptions-and-risks.md`
- Scope inventory: `bundle://inventories/01-scope-inventory.md`
- Normalized requirements: `bundle://requirements/01-normalized-requirements.md`
- Target solution: `bundle://architecture/01-target-solution.md`
- Phase plan: `bundle://plan/01-phase-plan.md`
- Traceability matrix: `bundle://traceability/01-requirement-traceability.md`
- Execution report seed: `bundle://reviews/01-execution-report.md`
- Prepared validator transcript: `bundle://reviews/prepared-validation.txt`
- Validator script: `bundle://scripts/validate_bundle.py`

## Handoff Notes

Execution should start at SB01. SB01-SB04 establish the inventory, OverlayLib/CanvasLib boundary, state contracts, and asset/runtime proof foundations. SB05-SB08 perform true UI validation. SB09 and SB10 close publishing-readiness, package/API/docs, and red-team proof. Any browser defect in a later phase must reopen the owning foundation instead of being hidden as residual risk.
