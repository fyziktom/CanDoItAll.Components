# SB12 — Economy SimulationSandbox orchestrator

## Goal
Turn sandbox from simple workflow into backend-neutral orchestration.

## Required
- `IEconomySimulationSandboxWorkflow`
- `IEconomySimulationBackendSelector`
- `IEconomyVisualizationPipeline`
- `IEconomyWebGlProjectionPipeline`
- `IEconomySnapshotPipeline`

## Rule
Sandbox may wire SimpleAccounts for now, but the workflow must be able to accept a backend abstraction.

## Validation
- SimpleAccounts path works.
- Fake backend path works.
- No bridge dependency on SimpleAccounts/Ledger.
