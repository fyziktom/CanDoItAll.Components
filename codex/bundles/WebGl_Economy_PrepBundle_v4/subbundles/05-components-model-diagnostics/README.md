# SB05 Components Model Diagnostics Batch Report

## Status

- Status: Completed

## Objective

- Produce deterministic batch diagnostics and import recipes for catalog model assets.

## Covered Inputs

- `bundle://02_subbundles/SB05_components_model_diagnostics_batch.md`
- Components review risk R3.

## Prerequisites

- SB04 browser-rendering and scheduler proof is trusted.

## Exact Source References

- `bundle://02_subbundles/SB05_components_model_diagnostics_batch.md`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/ModelLab.razor`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/18-webgl-scene-model-diagnostics.js`

## Deliverables

- `repo://artifacts/webgl-engine-prep-v4/model-diagnostics.json`
- `repo://artifacts/webgl-engine-prep-v4/model-diagnostics.md`

## Dependency Impact

- Makes external GLB visibility failures diagnosable before future domain visualization.

## Validation Depth

- Browser/model diagnostics must not crash on missing or fallback models.

## Implementation Steps

- Add deterministic batch diagnostics, import recipe fields, and report export.

## Do Not Do

- Do not add economy scenarios to the generic sandbox.

## Acceptance Checklist

- Reports include asset id, variant id, uri, mesh/material counts, bounds, warnings/errors, and suggested recipe.

## Proof Required

- Diagnostics artifacts, browser/model rendering screenshot, and command transcript.

## Browser Validation Logging

- Capture ModelLab or diagnostics route screenshot and report generation result.

## Progression Gate

- Proceed to SB06 when model proof is nonblank and diagnostics artifacts exist.

## Suggested Agent Prompt

- Operationalize GLB diagnostics without adding domain-specific model semantics.

