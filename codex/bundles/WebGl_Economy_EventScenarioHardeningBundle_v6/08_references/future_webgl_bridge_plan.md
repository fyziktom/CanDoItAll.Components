# Future WebGL bridge boundary

Status: documentation only. No bridge package is implemented in this bundle.

## Proposed package

`CanDoItAll.Economy.Simulation.WebGlBridge`

## References

- References `CanDoItAll.Economy.Simulation.Visualization`.
- References `CanDoItAll.Components.WebGlRunLib`.
- Does not belong in `CanDoItAll.Economy.Simulation.Abstractions`, `Simulation.SimpleAccounts`, `Simulation.Ledger`, `Simulation.Visualization`, `CanDoItAll.Components.WebGlLib`, or `CanDoItAll.Components.WebGlRunLib`.

## Contract

Input:

- `EconomyVisualAction[]`
- optional node/place binding catalog owned by the host application

Output:

- `WebGlRunAction[]`
- diagnostics for unresolved nodes, anchors, poses, symbols, and host binding gaps

## Mapping sketch

- `move-to-target` -> `move-to-object`
- `return-to-origin` -> `return-to-anchor`
- `change-state` and `perform-work` -> `change-pose`
- `show-symbol` and `hide-symbol` -> symbol actions
- `transfer-resource` -> sequence or parallel actions chosen by the host bridge profile
- `sequence` and `parallel` -> nested `WebGlRunAction.Steps`

## Non-goals for this bundle

- No Economy project references Components.
- No Components project references Economy.
- No WebGL terms are added to Economy simulation projects.
- No shared-well or entrepreneur semantics are added to WebGlLib or WebGlRunLib.
