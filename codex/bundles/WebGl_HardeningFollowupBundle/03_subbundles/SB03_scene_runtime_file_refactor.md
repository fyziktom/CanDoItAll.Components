# SB03 - Scene runtime file refactor

## Goal

Make the JS runtime maintainable before adding run/timeline features.

## Refactor targets

Current high-risk files:

```text
01-webgl-scene.js
03-webgl-scene-assets.js
sandbox-webgl.css
TycoonVillage.razor
WebGlSandboxVillageSceneFactory.cs
```

## Tasks

1. Split lifecycle/public API from scene graph.
2. Split primitive generation from GLB/model loading.
3. Move drag logic into a dedicated module.
4. Move patch/motion logic into dedicated modules.
5. Split sandbox inspector and proof panel into Razor components.
6. Split sandbox CSS by route/component.

## Acceptance criteria

- No ordinary JS module exceeds 250 lines unless justified.
- `01-webgl-scene*.js` becomes mostly composition/public API.
- Asset primitives are isolated and testable.
- Sandbox page is readable and under ~180 lines.
