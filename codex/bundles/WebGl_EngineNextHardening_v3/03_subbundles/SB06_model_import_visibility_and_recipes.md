# SB06 — Model Import Visibility and Recipes

## Goal

Make incorrectly converted or invisible GLB models diagnosable and recoverable without contaminating WebGlLib with asset-specific hacks.

## Current issue

Model diagnostics can detect no mesh, hidden meshes, transparent materials, near-zero/extreme bounds, and far-from-origin models. But the system still needs a cleaner recipe workflow for fixing import parameters per asset/variant.

## Tasks

1. Add import recipe support:
   ```text
   WebGlModelImportRecipe
   WebGlModelImportRecipeCatalog
   WebGlModelImportRecipeResolver
   ```
   Or extend existing `WebGlModelImportOptions` cleanly with catalog-level named presets.

2. Support per asset/variant:
   - `unitScale`
   - `fitMode`
   - `centerMode`
   - `fixedScale`
   - `rotationOffset`
   - `positionOffset`
   - `forceDoubleSidedMaterial`
   - `normalizeMaterialVisibility`
   - `debugBounds`
   - `disableTint`
   - optional `cameraPresetHint`
   - optional `knownIssueNotes`

3. Add Model Lab improvements:
   - show active import options,
   - allow editing import options locally in the UI,
   - export a suggested recipe JSON,
   - button to test primitive/model-low/model-high profiles,
   - show model bounds helper and axes helper,
   - show material transparency summary.

4. Add batch diagnostics:
   - route or tool that loads every registered GLB one by one,
   - records model diagnostics,
   - flags invisible or extreme models,
   - writes `artifacts/.../model-import-batch-diagnostics.json`.

5. Do not hardcode model-specific fixes in runtime modules.
   Store fixes in sandbox catalog/recipes.

## Done criteria

- Invisible GLBs are visible in diagnostics.
- Model Lab can produce a recipe candidate.
- Runtime fallback remains safe.
- WebGlLib remains generic.
