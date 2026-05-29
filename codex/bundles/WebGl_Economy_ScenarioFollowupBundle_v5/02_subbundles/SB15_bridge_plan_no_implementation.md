# SB15 - Future bridge plan, no implementation yet

Repositories: both

## Goal

Document the bridge that will eventually connect Economy visual actions to WebGlRunLib, but do not implement the bridge yet.

Future bridge package candidate:

```text
CanDoItAll.Economy.Simulation.WebGlBridge
```

Allowed future references:

```text
Economy.Simulation.WebGlBridge
  -> Economy.Simulation.Visualization
  -> CanDoItAll.Components.WebGlRunLib
  -> CanDoItAll.Components.WebGlLib
```

Forbidden now:

- Do not add this bridge in the current wave.
- Do not add WebGL references to `Simulation.Visualization`.
- Do not add Economy references to `Components`.

## Bridge responsibility later

- map `EconomyVisualFrame` to initial `WebGlSceneDocument`;
- map `EconomyVisualAction` to `WebGlRunAction`;
- map locations to scene coordinates;
- map categories to asset ids/symbol ids/pose keys;
- choose model/asset policy.
