# SB01 - Inventory and guardrails

## Goal

Before coding, inspect current WebGL implementation and identify all files that must remain compatible.

## Tasks

1. List current WebGlLib files:
   - C# DTOs
   - Razor components
   - JS runtime files
   - CSS
   - vendored Three.js files
   - GLB assets
   - build/verify asset scripts

2. Confirm whether `CanDoItAll.Components.WebGlSandbox` already exists.
   - If not, create it.
   - If yes, extend it.

3. Identify all usages of:
   - `WebGlWorkbench`
   - `WebGlWorkbenchSurface`
   - `WebGlLibHeadAssets`
   - `WebGlLibBodyAssets`
   - `window.CanDoItAll.webglWorkbench`

4. Add a short architecture note:
   - Existing workbench runtime remains stable.
   - New generic scene runtime is additive.
   - Future economy visualization will consume generic scene contracts.

## Output

Create:

```text
artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md
```

Include:

- Existing file map.
- Current GLB asset list.
- Current public APIs that must not break.
- Proposed new file list.
