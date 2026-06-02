# Large Simulation Performance And Resource Budget Architecture

## Goal

Prepare WebGlLib/WebGlRunLib for larger Economy and future production-line simulations without pretending the current small demos prove enough.

## Proposed budgets

- Max scene objects for default interactive mode.
- Max active GLB model instances.
- Max loaded asset templates.
- Max retained shared textures.
- Max active motions and queued motions.
- Max command stages per frame.
- Max command journal entries.
- Max average frame time before warning.
- Optional "degraded mode" profile that forces primitive or low-model rendering.

## Diagnostics

Expose budget diagnostics in both JS and C#:
- `resourceBudgetProfile`
- `resourceBudgetExceeded`
- `budgetWarnings`
- `assetEvictionCount`
- `degradedRenderingMode`
- `largeSceneObjectCount`
- `largeSceneStageCount`

## Proof

Add a synthetic large run document and a browser stress proof that asserts budget warnings and no crash/no runaway context growth.
