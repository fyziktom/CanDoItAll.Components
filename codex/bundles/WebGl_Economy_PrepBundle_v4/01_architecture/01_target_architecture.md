# 01. Target architecture: generic WebGL engine + economy simulation preparation

## Repository separation

```text
CanDoItAll.Components
  src/
    CanDoItAll.Components.WebGlLib
      Generic scene renderer.
      No domain semantics.

    CanDoItAll.Components.WebGlRunLib
      Generic visual run/playback contracts.
      May reference WebGlLib.
      No economy semantics.

    CanDoItAll.Components.WebGlSandbox
      Generic browser proof and model lab.

CanDoItAll.Economy
  src/
    CanDoItAll.Economy.Simulation.Abstractions
      Shared economy simulation contracts.

    CanDoItAll.Economy.Simulation.SimpleAccounts
      Simple-account backend for toy/community simulations.

    CanDoItAll.Economy.Simulation.Ledger
      Ledger-backed simulation adapter.

    CanDoItAll.Economy.Simulation.Visualization
      Economy visual-frame contracts without WebGL dependency.

    CanDoItAll.Economy.Simulator
      Existing simulator remains intact.
```

## No coupling in this wave

Do not add references between the two repositories yet.

Allowed:
- docs that say what the future adapter will do;
- DTOs that are intentionally mappable later;
- tests proving current project boundaries.

Forbidden:
- `CanDoItAll.Economy` package reference to `CanDoItAll.Components.WebGlLib`;
- `CanDoItAll.Components` reference to `CanDoItAll.Economy`;
- economy terms inside WebGlLib;
- ledger terms inside simple-account backend;
- simple-account types inside ledger backend.

## Boundary rule

If it renders, selects, imports, exports, patches, moves, or diagnoses generic 3D objects: it belongs in Components/WebGl.

If it defines how a community, market, company, well, or ledger evolves: it belongs in Economy.

If it is common across economy backends but not visual: it belongs in `Economy.Simulation.Abstractions`.

If it is common across economy backends and visual but not WebGL-specific: it belongs in `Economy.Simulation.Visualization`.

If it plays back arbitrary visual frames without knowing the domain: it belongs in `Components.WebGlRunLib`.
