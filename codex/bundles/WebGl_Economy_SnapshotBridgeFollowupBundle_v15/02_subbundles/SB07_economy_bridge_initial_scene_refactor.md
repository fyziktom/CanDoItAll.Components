# SB07 — Economy bridge initial scene refactor

## Goal
Split the large initial scene projector into smaller units.

## Current concern
`EconomyWebGlInitialSceneProjector.cs` is near the production size threshold and mixes multiple responsibilities.

## Target split
- `EconomyWebGlLayerProjector`
- `EconomyWebGlNodeProjector`
- `EconomyWebGlLinkProjector`
- `EconomyWebGlSymbolProjector`
- `EconomyWebGlVisualStateCatalogProjector`
- `EconomyWebGlProjectionDiagnostics`

## Validation
- Existing bridge tests still pass.
- New unit tests cover each projector.

## Status
- Completed.

## Prerequisites
- SB06 strict mapping proof is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlInitialSceneProjector.cs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeTests.cs`

## Dependency Impact
- Reduces bridge maintenance risk before sandbox orchestration and final performance proof.

## Validation Depth
- Requires source split assertions and bridge test coverage for layer, node, link, symbol, catalog, and diagnostics paths.

## Acceptance Checklist
- Initial scene responsibilities are split into focused production types.
- Existing behavior is preserved.
- New or existing tests cover each responsibility.

## Proof Required
- `bundle://proof/SB07/manifest.md`
- Test transcript and source assertions for split projectors.

## Browser Validation Logging
- Browser validation is not required unless rendered bridge output changes.

## Progression Gate
- SB12 may rely on bridge maintainability only after initial-scene split proof is recorded.

## Suggested Agent Prompt
- Split the Economy initial scene projector into focused projectors while preserving bridge output and tests.
