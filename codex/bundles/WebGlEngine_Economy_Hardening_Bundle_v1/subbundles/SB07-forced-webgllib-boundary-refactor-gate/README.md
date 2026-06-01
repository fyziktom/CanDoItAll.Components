# SB07 — Forced WebGlLib boundary refactor gate

## Status

Prepared / Not started

## Objective

Stop feature work and refactor the WebGlLib layer back to a clean ultra-light render substrate before run-layer hardening begins.

## Covered Inputs

- Normalized requirements: REQ-001, REQ-010
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB02-SB06 completed or explicitly blocked with recorded current-state reason

## Exact Source References

- `SRC-CW-001`: `repo://CanDoItAll.Components/README.md` (webgl-engine lines 11-21, 32-47) — Packages table and WebGL sandbox routes; WebGlLib is generic scene/asset/symbol/interaction/proof contracts and WebGlSandbox is proof host.
- `SRC-CW-002`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` (webgl-engine lines 35-88) — WebGlWorkbench remains node/edge oriented; generic scene layer is additive; WebGlLib remains render substrate; simulation belongs in future WebGlRunLib or consuming package.
- `SRC-CW-012`: `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` (webgl-engine lines 3-20) — Solution includes WebGlLib, WebGlSandbox, WebGlRunLib and WebGlRunLib.Tests.
- `SRC-CW-013`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` (webgl-engine lines 3-16) — WebGlRunLib exists as generic run and playback contracts over WebGlLib scene patches.

## Deliverables

- Boundary audit report: what belongs in WebGlLib, what belongs in WebGlRunLib, what belongs in Economy.
- Move or fence any heavy run/simulation semantics discovered in WebGlLib.
- Create minimal model-viewer sample or documented usage path proving WebGlLib can still be consumed without WebGlRunLib.
- Update package docs and dependency graph.

## Dependency Impact

- Phase: Boundary refactor gate
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB07/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB07/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- Dependency scan: WebGlLib must not reference WebGlRunLib or Economy.
- dotnet pack Components solution or at least dotnet build all Components projects.
- Minimal WebGlLib-only usage sample builds without WebGlRunLib references.
- Review artifacts under proof/SB07/boundary-audit.md.

## Browser Validation Logging

Required when browser-visible/runtime behavior is touched; otherwise record N/A with justification.

## Progression Gate

Hard stop gate: SB08 cannot start until this boundary audit explicitly passes.

## Suggested Agent Prompt

Start `SB07` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB07/manifest.md` plus `proof/SB07/semantic-invariants.md` for critical work.
