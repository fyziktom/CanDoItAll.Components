# Source references

## C-WGRUN-ACTIONKINDS

- Repo: `fyziktom/CanDoItAll.Components`
- Ref: `webgl-engine`
- Path: `src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionKinds.cs`
- Finding: Generic action vocabulary now uses DirectedFlowVisual and exposes WebGlRunActionKinds.All; ResourceTransferVisual is removed.

## C-WGRUN-VALIDATOR

- Repo: `fyziktom/CanDoItAll.Components`
- Ref: `webgl-engine`
- Path: `src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
- Finding: Boundary options are configurable and validator now scans run id, initial scene ids/kinds/tags/symbols/links/layers, frame/stage metadata.

## C-DOMAIN-DRIVER

- Repo: `fyziktom/CanDoItAll.Components`
- Ref: `webgl-engine`
- Path: `src/CanDoItAll.Components.WebGlRunLib/DomainDrivers/WebGlRunDomainMappingDriver.cs`
- Finding: Generic domain driver contract, manifest, hash, metadata scrubber, and driver validator exist.

## C-DOMAIN-AUDIT

- Repo: `fyziktom/CanDoItAll.Components`
- Ref: `webgl-engine`
- Path: `tools/webgllib/domain-boundary-audit.config.json`
- Finding: Domain leakage CI scans source/docs/tools/workflows with term registry and explicit allowlists.

## E-BRIDGE-DRIVER

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs`
- Finding: Economy bridge implements EconomyWebGlRunDomainMappingDriver and maps ResourceTransferVisual to DirectedFlowVisual.

## E-MAPPING-BOUNDARY

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappingBoundary.cs`
- Finding: Economy-owned strict generic boundary terms include economy, ledger, market, buyer/seller, investor, elite, price, water, well.

## E-MULTIGOODS

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/multi-goods-elite/scenario.definition.json`
- Finding: Third canary scenario exists with grain/tools/credit/equity-share, exchange, contribution, claim issue, fee, policy, and obligation events.

## E-MULTIGOODS-INVARIANTS

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/multi-goods-elite/expected.invariants.json`
- Finding: Third canary has concentration, top share, trade volume, issue count, and elite capital dependency metrics/invariants.

## E-READINESS

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs`
- Finding: Readiness report now has evidence records and bands, but evidence validator validates record structure rather than reading/hash-checking files.

## E-MUTATIONS

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.Mutations.cs`
- Finding: Mutation layer still combines store resolution, transfers, severity/rejection policy, flow metadata, and store policy resolution.

## E-BOUNDARY-AUDIT

- Repo: `fyziktom/CanDoItAll.Economy`
- Ref: `main`
- Path: `scripts/audit-simulation-boundaries.ps1`
- Finding: Economy boundary script enforces reference boundaries, line-count gates, determinism, and example-specific term scans.

