# SB11 — Economy generic scenario proof and large simulation readiness

## Status

Completed

## Objective

Keep the two current economy examples as examples over generic simulation contracts and prove scale/determinism for broader experimental economics use.

## Covered Inputs

- Normalized requirements: REQ-013, REQ-012
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB10 completed

## Exact Source References

- `SRC-EC-001`: `repo://CanDoItAll.Economy/README.md` (main lines 5-19, 21-36, 58-67) — Economy is standalone ledger module with headless-first decisions and optional UI integration; net10/EF Core 10 compatibility.
- `SRC-EC-003`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` (main lines 10-60, 62-139) — Economy bridge input/options/context and action mapper map visual actions to generic WebGlRunAction with strict fallback flags and provenance.
- `SRC-EC-004`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` (main lines 17-63) — Projector builds WebGlRunDocument from economy visual frames/actions and applies diagnostics.
- `SRC-EC-005`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` (main lines 17-63, 65-190) — Action stage projector maps, plans, validates, compiles and appends action stages, preserving source provenance.
- `SRC-EC-006`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` (main lines 9-45, 136-193) — Validator verifies initial scene, frames, source metadata, stage commands/waits and command target object references.

## Deliverables

- Inventory the two current example scenarios and extract what is generic vs scenario-specific.
- Add or improve scenario fixtures/probes for larger runs with many agents/resources/actions.
- Ensure Vernon-Smith-style experiment scenarios use generic visual mapping, not hard-coded bridge behavior.
- Add deterministic replay/readiness artifacts.

## Dependency Impact

- Phase: Economy bridge
- Repository scope: Economy
- Validation depth: Standard
- Downstream subbundles must treat this proof as prerequisite when named in `plan/01-phase-plan.md`.
- If this subbundle is reopened, all downstream proof depending on its behavior becomes suspect until rerun.

## Validation Depth

Standard.  
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB11/manifest.md` and update traceability before closing.

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
- [x] Proof artifacts are stored under `proof/SB11/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Proof Required

- Focused tests: SimulationSandboxWorkflow, EconomyPerformanceProbe, EconomyWebGlSnapshotVisualStateBuilder.
- Large scenario projection benchmark transcript with frame/action counts and elapsed time.
- Negative proof that unsupported scenario-specific action does not silently map to wait unless diagnostic mode is explicit.

## Browser Validation Logging

Required only if an Economy browser host route exists; otherwise record command-level projection proof and explicit browser-host gap.

## Progression Gate

Refactor gate: if scenario code adds switch statements for concrete example names, extract mapping/provider contract before closure.

## Suggested Agent Prompt

Start `SB11` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB11/manifest.md` plus `proof/SB11/semantic-invariants.md` for critical work.
