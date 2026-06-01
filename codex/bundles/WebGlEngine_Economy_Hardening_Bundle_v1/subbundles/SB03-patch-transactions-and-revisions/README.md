# SB03 — Transactional patching and canonical revision policy

## Status

Prepared / Not started

## Objective

Make C# and JS patch semantics align, prevent partial mutation on invalid patches, and define exactly one canonical scene revision.

## Covered Inputs

- Normalized requirements: REQ-003, REQ-004, REQ-008
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB02 completed

## Exact Source References

- `SRC-CW-004`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModel.cs` (webgl-engine lines 5-32, 34-55) — Scene model has top-level Revision and UiState.Revision; objects, links, layers, asset catalog, camera, interaction, metadata.
- `SRC-CW-005`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` (webgl-engine lines 7-97, 100-174) — C# reducer validates before apply, handles base revision, add/remove/patch objects, add/remove links, warnings/errors.
- `SRC-CW-006`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` (webgl-engine lines 23-114, 137-210) — JS patching mutates scene incrementally but later calls rebuildScene for any changed patch; validation is not preflight-transactional.
- `SRC-CW-010`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentSerializer.cs` (webgl-engine lines 8-50) — Scene document serializer normalizes, hashes, serializes, deserializes and validates WebGlSceneDocument.
- `SRC-CW-011`: `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` (webgl-engine lines 9-191) — Document tests cover roundtrip, forbidden run metadata, hash invariants, order-insensitive hashing, invalid vectors, missing asset warnings.

## Deliverables

- Architecture decision record for Scene.Revision vs UiState.Revision; update contracts and docs accordingly.
- Preflight JS patch validation or clone-then-commit implementation.
- C# reducer and JS patch parity tests covering wrong scene id, strict base revision, missing link endpoint, add/remove/patch ordering, layer cleanup and no partial commit.
- Export/import and WebGlSceneDocument hash behavior updated for canonical revision policy.

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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB03/manifest.md` and update traceability before closing.

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
- [ ] Proof artifacts are stored under `proof/SB03/`.
- [ ] Traceability and execution report are updated.
- [ ] No scope leakage into downstream subbundles.

## Proof Required

- Add WebGlScenePatchReducer parity tests in C#.
- Add JS audit/unit harness or browser-evaluate proof that failed patch leaves object/link counts and revisions unchanged.
- dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter Patch|Document|Revision.
- Browser proof: bad link endpoint patch fails and scene snapshot before/after hashes match.

## Browser Validation Logging

Required. Record route, viewport, actions, console log, diagnostics JSON and screenshot path where visual output changed.

## Progression Gate

Critical semantic gate: negative patch must not mutate scene; positive patch must mutate exactly expected objects/links and revision once.

## Suggested Agent Prompt

Start `SB03` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB03/manifest.md` plus `proof/SB03/semantic-invariants.md` for critical work.
