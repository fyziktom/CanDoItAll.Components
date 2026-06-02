# SB04 — Incremental render updates and performance diagnostics

## Status

Completed at `2026-06-02T01:36:45Z`

## Objective

Make transform-only, symbol-only and link-only changes avoid full scene rebuild and prove it with diagnostics and stress tests.

## Covered Inputs

- Normalized requirements: REQ-005, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB03 completed

## Exact Source References

- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-007`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` (webgl-engine lines 3-9, 155-173) — updateObjectRuntimeTransform calls resolveObjectPosition in positionOnly mode but the import list shown does not include resolveObjectPosition.
- `SRC-CW-003`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` (webgl-engine lines 65-115, 177-235) — Scene view serializes Scene+Options for change detection and exposes interop API: fit/focus/reset/capture/export/import/patch/batch/transform/motion.
- `SRC-CW-008`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` (webgl-engine lines 33-64, 104-126) — Owned material dispose also disposes material textures. This needs texture ownership separation for cloned materials sharing template textures.

## Deliverables

- Introduce patch classification: transform-only, symbol-only, visual-replace, graph-structure, scene-rebuild.
- Avoid rebuildScene for transform-only and symbol-only patches; update links/symbol positions incrementally.
- Add diagnostics counters: fullSceneRebuildCount, transformOnlyPatchCount, symbolOnlyPatchCount, linkGeometryUpdateCount.
- Large-scene primitive performance proof with at least 250 objects and explicit target for 1000-object optional stress.

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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB04/manifest.md` and update traceability before closing.

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
- [x] Proof artifacts are stored under `proof/SB04/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Closure Notes

- Added runtime patch classification and routed transform-only, symbol-only, and link-only changes through incremental update paths.
- Added rebuild, classification, and link-geometry diagnostics in JS runtime snapshots, command results, C# interop DTOs, and proof snapshots.
- Captured failing-first browser stress proof showing the pre-SB04 rebuild-equivalent behavior, then passing browser proof with 250 objects and 100 transform patches.
- Completed refactor and boundary scan with no WebGlRunLib, Economy, ledger, market, production-line, Vernon, or Smith terms in touched WebGlLib production files.

## Proof Required

- Focused JS/browser proof: applying 100 transform patches increments transformOnlyPatchCount and does not increment fullSceneRebuildCount.
- dotnet build/test Components solution.
- Browser proof route /tycoon-village or a new /webgl-stress route with diagnostics JSON and screenshot.
- Record frame time and memory/rebuild diagnostics before/after.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Mandatory refactor checkpoint A: before any run-layer work, review WebGlLib touched files and remove accidental run/domain semantics, duplicate patch code, and broad rebuild fallbacks.

## Suggested Agent Prompt

Start `SB04` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB04/manifest.md` plus `proof/SB04/semantic-invariants.md` for critical work.
