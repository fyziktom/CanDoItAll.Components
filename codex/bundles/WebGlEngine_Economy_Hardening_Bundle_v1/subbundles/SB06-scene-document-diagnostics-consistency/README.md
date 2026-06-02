# SB06 — Scene document, layer, validator and diagnostics consistency

## Status

Completed

## Objective

Harden generic scene validation, layer membership, typed diagnostics parity and public docs.

## Covered Inputs

- Normalized requirements: REQ-008, REQ-009, REQ-001
- User requirement: generic engine hardening across Components and Economy with forced refactor gates.

## Prerequisites

SB03 completed, SB04 completed

## Exact Source References

- `SRC-CW-004`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModel.cs` (webgl-engine lines 5-32, 34-55) — Scene model has top-level Revision and UiState.Revision; objects, links, layers, asset catalog, camera, interaction, metadata.
- `SRC-CW-010`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentSerializer.cs` (webgl-engine lines 8-50) — Scene document serializer normalizes, hashes, serializes, deserializes and validates WebGlSceneDocument.
- `SRC-CW-011`: `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` (webgl-engine lines 9-191) — Document tests cover roundtrip, forbidden run metadata, hash invariants, order-insensitive hashing, invalid vectors, missing asset warnings.
- `SRC-CW-001`: `repo://CanDoItAll.Components/README.md` (webgl-engine lines 11-21, 32-47) — Packages table and WebGL sandbox routes; WebGlLib is generic scene/asset/symbol/interaction/proof contracts and WebGlSandbox is proof host.
- `SRC-CW-002`: `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` (webgl-engine lines 35-88) — WebGlWorkbench remains node/edge oriented; generic scene layer is additive; WebGlLib remains render substrate; simulation belongs in future WebGlRunLib or consuming package.

## Deliverables

- Add WebGlSceneModelValidator or extend document validator for live scene validation.
- Decide canonical layer membership source and clean stale layer object IDs on object removal.
- Align WebGlRuntimeDiagnostics with JS buildDiagnosticsSnapshot or intentionally mark extra JS-only diagnostics.
- Update README/package docs, including WebGlRunLib in root package map.

## Dependency Impact

- Phase: WebGlLib hardening
- Repository scope: Components
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

None planned. If current repo state makes a deliverable obsolete, record it in `proof/SB06/manifest.md` and update traceability before closing.

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
- [x] Proof artifacts are stored under `proof/SB06/`.
- [x] Traceability and execution report are updated.
- [x] No scope leakage into downstream subbundles.

## Closure Notes

Completed in `proof/SB06/manifest.md`. Browser validation is N/A for a fresh route run because SB06 changed no browser-visible JS/runtime behavior; the diagnostics capture shape is covered by C# deserialization tests and the JS/C# diagnostics parity scan.

## Proof Required

- Unit tests for duplicate layer membership, stale layer object ids, invalid vectors, missing assets, diagnostics deserialization.
- dotnet test Components WebGlLib tests --filter Document|Diagnostics|Layer|Validator.
- Browser diagnostics capture deserializes to C# WebGlRuntimeDiagnostics without losing critical counters.

## Browser Validation Logging

Required when browser-visible/runtime behavior is touched; otherwise record N/A with justification.

## Progression Gate

Refactor gate: public DTO additions must remain backward compatible or include schema migration notes.

## Suggested Agent Prompt

Start `SB06` only after confirming prerequisites. Read this README, source references, `plan/01-phase-plan.md`, and `traceability/01-requirement-traceability.md`. Implement only the scoped deliverables. Create failing-first or adversarial proof before the fix where practical, then implement, validate, complete the mandatory refactor gate, and update `proof/SB06/manifest.md` plus `proof/SB06/semantic-invariants.md` for critical work.
