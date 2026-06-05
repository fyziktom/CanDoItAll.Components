# CanDoItAll Components WebGL Engine Stabilization Follow-up Bundle v14

**Bundle id:** `components-webgl-engine-stabilization-followup-v14`  
**Created:** `2026-06-05T22:36:37.038142+00:00`  
**Primary repository:** `fyziktom/CanDoItAll.Components` branch `webgl-engine`  
**Implementation scope:** Components only. Do not modify `CanDoItAll.Economy` in this bundle.

## Purpose

This follow-up bundle stabilizes the generic WebGL and WebGlRun engine in `CanDoItAll.Components` so future economic simulation work can continue mainly in `CanDoItAll.Economy` without repeatedly widening the generic engine.

The previous shared bundles were useful while the Economy use case was discovering missing rendering, playback, and proof contracts. The next step is different: turn Components into a release-candidate substrate with approved API surfaces, package boundaries, deterministic runtime proof, domain-driver boundaries, and CI gates. After this wave, Economy should consume these contracts through its domain driver and scenario/oracle work instead of pushing more domain-shaped features into Components.

## Non-negotiable direction

- Keep `WebGlLib` as a generic render substrate.
- Keep `WebGlRunLib` as generic run/playback/action-stage contracts over `WebGlLib`.
- Do not add Economy, manufacturing-line, market, account, investor, production, or scenario-specific semantics into Components.
- If a domain needs a term or mapping, it belongs in a domain driver in the consuming repository.
- Freeze the generic API after the release-candidate gates pass.
- Further Components work after freeze should be limited to bugfixes, documentation, CI/proof maintenance, and explicitly approved API changes.

## High-level findings

1. The solution includes both generic libraries and samples, which is good for freeze proof, but the global `IsPackable=true` deserves explicit package-scope hardening so sandbox/demo/test projects are not accidentally treated as distributable packages.
2. `WebGlSceneView` has become the most important boundary component and now owns a large amount of lifecycle/interoperability logic. This should be split behind stable interfaces/facades without breaking its public API.
3. `window.CanDoItAll.webglScene` is a de facto JavaScript public API. It needs a generated API manifest, approval file, and backward-compatibility checks.
4. Runtime idle is now more nuanced with semantic/visual/final-render-drained states. This is powerful, but it needs explicit proof modes so tests do not accidentally accept a visually unsettled runtime as fully idle.
5. The domain-driver contract exists and should now be frozen with manifest/hash compatibility rules.
6. Domain leakage audit exists but needs tighter source/package gates and controlled allowlist governance.
7. Generic samples exist, but `WebGlRunLibGenericSample` needs package-mode proof similar to the WebGlLib-only viewer.

## Expected output of Codex execution

Codex must produce a completed bundle inside `codex/bundles/components-webgl-engine-stabilization-followup-v14` in the Components repo with proof artifacts for every subbundle. The implementation must end with an RC signoff document declaring which APIs are frozen and which issues remain explicitly deferred.

## Execution rule

Implement subbundles in order. After SB08, perform a mandatory senior QA/refactor pass before proceeding. After SB14, perform the final release-candidate freeze signoff. Do not skip failing-first tests, package proofs, browser proofs, or domain-leakage audits.
