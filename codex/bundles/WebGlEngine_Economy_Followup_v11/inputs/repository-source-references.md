# Repository source references

- Components: `src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionKinds.cs` — generic action vocabulary now includes `DirectedFlowVisual`, not `ResourceTransferVisual`.
- Components: `src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` — generic boundary options now accept external forbidden term configuration.
- Components: `.github/workflows/domain-leakage.yml` and `tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/domain-leakage-terms.json` — domain leakage CI exists but needs broader/configurable coverage.
- Components: `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunPlaybackStopCoordinator.cs` — immediate stop/cancel/final stop/late drain contract exists.
- Economy: `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappingBoundary.cs` — Economy-owned forbidden term list and boundary options.
- Economy: `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` — Economy visual actions map through domain mapping driver.
- Economy: `src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/multi-goods-elite/*` — third canary scenario.
- Economy: `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine*.cs` — core simple account semantics and mutation hot spot.
- Economy: `scripts/audit-simulation-boundaries.ps1` — current boundary/line-count/domain-term audit.
