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

## Status
- Completed.

## Prerequisites
- SB06 strict bridge mapping proof is complete.
- SB08 snapshot builder proof is complete.
- SB11 mapping contract boundary decision is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.SimulationSandbox\EconomySimulationSandboxWorkflow.cs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\SimulationSandboxWorkflowTests.cs`
- `C:\repositories\CanDoItAll.Economy\scripts\audit-simulation-boundaries.ps1`

## Dependency Impact
- Critical orchestration foundation for a later connected sandbox UI.

## Validation Depth
- Requires SimpleAccounts positive path, fake-backend positive path, boundary negative proof, and anti-stub audit.

## Acceptance Checklist
- Workflow depends on backend, visualization, WebGL projection, and snapshot pipeline abstractions.
- SimpleAccounts remains wired through an adapter/selector.
- Fake backend can run without bridge depending on SimpleAccounts or Ledger.

## Proof Required
- `bundle://proof/SB12/manifest.md`
- `bundle://proof/SB12/semantic-invariants.md`
- Economy test transcript and boundary audit transcript.

## Browser Validation Logging
- Browser validation is not required unless a sandbox route is added or changed.

## Progression Gate
- SB15/SB16 may cite sandbox readiness only after backend-neutral workflow proof is recorded.

## Suggested Agent Prompt
- Refactor SimulationSandbox into backend-neutral orchestration seams while proving SimpleAccounts and fake backend paths.
