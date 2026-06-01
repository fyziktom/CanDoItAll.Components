# SB09 — WebGlRunLib execution integration over WebGlSceneView

## Status

Prepared / Not started

## Objective

Define and prove how WebGlRunLib run documents execute into WebGlLib scene patches, command batches and browser-visible motion without leaking simulation logic into WebGlLib.

## Covered Inputs

- Normalized requirements: REQ-010, REQ-011, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB08 completed

## Exact Source References

- `SRC-CW-003`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` (webgl-engine lines 65-115, 177-235) — Scene view serializes Scene+Options for change detection and exposes interop API: fit/focus/reset/capture/export/import/patch/batch/transform/motion.
- `SRC-CW-013`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` (webgl-engine lines 3-16) — WebGlRunLib exists as generic run and playback contracts over WebGlLib scene patches.
- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-012`: `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` (webgl-engine lines 3-20) — Solution includes WebGlLib, WebGlSandbox, WebGlRunLib and WebGlRunLib.Tests.

## Deliverables

- A generic adapter or service that turns WebGlRunFrame/WebGlRunActionStage into WebGlSceneCommandBatch.
- Deterministic playback controls sufficient for proof: apply frame, play sequence, pause/cancel/reset, export proof snapshot.
- No mandatory dependency from WebGlLib to WebGlRunLib; higher layer calls WebGlSceneView public API.
- Browser proof with generic non-economy run document.

## Dependency Impact

- Phase: WebGlRunLib hardening
- Repository scope: Components
- Validation depth: Critical foundation
- Downstream subbundles must treat this proof as prerequisite when named in `plan/01-phase-plan.md`.
- If this subbundle is reopened, all downstream proof depending on its behavior becomes suspect until rerun.

## Validation Depth

Critical foundation.  
Critical subbundles require semantic adequacy proof, artifact-backed manifest, negative proof, positive proof, anti-stub audit and local refactor gate.

## Implementation Steps

1. Reopen source references and update current-state notes if the repo has changed.
2. Add or adjust failing-first test/proof for the intended behavior.
3. Implement the smallest correct change set for this subbundle only.
4. Run focused validation.
5. Run broader build/test/browser proof listed below.
6. Complete mandatory refactor gate using `templates/refactor-gate-checklist.md`.
7. Update proof manifest, semantic invariants when critical, execution report and traceability.

## Scope Exceptions

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB09/manifest.md` and update traceability before closing.

## Do Not Do

- Do not add Economy or production-line concepts to Components.
- Do not solve failures by relaxing strict validation without a documented diagnostic mode.
- Do not close the subbundle with structure-only proof.
- Do not skip browser proof when the runtime or UI behavior changes.
- Do not proceed to downstream subbundles until this progression gate passes.

## Acceptance Checklist

- [ ] All owned requirements are addressed.
- [ ] Source references were reread against current repo state.
- [ ] Negative proof exists for at least one realistic failure mode.
- [ ] Positive proof demonstrates intended behavior.
- [ ] Refactor gate completed and recorded.
- [ ] Proof artifacts are stored under `proof/SB09/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- WebGlRunLib tests for frame-to-batch compilation and barrier order.
- Browser proof route in WebGlSandbox for generic run playback.
- Diagnostics prove commands are batched; no per-object interop loop for large runs.
- Dependency scan confirming WebGlLib remains independent.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Mandatory refactor checkpoint B: before Economy work, inspect WebGlRunLib APIs for domain leakage and overfitting to current examples.

## Suggested Agent Prompt

Start `SB09` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB09/manifest.md` plus `proof/SB09/semantic-invariants.md` for critical work.
