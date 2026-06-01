# SB12 — Cross-repo packaging, reference and documentation integration

## Status

Prepared / Not started

## Objective

Prove both repos work as a coherent system through project-reference and package-consumption modes without circular dependencies.

## Covered Inputs

- Normalized requirements: REQ-014, REQ-001, REQ-010
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB07, SB09, SB10 completed

## Exact Source References

- `SRC-CW-001`: `repo://CanDoItAll.Components/README.md` (webgl-engine lines 11-21, 32-47) — Packages table and WebGL sandbox routes; WebGlLib is generic scene/asset/symbol/interaction/proof contracts and WebGlSandbox is proof host.
- `SRC-CW-012`: `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` (webgl-engine lines 3-20) — Solution includes WebGlLib, WebGlSandbox, WebGlRunLib and WebGlRunLib.Tests.
- `SRC-CW-013`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` (webgl-engine lines 3-16) — WebGlRunLib exists as generic run and playback contracts over WebGlLib scene patches.
- `SRC-EC-002`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj` (main lines 3-22) — Economy WebGlBridge targets net10 and references Components WebGlRunLib either via local project ref or package.
- `SRC-EC-001`: `repo://CanDoItAll.Economy/README.md` (main lines 5-19, 21-36, 58-67) — Economy is standalone ledger module with headless-first decisions and optional UI integration; net10/EF Core 10 compatibility.

## Deliverables

- Components package versioning and pack output instructions for WebGlLib and WebGlRunLib.
- Economy build instructions for ComponentsRepoRoot local project reference and UseComponentsWebGlRunLibPackage=true package reference.
- Dependency graph and package boundary scans.
- Docs explaining ultra-light WebGlLib, generic WebGlRunLib and Economy bridge consumption paths.

## Dependency Impact

- Phase: Integration
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB12/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB12/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages.
- Economy build with ComponentsRepoRoot local path.
- Economy build with UseComponentsWebGlRunLibPackage=true after package feed setup.
- Dependency scan transcript: no Economy reference in Components.

## Browser Validation Logging

Required when browser-visible/runtime behavior is touched; otherwise record N/A with justification.

## Progression Gate

Mandatory refactor checkpoint C: before browser/perf final proof, inspect package boundaries and docs for accidental heavyweight defaults.

## Suggested Agent Prompt

Start `SB12` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB12/manifest.md` plus `proof/SB12/semantic-invariants.md` for critical work.
