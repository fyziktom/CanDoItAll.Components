# SB02 — WebGlLib JavaScript runtime correctness and module audit

## Status

Completed

SB02 closed with a static import/export audit, the `resolveObjectPosition` import fix, npm/README wiring, and browser proof on `/tycoon-village`. Evidence is recorded under `proof/SB02/`.

## Objective

Fix unresolved JS imports and create static/runtime audit coverage for the modular vanilla JS runtime.

## Covered Inputs

- Normalized requirements: REQ-002, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB01 completed

## Exact Source References

- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-007`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` (webgl-engine lines 3-9, 155-173) — updateObjectRuntimeTransform calls resolveObjectPosition in positionOnly mode but the import list shown does not include resolveObjectPosition.
- `SRC-CW-003`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` (webgl-engine lines 65-115, 177-235) — Scene view serializes Scene+Options for change detection and exposes interop API: fit/focus/reset/capture/export/import/patch/batch/transform/motion.
- `SRC-CW-002`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` (webgl-engine lines 35-88) — WebGlWorkbench remains node/edge oriented; generic scene layer is additive; WebGlLib remains render substrate; simulation belongs in future WebGlRunLib or consuming package.

## Deliverables

- Fix missing or stale imports such as resolveObjectPosition in 11-webgl-scene-graph.js if still present.
- Add tools/webgllib/audit-scene-runtime-imports.cjs or equivalent that parses ES module import/export references for runtime/scene modules.
- Include audit in npm scripts and CI-ready validation notes.
- Add a browser smoke proving create, drag, transform patch and dispose all execute without console errors.

## Dependency Impact

- Phase: WebGlLib hardening
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB02/manifest.md` and update traceability before closing.

## Do Not Do

- Do not add Economy or production-line concepts to Components.
- Do not solve failures by relaxing strict validation without a documented diagnostic mode.
- Do not close the subbundle with structure-only proof.
- Do not skip browser proof when the runtime or UI behavior changes.
- Do not proceed to downstream subbundles until this progression gate passes.

## Acceptance Checklist

- [x] All owned requirements are addressed.
- [x] Source references were reread against current repo state.
- [x] Negative proof exists for at least one realistic failure mode.
- [x] Positive proof demonstrates intended behavior.
- [x] Refactor gate completed and recorded.
- [x] Proof artifacts are stored under `proof/SB02/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Proof Required

- npm run webgllib:audit-scene-runtime-imports.
- npm run webgllib:audit-scene-runtime.
- dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj.
- Browser route /tycoon-village: create scene, drag draggable object, apply transform-only patch, capture diagnostics, dispose by navigation/reload; capture console log.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Semantic gate: failing-first proof must show the audit catches an intentionally unresolved symbol or a fixture copy before accepting the fix.

## Suggested Agent Prompt

Start `SB02` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB02/manifest.md` plus `proof/SB02/semantic-invariants.md` for critical work.
