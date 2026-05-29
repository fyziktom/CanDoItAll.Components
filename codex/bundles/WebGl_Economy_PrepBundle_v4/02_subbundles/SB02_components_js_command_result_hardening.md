# SB02 — Components JS command result hardening

## Goal

Centralize command-result creation for WebGL scene JS modules.

## Tasks

1. Add `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js`.
2. Move common command result construction out of:
   - `13-webgl-scene-patching.js`
   - `14-webgl-scene-motion.js`
3. Ensure all command results have:
   - `commandId`
   - `success`
   - `succeeded`
   - `sceneId`
   - `commandKind`
   - `revision`
   - `errors`
   - `warnings`
   - `affectedObjectIds`
   - `affectedLinkIds`
   - `diagnostics`
   - `metadata`
4. Fix the add-object-without-id patch path so it never calls helpers with wrong argument shape.
5. Add JS audit checks that patch/motion do not define private duplicate `commandResult` functions.

## Validation

- `npm run webgllib:audit-scene-runtime`
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
