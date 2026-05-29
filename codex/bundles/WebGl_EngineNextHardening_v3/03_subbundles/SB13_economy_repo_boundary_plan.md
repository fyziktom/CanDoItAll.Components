# SB13 — Future Economy Repo Boundary Plan

This subbundle is a planning artifact for the next repository, not an instruction to add economy semantics to Components.

## Current Economy repo facts

`CanDoItAll.Economy.slnx` already contains separate projects for `Economy.Accounts`, `Economy.Ledger`, `Economy.Simulator`, `Economy.Simulator.Components`, persistence, SDK, markets, memory, etc. The simulator documentation states that simulator-specific concepts should stay above Core/Ledger/BusinessObjects/SDK/Node/CLI.

## Required future separation

Add a future project:

```text
src/CanDoItAll.Economy.Simulation.Abstractions/
```

Dependency rule:

```text
Simulation.Abstractions -> no Ledger, no Accounts, no SDK, no persistence, no UI
Simulation.SimpleAccounts -> Simulation.Abstractions + Economy.Accounts
Simulation.Ledger -> Simulation.Abstractions + Economy.Ledger
Simulation.Visualization -> Simulation.Abstractions + WebGlRunLib/WebGlLib package
```

## Shared abstractions allowed

- `EconomicSimulationRunId`
- `EconomicScenarioId`
- `EconomicActorId`
- `EconomicResourceId`
- `EconomicFlowId`
- `EconomicSimulationClock`
- `EconomicSimulationStep`
- `EconomicObservation`
- `EconomicActorSnapshot`
- `EconomicResourceSnapshot`
- `EconomicFlowSnapshot`
- `EconomicRunFrame`
- `EconomicRunIssue`
- `EconomicRunArtifactRef`
- `IEconomicScenarioCompiler`
- `IEconomicRunFrameSource`
- `IEconomicSimulationBackend`
- `IEconomicVisualizationProjector`

## Explicitly forbidden in shared abstractions

- ledger transaction classes
- UTXO assumptions
- simple-account balance mutation classes
- EF Core
- UI components
- WebGL runtime details
- scenario-specific well/community/business logic

## Future shared-well proof

Implement later in Economy repo:
- Abstractions define actors/resources/flows.
- SimpleAccounts backend updates balances and resource quantities.
- Ledger backend maps the same scenario to ledger-backed transactions.
- Visualization projector maps run frames to generic WebGlRunLib patches.
- The two backends are compared by shared observations, not by internal transaction mechanics.

## Architecture tests

Add tests to ensure:
- Abstractions does not reference Ledger or Accounts.
- Ledger adapter does not reference SimpleAccounts.
- SimpleAccounts adapter does not reference Ledger.
- Visualization references WebGlRunLib only through package/reference allowed by the solution.
