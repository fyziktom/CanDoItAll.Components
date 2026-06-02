# SB13 — Browser, performance and memory red-team proof

## Status

Completed

## Objective

Prove the hardened engine works in browser with generic and economy runs, including stress, console cleanliness, fallback behavior and dispose safety.

## Covered Inputs

- Normalized requirements: REQ-015, REQ-005, REQ-006, REQ-012, REQ-013
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB12 completed

## Exact Source References

- `SRC-CW-003`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` (webgl-engine lines 65-115, 177-235) — Scene view serializes Scene+Options for change detection and exposes interop API: fit/focus/reset/capture/export/import/patch/batch/transform/motion.
- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-008`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` (webgl-engine lines 33-64, 104-126) — Owned material dispose also disposes material textures. This needs texture ownership separation for cloned materials sharing template textures.
- `SRC-CW-009`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js` (webgl-engine lines 5-77) — Asset cache is state-local and disposes templates when scene is disposed.
- `SRC-EC-004`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` (main lines 17-63) — Projector builds WebGlRunDocument from economy visual frames/actions and applies diagnostics.
- `SRC-EC-006`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` (main lines 9-45, 136-193) — Validator verifies initial scene, frames, source metadata, stage commands/waits and command target object references.

## Deliverables

- Playwright/browser proof for WebGlSandbox generic scene and generic run playback.
- Playwright/browser proof or executable artifact for Economy projection rendered through Components runtime if host route exists; otherwise record blocker and provide command-level projection proof.
- Stress proof: primitive large scene, GLB profile switching, repeated import/export, dispose/recreate.
- Memory/resource red-team report for asset cache, material/texture ownership and rebuild counters.

## Dependency Impact

- Phase: Final proof
- Repository scope: Both
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB13/manifest.md` and update traceability before closing.

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
- [x] Proof artifacts are stored under `proof/SB13/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Closure Notes

- Browser proof covers WebGlSandbox `/tycoon-village`, `/run-playback`, `/performance-proof`, and Economy Node `/economy/simulation-sandbox`.
- `/performance-proof` exposed an oversized command-result callback payload; `20-webgl-scene-command-results.js` now compacts Blazor event callback results while preserving rich direct JS interop results.
- Economy browser proof applies frame 1 through `WebGlRunBrowserApplyAdapter` with 3 stages, 2 motions, zero adapter errors/warnings, a nonblank pixel probe, snapshot capture, and analysis.
- Focused Components/WebGlRunLib tests, resource ownership proof, boundary audits, command-batch parity audit, and Economy focused tests passed. Evidence is stored under `proof/SB13/`.

## Proof Required

- Browser console log must contain no unexpected errors; known Three.js GLTF extension warnings must be classified.
- Diagnostics JSON must include rebuild counters, asset cache counters, missing/fallback asset counters, frame timing and command batch metrics.
- Screenshots reviewed against visual QA checklist, not merely attached.
- Red-team negative cases: bad patch, missing GLB, unresolved economy mapping, repeated dispose/import.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Critical final proof gate: prose-only success is invalid; every claim must point to proof/SB13 manifest artifacts.

## Suggested Agent Prompt

Start `SB13` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB13/manifest.md` plus `proof/SB13/semantic-invariants.md` for critical work.
