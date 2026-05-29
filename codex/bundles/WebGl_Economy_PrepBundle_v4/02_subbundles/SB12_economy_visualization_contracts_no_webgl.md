# SB12 — Economy visualization contracts without WebGL dependency

## Goal

Prepare economy visual frames that can later map to WebGlRunLib/WebGlLib, without coupling now.

## Project

Create:

`src/CanDoItAll.Economy.Simulation.Visualization`

References:
- `Simulation.Abstractions`

## Contracts

Add:

- `EconomyVisualFrame`
- `EconomyVisualNode`
- `EconomyVisualLink`
- `EconomyVisualSymbol`
- `EconomyVisualLayer`
- `EconomyVisualLayoutHint`
- `EconomyVisualFrameDelta`
- `IEconomyVisualFrameMapper`

## Semantic categories

Use generic economy terms only:
- actor;
- resource;
- institution;
- rule;
- market;
- obligation;
- trust;
- conflict;
- risk;
- collaboration;
- scarcity.

No WebGL types.

## Validation

- tests map shared-well frame to visual frame;
- tests map entrepreneur frame to visual frame;
- dependency scan proves no Components/WebGl references.
