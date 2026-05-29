# WebGL Symbolic Tycoon Sandbox Implementation Report

## Summary

Implemented an additive, domain-neutral WebGL scene layer beside the existing `WebGlWorkbench` runtime and added a standalone `CanDoItAll.Components.WebGlSandbox` app. The proof route `/tycoon-village` renders a small generic village with GLB-backed people/gears/markers, primitive fallback buildings/trees, status symbols, selection, hover, camera commands, and deterministic proof snapshots.

## Files Changed

- Added scene, asset, symbol, interaction, and interop contracts under `src/CanDoItAll.Components.WebGlLib/WebGl`.
- Added `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`.
- Added scene runtime modules under `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene`.
- Added scene CSS under `src/CanDoItAll.Components.WebGlLib/wwwroot/css/scene`.
- Updated WebGlLib asset manifest/build/verify scripts and generated asset include components.
- Added `src/CanDoItAll.Components.WebGlSandbox` with `/`, `/asset-catalog`, and `/tycoon-village`.
- Updated solution and README files.

## Asset Discovery

GLB inventory found:

- `1gears.glb`
- `gears.glb`
- `lowpoly_person_boxing.glb`
- `question_box.glb`

The sandbox maps these to generic logical ids. Missing exact categories such as houses and trees use primitive fallbacks.

## Validation Results

- `npm install`: passed
- `npm run webgllib:build-assets`: passed
- `npm run webgllib:verify-assets`: passed
- `dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`: passed
- `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj`: passed
- `dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj`: passed
- `dotnet build CanDoItAll.Components.slnx`: passed
- Forbidden dependency/domain scans: passed

## Browser Proof

Route: `http://localhost:5298/tycoon-village`

Proof snapshot:

- Objects: 20
- Links: 5
- Symbols: 9
- Loaded GLB assets: 4
- Fallback objects: 12
- Missing assets: 0
- Selected object after canvas click: `building.house-b`
- Canvas image export length: 323460
- Console: 0 errors, 1 Three.js GLTFLoader warning for a known GLB extension

Screenshots:

- `codex/bundles/WebGl_TycoonSandbox/proof/SB08/browser/webgl-tycoon-village-final-desktop.png`
- `codex/bundles/WebGl_TycoonSandbox/proof/SB08/browser/webgl-tycoon-village-final-mobile.png`
- `codex/bundles/WebGl_TycoonSandbox/proof/SB08/browser/webgl-tycoon-village-symbols-visible.png`

## Known Limitations

- Scene runtime is an MVP: no physics, pathfinding, particle systems, or animation blending.
- GLB material warning is emitted by Three.js for an extension in an existing model, but it does not prevent render or validation.
- Primitive fallbacks are intentionally simple and generic.

## Future Integration Notes

Future economy/process modules should map their domain data into `WebGlSceneModel` from outside WebGlLib. Keep WebGlLib free of domain-specific concepts and put domain-specific symbol policy/mapping in the consuming repo.

