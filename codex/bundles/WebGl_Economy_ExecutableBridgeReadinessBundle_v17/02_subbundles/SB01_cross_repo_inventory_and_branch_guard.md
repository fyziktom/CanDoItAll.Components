# SB01 — Cross-repo inventory and branch guard

## Goal

Confirm current branches, repository roots, and dependency boundaries before editing.

## Required actions

- Do not create or switch to a new branch.
- Record current branch and latest commit for both repos.
- Confirm `CanDoItAll.Components` branch is the currently checked-out branch, expected `webgl-engine` unless user changed it.
- Confirm `CanDoItAll.Economy` branch is current `main` or user-selected branch.
- Record project inventory for:
  - `CanDoItAll.Components.WebGlLib`
  - `CanDoItAll.Components.WebGlRunLib`
  - `CanDoItAll.Economy.Simulation.Abstractions`
  - `CanDoItAll.Economy.Simulation.Visualization`
  - `CanDoItAll.Economy.Simulation.WebGlBridge`
  - `CanDoItAll.Economy.SimulationSandbox`

## Closure proof

- `proof/SB01/manifest.md`
- branch transcript
- project inventory transcript
- dependency graph transcript
