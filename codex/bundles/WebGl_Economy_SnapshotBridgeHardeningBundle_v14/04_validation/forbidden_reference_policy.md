# Forbidden reference policy

## Components

Forbidden:
- any reference to `CanDoItAll.Economy`
- any domain vocabulary in WebGL runtime JS:
  - economy
  - ledger
  - account
  - water
  - well
  - farmer
  - land
  - oligarchy
  - entrepreneur

## Economy

Allowed:
- `Simulation.WebGlBridge` may reference Components `WebGlRunLib`.
- Future `SimulationSandbox` may compose backend + visualization + bridge + UI.

Forbidden:
- `Simulation.Abstractions` -> Components/WebGL
- `Simulation.Visualization` -> Components/WebGL
- `Simulation.SimpleAccounts` -> Components/WebGL
- `Simulation.Ledger` -> Components/WebGL
- `Simulation.WebGlBridge` -> SimpleAccounts or Ledger backend projects
