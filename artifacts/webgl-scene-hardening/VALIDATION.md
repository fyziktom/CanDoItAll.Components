# WebGL Scene Hardening Validation

Large-screen-only hard rule: browser validation used a large desktop viewport. No small-screen tuning was performed for this bundle.

## Commands

- `npm install`
- `npm run webgllib:inventory-glb`
- `npm run webgllib:build-assets`
- `npm run webgllib:verify-assets`
- `dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`
- `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj`
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
- `dotnet build CanDoItAll.Components.slnx`

## Browser Proof

Route: `/tycoon-village`

Stored artifacts:

- `browser-summary.json`: primitive, mixed GLB, high GLB, selection, drag, motion, export/import, and missing-asset fallback phases.
- `browser-final-proof.json`: final runtime snapshot and exported scene state.
- `browser-final-canvas.png`: WebGL canvas capture.
- `browser-console.log`: console transcript; only the known Three.js GLTF extension warning was present.

Final browser proof confirmed:

- `window.CanDoItAll.webglWorkbench` exists.
- `window.CanDoItAll.webglScene` exists.
- Primitive profile rendered 20 objects and 29 primitive instances.
- Mixed GLB profile rendered 5 loaded assets, 21 model instances, and no missing assets.
- High GLB profile rendered 12 loaded assets, 21 model instances, and no missing assets before the intentional fallback test.
- Canvas selection selected `building.house-b`.
- Drag moved `agent.runner` and fired `ObjectsMoved`.
- Motion command reached `{ x: 0.4, y: 0, z: -0.4 }`.
- Export/import preserved the runner position.
- Intentional missing asset reported `asset.missing.intentional` and kept the scene healthy.
