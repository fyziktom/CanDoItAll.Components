# SB08 - Generic tycoon village demo

## Goal

Render a small generic village scene using repository GLB assets and the new generic scene contracts.

## Scene content

Create a factory:

```text
src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxVillageSceneFactory.cs
```

It should return `WebGlSceneModel`.

The scene should include:

- Village ground/grid.
- Several houses/buildings.
- Trees/props if assets exist.
- Several low-poly people/agents if assets exist.
- At least 3 status symbols above objects.
- A few generic links/paths/flows.
- A tycoon/isometric camera preset.

## Asset discovery

Before hardcoding asset ids, inventory GLB files:

```powershell
Get-ChildItem -Path src -Recurse -Filter *.glb | Select-Object FullName
```

Map existing assets into generic ids:

```text
asset.agent.person.default
asset.building.house.default
asset.building.service.default
asset.prop.tree.default
asset.symbol.warning.default
asset.symbol.info.default
asset.symbol.energy.default
```

If exact assets are missing, use primitive fallbacks.

## Example symbolic semantics

These are generic examples, not economy-specific:

```text
alert
busy
available
needs-input
ready
blocked
warning
info
```

Do not use economy-specific terms like debt/trust/conflict.

## UI

The `/tycoon-village` page should include:

- Left or top toolbar:
  - Fit view
  - Reset camera
  - Toggle symbols
  - Toggle labels
  - Capture proof snapshot
- Main WebGL scene.
- Inspector panel for selected object.
- Diagnostics/proof panel.

## Acceptance criteria

- Page renders a visually recognizable small village.
- Symbols appear above some objects.
- At least one symbol uses a GLB asset if available, otherwise fallback primitive.
- Click selection updates inspector.
- Proof snapshot includes object count, symbol count, asset counts, selected object id.
