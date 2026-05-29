# SB01 - Inventory and regression guard

## Goal

Create a source-grounded inventory of the current WebGL implementation and protect existing workbench behavior.

## Tasks

1. Compare `main` with `codex/webgl-symbolic-tycoon-sandbox`.
2. Inventory:
   - all `WebGlLib/WebGl` DTOs,
   - all `wwwroot/js/runtime/scene/*.js`,
   - all `WebGlSandbox` pages/factories/CSS,
   - all `.glb` and `.gltf` files under the repo,
   - all asset catalog IDs.
3. Produce `artifacts/webgl-scene-hardening/01_INVENTORY.md`.
4. Add a regression proof that both JS namespaces exist:
   - `window.CanDoItAll.webglWorkbench`
   - `window.CanDoItAll.webglScene`
5. Confirm `CanDoItAll.Components.slnx` still contains both `WebGlLib` and `WebGlSandbox`.

## Acceptance criteria

- Inventory lists all GLB/GLTF files, including user-provided models not yet used.
- Inventory identifies which assets are used by current sandbox and which are unused.
- Existing workbench runtime remains untouched except shared asset include changes.
