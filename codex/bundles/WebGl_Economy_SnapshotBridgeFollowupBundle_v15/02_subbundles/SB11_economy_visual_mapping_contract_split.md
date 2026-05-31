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

## Status
- Completed.

## Prerequisites
- SB01 boundary baseline is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Experiment\EconomyVisualMappingDefinition.cs`
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests`

## Dependency Impact
- Keeps renderer-specific mapping details out of core simulation abstractions before backend-neutral sandbox orchestration.

## Validation Depth
- Requires boundary audit, semantic-key assertions, and tests covering whichever split decision is implemented.

## Acceptance Checklist
- A documented split decision is implemented.
- Abstractions do not reference Components/WebGL.
- Renderer-specific asset paths or WebGL labels live outside renderer-neutral contracts.

## Proof Required
- `bundle://proof/SB11/manifest.md`
- Economy test transcript and boundary/source assertions.

## Browser Validation Logging
- Browser validation is not required for contract split work.

## Progression Gate
- SB12 may proceed when mapping contract boundaries are proven.

## Suggested Agent Prompt
- Choose and implement the smallest visual mapping contract split that keeps abstractions renderer-neutral and bridge details bridge-owned.
