# SB08 — WebGlRunLib generic run/playback contract stabilization

## Status

Prepared / Not started

## Objective

Stabilize generic run contracts above WebGlLib so Economy and future production-line simulators can consume the same abstractions.

## Covered Inputs

- Normalized requirements: REQ-010, REQ-011
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB07 completed

## Exact Source References

- `SRC-CW-013`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` (webgl-engine lines 3-16) — WebGlRunLib exists as generic run and playback contracts over WebGlLib scene patches.
- `SRC-CW-012`: `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` (webgl-engine lines 3-20) — Solution includes WebGlLib, WebGlSandbox, WebGlRunLib and WebGlRunLib.Tests.
- `SRC-EC-003`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` (main lines 10-60, 62-139) — Economy bridge input/options/context and action mapper map visual actions to generic WebGlRunAction with strict fallback flags and provenance.
- `SRC-EC-004`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` (main lines 17-63) — Projector builds WebGlRunDocument from economy visual frames/actions and applies diagnostics.
- `SRC-EC-005`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` (main lines 17-63, 65-190) — Action stage projector maps, plans, validates, compiles and appends action stages, preserving source provenance.

## Deliverables

- Document WebGlRunDocument, frames, stages, actions, planner/compiler, object bindings, visual states and run-level metadata boundary.
- Ensure contracts are generic and contain no economy, ledger, market, production-line or domain-specific naming.
- Add validator for run documents and action plans.
- Add compile/parity tests for sequential, parallel, wait, event barrier, object motion barrier and scene patch actions.

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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB08/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB08/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj.
- Package-boundary scan: WebGlRunLib depends on WebGlLib only, not Economy.
- Adversarial tests: unsupported/domain-specific action names are rejected or mapped only through generic extension metadata.

## Browser Validation Logging

Required when browser-visible/runtime behavior is touched; otherwise record N/A with justification.

## Progression Gate

Critical semantic gate: generic run fixtures must include both non-economy sample and Economy-compatible sample without Economy references.

## Suggested Agent Prompt

Start `SB08` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB08/manifest.md` plus `proof/SB08/semantic-invariants.md` for critical work.
