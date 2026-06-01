# SB01 — Cross-repo current-state audit and safety harness

## Status

Prepared / Not started

## Objective

Re-read both repos, record current source hashes, detect already-landed changes, and create baseline proof scripts before touching production code.

## Covered Inputs

- Normalized requirements: REQ-001, REQ-015
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

None

## Exact Source References

- `SRC-CW-001`: `repo://CanDoItAll.Components/README.md` (webgl-engine lines 11-21, 32-47) — Packages table and WebGL sandbox routes; WebGlLib is generic scene/asset/symbol/interaction/proof contracts and WebGlSandbox is proof host.
- `SRC-CW-002`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` (webgl-engine lines 35-88) — WebGlWorkbench remains node/edge oriented; generic scene layer is additive; WebGlLib remains render substrate; simulation belongs in future WebGlRunLib or consuming package.
- `SRC-CW-012`: `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` (webgl-engine lines 3-20) — Solution includes WebGlLib, WebGlSandbox, WebGlRunLib and WebGlRunLib.Tests.
- `SRC-CW-013`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` (webgl-engine lines 3-16) — WebGlRunLib exists as generic run and playback contracts over WebGlLib scene patches.
- `SRC-EC-001`: `repo://CanDoItAll.Economy/README.md` (main lines 5-19, 21-36, 58-67) — Economy is standalone ledger module with headless-first decisions and optional UI integration; net10/EF Core 10 compatibility.
- `SRC-EC-002`: `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj` (main lines 3-22) — Economy WebGlBridge targets net10 and references Components WebGlRunLib either via local project ref or package.
- `SRC-BW-001`: `repo://CanDoItAll/codex/skills/bundles/candoitall-bundle-preparation/SKILL.md` (development lines 28-49, 50-93, 194-208) — Bundle preparation flow, required root sections, subbundle contract, semantic gates and validator expectations.

## Deliverables

- Fresh inventory of WebGlLib, WebGlRunLib, WebGlSandbox, Economy.Simulation.WebGlBridge, Economy.SimulationSandbox and tests.
- Source hash table under proof/SB01/changed-file-baseline.md.
- Baseline build/test/browser command transcript paths.
- Decision record stating which observations in this bundle are still current and which were superseded by repo changes.

## Dependency Impact

- Phase: Foundation
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB01/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB01/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- dotnet build CanDoItAll.Components.slnx from Components checkout.
- dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj.
- dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj when present.
- dotnet build CanDoItAll.Economy.slnx from Economy checkout.
- dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --filter WebGl|Simulation.
- Record any unavailable project/test explicitly as blocker or changed-current-state note.

## Browser Validation Logging

Required when browser-visible/runtime behavior is touched; otherwise record N/A with justification.

## Progression Gate

Critical foundation gate: no downstream subbundle may start until current-state hashes, baseline commands, known failures, and branch/ref names are recorded.

## Suggested Agent Prompt

Start `SB01` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB01/manifest.md` plus `proof/SB01/semantic-invariants.md` for critical work.
