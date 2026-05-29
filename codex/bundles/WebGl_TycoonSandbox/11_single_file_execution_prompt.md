# Single-file execution prompt

Use this prompt if you want one compact Codex instruction instead of the full workflow.

```text
Work in C:\repositories\CanDoItAll.Components.

Create branch feature/webgl-symbolic-tycoon-sandbox.

Extend src/CanDoItAll.Components.WebGlLib with additive, domain-neutral WebGL scene support suitable for tycoon-style visualizations. Keep existing WebGlWorkbench stable.

Add generic C# contracts and services for Scene, Assets, Symbols, Interaction, and Interop:
- WebGlSceneModel/Object/Link/Camera/Environment/UiState/ProofSnapshot
- WebGlAssetCatalog/Definition/Variant/Animation and provider/validator services
- WebGlStatusSymbol, symbol effects, intensity policy, default symbol policy
- Scene selection/hover/move/command event models
- Runtime diagnostics/options/ready/error models

Add Components/Scene/WebGlSceneView.razor.

Add new JS runtime under wwwroot/js/runtime/scene exposed as window.CanDoItAll.webglScene. It must render scene objects, load GLB/GLTF via asset catalog, use primitive fallbacks, render symbols above objects, support hover/selection, fit/reset/focus camera, overlays, diagnostics, proof snapshot, and clean dispose.

Update WebGlLib asset inclusion so scene runtime scripts can be loaded without breaking existing workbench runtime.

Create standalone src/CanDoItAll.Components.WebGlSandbox project with only BaseLib, OverlayLib, and WebGlLib references. Add it to CanDoItAll.Components.slnx.

Create /tycoon-village page that renders a small generic village scene from available GLB assets in the repo. Inventory GLB files first; map closest assets to generic building/prop/agent/symbol ids; use primitive fallbacks if missing. The demo must show buildings, props, agents, status symbols above objects, color/intensity variation, selection, inspector, and proof snapshot.

Do not add any economy-specific or process-specific concepts. No dependency on CanDoItAll.Modules.Processes, CanDoItAll.Economy, or the main CanDoItAll app. All source code comments must be in English.

Validate with:
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx

Create artifacts/webgl-symbolic-tycoon-sandbox/IMPLEMENTATION_REPORT.md with file changes, architecture summary, validation results, browser proof results, known limitations, and follow-up recommendations.
```
