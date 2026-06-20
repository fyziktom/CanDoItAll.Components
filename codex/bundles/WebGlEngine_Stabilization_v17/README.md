# CanDoItAll Components WebGL Engine Stabilization Follow-up Bundle v17

Bundle id: `CanDoItAll.Components.WebGlEngine.Stabilization.Followup.v17`  
Created UTC: `2026-06-06T10:55:44Z`  
Scope: **CanDoItAll.Components only**  
Primary branch assumption: `webgl-engine`

## Mission

This bundle finishes the generic WebGL/Run stabilization wave before the project moves back to primarily Economy work.

The goal is not to add Economy features. The goal is to make the generic WebGL engine stable enough that future Economy or production-line simulators can consume it through domain drivers without changing generic Components except for genuine generic bugs, performance work, security fixes, packaging issues or explicitly approved generic API changes.

## Hard scope rules

- Modify only `CanDoItAll.Components`.
- Do not modify `CanDoItAll.Economy`.
- Do not add economy, market, buyer/seller, elite, production-line, machine, station, work-order or domain-specific semantics to `src/CanDoItAll.Components.WebGlLib` or `src/CanDoItAll.Components.WebGlRunLib`.
- A production-line canary may use domain vocabulary only inside sample/test/canary fixtures or a domain-driver fixture, never in generic engine source.
- Every checkpoint must be reviewed before continuing.

## Why another Components-only bundle is still justified

Codex already made several good moves:
- `IsPackable=false` is now default and WebGL packages opt in.
- approval tests exist for public API, JS surface, package content and action vocabulary.
- `webgl:validate-rc` exists as a single RC command.
- runtime idle policy modes exist.
- domain driver contract exists.
- hard/soft domain-boundary profiles exist.

The remaining risk is proof truthfulness and generic flexibility:
- WebGlRunLib package-mode sample still needs true package-mode switching.
- RC validation must prove that each step actually tested what it claims.
- WebGlSceneView still concentrates too much boundary behavior.
- JS API approval must lock behavior, not just names.
- manufacturing/production-line canary is still needed to reveal economy-shaped assumptions.

## Required implementation posture

Codex may work for hours, but must pause at every checkpoint:
- Checkpoint A: proof truthfulness
- Checkpoint B: public/runtime boundary
- Checkpoint C: simulator-canary and lifecycle
- Checkpoint D: performance and observer
- Final signoff

No checkpoint can be bypassed by merely saying tests pass. It must include artifact-backed proof.

## Outputs required from Codex

- Updated Components code and tests.
- Updated docs.
- Updated approval snapshots only when intentionally approved.
- Full RC validation artifacts.
- A final `reviews/02-final-red-team-closure.md` answering:
  - Is Components WebGL ready to freeze?
  - Which remaining changes are allowed after freeze?
  - What must move to Economy or a future production-line domain driver?
