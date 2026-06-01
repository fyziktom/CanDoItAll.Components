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

## Status

Completed.

## Goal

Continue bridge projector decomposition while preserving behavior and adding reusable diagnostics aggregation.

## Prerequisites

- SB05 strict validation must pass or be explicitly reopened with compatible diagnostics.

## Owned Requirements

- R06 Economy Bridge Refactor.

## Dependency Impact

Supports SB07 renderer-neutral mapping and SB08 runner diagnostics.

## Validation Depth

Refactor with focused tests for each projector and diagnostics aggregator.

## Proof Required

- Economy test transcript for isolated projector tests.
- Source assertions showing mapping, validation, and command generation are separated.
- Proof manifest.

## Progression Gate

Pass only when projectors remain separated and no method mixes mapping, validation, and command generation responsibilities.
