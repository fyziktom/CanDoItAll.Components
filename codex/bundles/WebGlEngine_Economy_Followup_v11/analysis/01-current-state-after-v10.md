# Current-state analysis after v10

## What improved

1. Generic `ResourceTransferVisual` has been renamed to `DirectedFlowVisual` in Components.
2. Economy bridge maps domain action `ResourceTransferVisual` to generic `DirectedFlowVisual` through a mapping driver.
3. Generic domain term scanning is no longer hardcoded inside the generic validator; `WebGlRunGenericBoundaryOptions` accepts external terms.
4. Economy owns a domain-specific boundary profile through `EconomyWebGlMappingBoundary`.
5. Pause/stop lifecycle now uses a stop coordinator that performs immediate runtime stop before C# playback drain.
6. `multi-goods-elite` scenario exists as a third canary scenario and is structurally different from shared-well and farmer-land.

## Remaining concerns

1. Domain leakage scanning is still partial. It scans configured C# roots and configured terms, but should include generated public constants, docs, tests, bundle artifacts and new terms introduced by the third canary.
2. Economy domain driver exists implicitly but should be formalized as a reusable cross-domain driver contract.
3. `multi-goods-elite` introduces semantically rich concepts such as credit, equity, claims, contribution, elite dependency and policy shock. These must be handled by clear semantic drivers, not just simple transfer aliases.
4. Browser proof must prove live browser state. It is not enough to compare expected documents or capture runtime diagnostics without document/scene hash verification.
5. Readiness claims must be artifact-derived. Manual booleans are acceptable only as low-level input but must not directly grant research-ready or observer-valid status.
6. Mutation/store-resolution logic remains concentrated in a large partial class and should be split to reduce bug-driven experimental noise.
