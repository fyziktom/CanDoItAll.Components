# SB10 — Economy WebGlBridge strict mapping and provenance hardening

## Status

Prepared / Not started

## Objective

Harden Economy's mapping from generic economy visual frames/actions into WebGlRunDocument without weakening genericity.

## Covered Inputs

- Normalized requirements: REQ-012, REQ-014, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB08 completed; SB09 completed or integration API explicitly documented

## Exact Source References

- `SRC-EC-002`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj` (main lines 3-22) — Economy WebGlBridge targets net10 and references Components WebGlRunLib either via local project ref or package.
- `SRC-EC-003`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` (main lines 10-60, 62-139) — Economy bridge input/options/context and action mapper map visual actions to generic WebGlRunAction with strict fallback flags and provenance.
- `SRC-EC-004`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` (main lines 17-63) — Projector builds WebGlRunDocument from economy visual frames/actions and applies diagnostics.
- `SRC-EC-005`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` (main lines 17-63, 65-190) — Action stage projector maps, plans, validates, compiles and appends action stages, preserving source provenance.
- `SRC-EC-006`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` (main lines 9-45, 136-193) — Validator verifies initial scene, frames, source metadata, stage commands/waits and command target object references.

## Deliverables

- Bridge validator enforces source frame/action/event/input-pack provenance on every generated stage/command.
- Unresolved node/action/pose/symbol mappings are errors by default and warnings only under explicit diagnostic fallback options.
- Strict and diagnostic modes have separate tests and proof outputs.
- Bridge does not add economy-only APIs to Components.

## Dependency Impact

- Phase: Economy bridge
- Repository scope: Economy
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB10/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB10/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge.
- Negative tests for unresolved subject/target/pose/symbol/action kind.
- Positive test for valid visual action producing WebGlRunDocument with source provenance.
- Project/package reference mode build check.

## Browser Validation Logging

Required only if an Economy browser host route exists; otherwise record command-level projection proof and explicit browser-host gap.

## Progression Gate

Critical semantic gate: no fallback-only happy path may close this subbundle; strict mode must fail correctly on bad mapping.

## Suggested Agent Prompt

Start `SB10` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB10/manifest.md` plus `proof/SB10/semantic-invariants.md` for critical work.
