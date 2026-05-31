# SB11 — Visual mapping contract split

## Goal
Keep `Simulation.Abstractions` from becoming a rendering-mapping dumping ground.

## Current concern
`EconomyVisualMappingDefinition.cs` is large and contains visual-to-asset mapping details.

## Required decision
Choose one:
1. Keep in `Simulation.Abstractions`, but split into multiple files and keep it renderer-neutral.
2. Create `CanDoItAll.Economy.Simulation.Visualization.Abstractions`.
3. Move renderer-specific asset mapping into `Simulation.WebGlBridge`.

## Validation
- Abstractions must not reference Components/WebGL.
- Visual mapping keys should be semantic, not `.glb`, `webgl`, or raw asset file paths.
