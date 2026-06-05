# Semantic invariants SB03

## Invariants

- SB03-I01: Generic Components owns only run primitives and driver extension points.
- SB03-I02: Economy owns Economy action vocabulary, boundary terms, and mapping policy.
- SB03-I03: The generic driver hook must be usable without seeding Economy terms into generic Components.

## Semantic Adequacy Gate

- Shallow-pass trap: adding a marker interface while the actual mapper still switches directly to a domain-shaped generic action.
- Adversarial negative proof: `bundle://proof/SB04/source-scan-economy-no-generic-resource-transfer.txt` confirms no bridge code maps to `WebGlRunActionKinds.ResourceTransferVisual`.
- Semantic positive proof: `bundle://proof/SB04/economy-webglbridge-phase-a-test.txt` includes `EconomyDriverMapsDomainOwnedFlowActionToGenericDirectedFlow`.
- Anti-stub audit: `bundle://proof/SB03/anti-stub-scan.txt`.
- Changed source hashes: `bundle://proof/SB03/changed-file-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB03-I02 | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappedActionValidator.cs`; `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` | Economy driver maps domain actions to generic actions before validation and projection. | `bundle://proof/SB04/source-scan-economy-no-generic-resource-transfer.txt` |
