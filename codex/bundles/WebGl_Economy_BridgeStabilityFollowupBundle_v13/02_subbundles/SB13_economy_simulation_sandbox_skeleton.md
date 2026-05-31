# SB13 — Economy SimulationSandbox skeleton

## Goal
Prepare where the future joined version lives without building the full demo.

## Tasks
- Add `CanDoItAll.Economy.SimulationSandbox` or document why existing `CanDoItAll.Economy.Sandbox` will host it.
- It may reference:
  - Simulation.Abstractions
  - Simulation.SimpleAccounts
  - Simulation.Visualization
  - Simulation.WebGlBridge
  - Components WebGl UI package/project
- It must not move bridge code into Components.
- Add a compile-only page/service skeleton:
  - load input pack
  - materialize simulation
  - map visual frames/actions
  - project WebGlRunDocument
  - display diagnostics placeholder

## Tests
- build succeeds.
- boundary audit confirms only sandbox depends on UI components beyond bridge.
