# SB06 - Economy bridge projector refactoring

The split of initial scene projector is good. Continue:

- keep layer, node, link, symbol, visual-state-catalog projectors separate,
- keep action-stage projection separate,
- add bridge diagnostics aggregator,
- no method should mix mapping, validation, and command generation responsibilities,
- add tests for each projector in isolation.

Watch these files:

- `EconomyWebGlActionStageProjector.cs`
- `EconomyWebGlBridgeContracts.cs`
- `EconomyWebGlRunValidator.cs`
