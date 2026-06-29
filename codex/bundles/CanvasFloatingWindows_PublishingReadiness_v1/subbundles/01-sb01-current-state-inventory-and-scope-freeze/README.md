# SB01 Current State Inventory And Scope Freeze

## Status

- `Completed`

## Objective

Freeze the Canvas/Floating Windows publishing scope, regenerate the durable inventory, record prior-bundle pattern decisions, and prove WebGL implementation remains outside this bundle.

## Covered Inputs

- RAW01: Reuse the basic-components publishing preparation pattern.
- RAW02: Study the system from recent bundles.
- RAW03: Prepare a new Canvas and floating-windows bundle.
- RAW04: Do not do WebGL part yet.
- R01, R02, R03, R13.

## Prerequisites

- Prepared bundle files exist.
- Repository source is readable from the Components repo root.
- Prior bundle artifacts are available under `codex/bundles`.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib
- repo://src/CanDoItAll.Components.OverlayLib
- repo://src/CanDoItAll.Components.Sandbox
- repo://tools/canvaslib
- repo://tests/CanDoItAll.Components.BaseLib.Tests
- repo://codex/bundles/StandardComponents_PublishingReadiness_v1
- repo://codex/bundles/WebGlEngine_Stabilization_v17

## Deliverables

- Canvas/Floating Windows inventory JSON and xlsx or markdown map.
- Current-state findings updated from fresh repo inspection.
- WebGL exclusion source assertion.
- Prepared-stage validator transcript.
- SB01 proof manifest and semantic invariants.

## Dependency Impact

- SB02-SB10 depend on the inventory and scope boundary.
- If any Canvas, Overlay, sandbox, asset, package, or test surface is missed, later proof may close the wrong behavior.
- If WebGL exclusion is wrong, the bundle violates the raw request.

## Validation Depth

- Critical foundation.
- Structural validator plus inventory verifier.
- Semantic Adequacy Gate with shallow-pass trap, adversarial negative proof, semantic positive proof, anti-stub audit, and raw-note literal closure.

## Implementation Steps

1. Re-scan CanvasLib, OverlayLib, Sandbox Canvas/Overlay pages, tests, tools, package files, docs, and prior bundles.
2. Generate `inventories/current-state-data.json` and `inventories/canvas-floating-windows-publishing-map.xlsx` or document why xlsx generation is blocked.
3. Update `analysis/01-current-state.md` and `inventories/01-scope-inventory.md` if repo state changed.
4. Assert that no WebGL implementation file is planned for modification.
5. Run the prepared-stage validator and save output to `reviews/prepared-validation.txt`.
6. Create `proof/SB01/manifest.md` and `proof/SB01/semantic-invariants.md`.

## Scope Exceptions

- WebGL bundle files may be cited as pattern evidence only.
- No Canvas or Overlay source implementation changes are required in SB01 unless needed for inventory scripts.

## Do Not Do

- Do not edit WebGL source.
- Do not refactor Canvas or Overlay behavior.
- Do not treat the existing Canvas benchmark as renderer-migration approval.

## Acceptance Checklist

- Inventory names CanvasLib, OverlayLib, sandbox routes, tests, assets, package metadata, docs, and known missing proof.
- WebGL exclusion is explicit.
- Every raw note maps to requirements and subbundles.
- Prepared-stage validator passes.

## Proof Required

- `bundle://reviews/prepared-validation.txt`
- `bundle://inventories/current-state-data.json`
- `bundle://inventories/canvas-floating-windows-publishing-map.md`
- `bundle://proof/SB01/manifest.md`
- `bundle://proof/SB01/semantic-invariants.md`
- Transcripts for inventory generation, source assertions, and anti-stub audit.

## Browser Validation Logging

- N/A for implementation; this phase does not change browser-visible behavior.
- If a route smoke is used to prove sandbox route existence, log route, viewport, screenshot path, and result in the execution report.

## Progression Gate

- SB02-SB04 may start only after SB01 passes prepared-stage validation and the inventory/source-boundary manifest exists.
- Reopen SB01 if any later phase discovers an unowned Canvas, Overlay, sandbox, asset, package, or WebGL boundary item.

## Suggested Agent Prompt

```text
Execute SB01 only. Regenerate and verify the Canvas/Floating Windows inventory, prove WebGL exclusion, run the prepared-stage validator, create SB01 proof artifacts, update execution report rows, and stop before source refactors.
```
