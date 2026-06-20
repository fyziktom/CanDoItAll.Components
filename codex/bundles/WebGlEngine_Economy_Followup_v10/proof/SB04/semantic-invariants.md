# Semantic invariants SB04

## Invariants

- SB04-I01: Economy can keep `ResourceTransferVisual` only inside Economy-owned visualization and bridge code.
- SB04-I02: Generic runtime execution receives `DirectedFlowVisual`, not an Economy-shaped action name.
- SB04-I03: Strict fixture projection must remain valid after the mapping change.

## Semantic Adequacy Gate

- Shallow-pass trap: renaming the constant but leaving strict fixtures untested.
- Adversarial negative proof: `bundle://proof/SB04/source-scan-economy-no-generic-resource-transfer.txt`.
- Semantic positive proof: `bundle://proof/SB04/economy-webglbridge-phase-a-test.txt` passes 20 strict mapping tests, including `MultiGoodsEliteFixtureProjectsWithRendererBindingAndStrictGenericRunBoundary`.
- Anti-stub audit: `bundle://proof/SB04/anti-stub-scan.txt`.
- Changed source hashes: `bundle://proof/SB04/changed-file-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB04-I02 | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappedActionValidator.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs` | Domain action is translated to a generic directed-flow action before entering the generic run compiler. | `bundle://proof/SB04/source-scan-economy-no-generic-resource-transfer.txt` |
