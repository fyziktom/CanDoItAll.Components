# SB10 - Economy: generic scenario definition and loader

Hardcoded frames are not enough. Add a scenario definition layer.

Add to `Simulation.Abstractions`:

- `SimulationScenarioDefinition`
- `SimulationScenarioEntityDefinition`
- `SimulationScenarioPlaceDefinition`
- `SimulationScenarioResourceDefinition`
- `SimulationScenarioStoreDefinition`
- `SimulationScenarioBehaviorDefinition`
- `SimulationScenarioEventTemplate`
- `SimulationScenarioDefinitionSerializer`
- `SimulationScenarioDefinitionValidator`

Requirements:

- load/save JSON;
- deterministic hash excluding UI/playback fields;
- validate duplicate ids, dangling references, invalid quantities, invalid time steps;
- do not reference WebGL or Components;
- no backend-specific ledger/simple-account fields except metadata with `backend.*` prefix.

Proof:

- convert shared-well seed to a definition;
- materialize it to frames through SimpleAccounts;
- hash is stable across property ordering.
