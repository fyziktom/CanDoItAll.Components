# SB10 - Visual mapping contract boundary

## Goal
Keep generic visual mapping renderer-neutral until the WebGlBridge layer.

## Required actions

1. Review `EconomyVisualMappingDefinition` location and contents.
2. Split the broad file into smaller files by contract family.
3. Remove or rename WebGL-specific validator terminology from abstractions where possible.
4. Keep asset IDs as abstract visual asset keys in visualization layer; resolve to WebGL assets only in bridge/mapping loader.
5. Add validation that forbids example-specific names in generic mapping categories unless in fixtures.

## Acceptance criteria

- Mapping contracts can support non-WebGL visualization in the future.
- WebGL-specific logic remains in bridge or Components, not Economy abstractions.
