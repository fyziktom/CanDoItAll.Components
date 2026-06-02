# SB05 — Texture-safe resource ownership and asset cache hardening

## Status

Completed at `2026-06-02T02:26:23Z`

## Objective

Prevent GLB texture/material lifecycle bugs and make asset fallback/cache behavior deterministic under repeated load/import/dispose cycles.

## Covered Inputs

- Normalized requirements: REQ-006, REQ-007, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB04 completed

## Exact Source References

- `SRC-CW-008`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` (webgl-engine lines 33-64, 104-126) — Owned material dispose also disposes material textures. This needs texture ownership separation for cloned materials sharing template textures.
- `SRC-CW-009`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js` (webgl-engine lines 5-77) — Asset cache is state-local and disposes templates when scene is disposed.
- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-002`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` (webgl-engine lines 35-88) — WebGlWorkbench remains node/edge oriented; generic scene layer is additive; WebGlLib remains render substrate; simulation belongs in future WebGlRunLib or consuming package.

## Deliverables

- Separate ownsMaterial, ownsGeometry and ownsTexture semantics.
- Ensure cloned tinted materials do not dispose shared template textures unless textures were explicitly cloned and marked owned.
- Add cache diagnostics and disposal proof for repeated import/profile switch/dispose.
- Clarify state-local vs optional shared asset cache policy; do not introduce global cache without ownership tests.

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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB05/manifest.md` and update traceability before closing.

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
- [x] Proof artifacts are stored under `proof/SB05/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Closure Notes

- Split resource ownership into geometry, material, and texture semantics so tinted material clones do not dispose shared template textures.
- Made state-local asset cache disposal release cached template geometry/material/texture resources with diagnostics.
- Added durable JS resource ownership proof, typed DTO coverage, and WebGlLib docs for cache/resource counters.
- Captured browser proof with two textured `question_box.glb` instances, profile switching, intentional missing asset fallback, and final scene/cache disposal.

## Proof Required

- Browser proof switching primitive/mixed/high profiles repeatedly keeps no missing assets except intentional fallback.
- Source-level assertion/audit for disposeMaterialTextures usage and ownership flags.
- Stress proof: load same GLB in multiple instances, remove one instance, remaining instances still render and diagnostics show no premature texture disposal.
- dotnet build and existing WebGlLib tests.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Critical resource gate: no downstream performance or economy browser proof is valid until texture ownership and repeated dispose proof pass.

## Suggested Agent Prompt

Start `SB05` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB05/manifest.md` plus `proof/SB05/semantic-invariants.md` for critical work.
