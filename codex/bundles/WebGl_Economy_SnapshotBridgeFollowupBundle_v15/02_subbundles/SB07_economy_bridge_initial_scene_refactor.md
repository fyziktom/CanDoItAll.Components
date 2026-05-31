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
