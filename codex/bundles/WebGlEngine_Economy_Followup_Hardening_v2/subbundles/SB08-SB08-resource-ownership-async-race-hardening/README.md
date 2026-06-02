# SB08 — Resource ownership async race hardening

## Objective

Stress and harden model resource ownership, pending GLB promises, cache disposal, and repeated browser recreate loops.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/16-webgl-scene-models.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/10-webgl-scene-lifecycle.js`

## Deliverables

- Async disposal tests or browser proof for pending asset promises.
- Resource ownership test covers shared texture retention and owned material disposal.
- Diagnostics prove no monotonic leak in repeated cycles beyond expected cache behavior.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- `npm run webgllib:test-resource-ownership` enhanced.
- Browser stress proof on high GLB profile.
- Console logs captured and reviewed for GLTF/load/dispose errors.

## Implementation Steps

- Add JS/browser tests for disposing scene while GLB loads are pending.
- Validate tinted material clones retain shared textures and do not dispose template textures.
- Validate repeated import/dispose/recreate cycles do not accumulate object groups, hit meshes, labels, or cache entries.
- Document extension rule for future global/shared asset cache.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB08 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- `npm run webgllib:test-resource-ownership` enhanced.
- Browser stress proof on high GLB profile.
- Console logs captured and reviewed for GLTF/load/dispose errors.

Critical subbundles must also create/update `proof/SB08/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB08 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB08/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
