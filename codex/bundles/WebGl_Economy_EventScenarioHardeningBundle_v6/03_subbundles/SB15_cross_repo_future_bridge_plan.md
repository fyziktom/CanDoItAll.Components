# SB15 - Cross-repo future bridge plan, not implementation

Create documentation only.

Define a future integration package boundary:

- possible package: `CanDoItAll.Economy.Simulation.WebGlBridge`
- input: `EconomyVisualAction[]`
- output: `WebGlRunAction[]`
- references: Economy Visualization + Components WebGlRunLib

Do not implement the bridge now.

The purpose is to prevent accidental direct references between Economy and Components in this preparation phase.
