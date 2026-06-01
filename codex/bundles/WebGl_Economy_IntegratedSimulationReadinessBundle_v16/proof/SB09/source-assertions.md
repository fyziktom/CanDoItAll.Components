# SB09 Source Assertions

- SimulationSandbox now selects backends through a registry and honors the run-plan backend id unless an explicit injected backend is supplied.
- Public sandbox projection no longer exposes `SimpleSimulationScenario` as the joined workflow contract.
- Snapshot pipeline receives the projected run document and attaches visual state in the headless sandbox workflow.

