# SB12 - Economy SimpleAccounts refactor and validation

## Problem

`SimpleSimulation.cs` currently mixes data models, backend, scenario factories, frame/delta materialization, helper methods, and sample content.

## Required split

```text
Model/
  SimpleAccount.cs
  SimpleAccountBalance.cs
  SimpleResource.cs
  SimpleFlow.cs
  SimpleObligation.cs
  SimpleRule.cs

Backend/
  SimpleSimulationBackend.cs

Scenarios/
  SimpleSimulationScenario.cs
  SimpleSimulationScenarioFactory.cs
  SharedWellScenarioSeed.cs
  EntrepreneurScenarioSeed.cs

Materialization/
  SimpleSimulationFrameMaterializer.cs
  SimpleSimulationDeltaBuilder.cs

Validation/
  SimpleSimulationConsistencyValidator.cs
```

## Required validations

- Flow timestamps match frame clock.
- Flow store ids exist or are explicitly empty when actor-level flow is intended.
- Store owner actor ids exist.
- Flow actor ids exist.
- Resource ids exist in a resource definition catalog.
- Deltas are minimal and deterministic.
