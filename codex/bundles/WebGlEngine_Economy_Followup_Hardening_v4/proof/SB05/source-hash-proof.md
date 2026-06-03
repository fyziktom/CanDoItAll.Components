# SB05 source hash proof

Pathless source fixture:

| Source | SHA-256 |
| --- | --- |
| repo://tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/experiment.json | sha256:990264814d461b7f0f19f2a2e1a24b5119d6711adae6b2543dffcd9d7008d4a4 |

The focused unit test `SessionService_LoadsPathlessScenarioSourceWithoutExperimentJsonPath` constructs an in-memory `EconomySimulationScenarioSource` with an empty `ExperimentJsonPath`, sets `ContentHash` from `SimulationExperimentInputPackValidator.HashFile(experimentPath)`, loads through `IEconomySimulationSandboxSessionService.Load(EconomySimulationScenarioSource, ...)`, and asserts the loaded session keeps the pathless scenario id, empty experiment path, and generated WebGL run document.
