# Missing Generic Helpers Catalog

## WebGlLib helpers still needed or incomplete

1. `webgl-scene-command-results.js`
   - central command result factory
   - bounded command result history
   - consistent failure handling

2. `webgl-scene-asset-cache.js`
   - state-local cache lifecycle
   - optional future shared cache with ref counts
   - dispose cached templates on state dispose

3. `webgl-scene-scheduler.js`
   - true idle scheduler
   - no permanent rAF loop for idle static scenes

4. `webgl-scene-indexes.js`
   - object/link/layer/tag/asset indexes
   - avoid repeated linear scans in larger scenes

5. `webgl-scene-layer-visibility.js`
   - layer visibility and hit-test filtering

6. `webgl-scene-import-recipes.js`
   - import recipe resolution
   - model-specific import options outside runtime logic

7. `webgl-scene-document-normalizer.cs`
   - deterministic scene hash normalization
   - recursive metadata policy

8. `webgl-scene-validation.cs`
   - duplicate object id detection
   - dangling link detection
   - missing asset/fallback detection

9. `WebGlRunLib`
   - generic run timeline/playback over WebGlSceneModel
   - no domain semantics

## Economy repo helpers needed later

1. `CanDoItAll.Economy.Simulation.Abstractions`
2. `CanDoItAll.Economy.Simulation.SimpleAccounts`
3. `CanDoItAll.Economy.Simulation.Ledger`
4. `CanDoItAll.Economy.Simulation.Visualization`
5. Architecture tests for reference isolation.
