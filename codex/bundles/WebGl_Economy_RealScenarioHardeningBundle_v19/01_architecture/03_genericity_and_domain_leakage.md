# Genericity and domain leakage policy

Allowed in examples/tests/fixtures:

- shared-well
- water
- farmer
- land
- parcel
- entrepreneur
- village

Forbidden in generic projects unless test/example scoped:

- `CanDoItAll.Components.*`
  - any Economy/domain term
- `CanDoItAll.Economy.Simulation.Abstractions`
  - no WebGL or Components dependency
  - no hard-coded well/farmer/land semantics in generic models
- `CanDoItAll.Economy.Simulation.Visualization`
  - no WebGL asset/runtime dependency
- `CanDoItAll.Economy.Simulation.WebGlBridge`
  - may reference WebGlRunLib, but not SimpleAccounts/Ledger backends
- `CanDoItAll.Economy.SimulationSandbox`
  - may compose backends and bridge, but must do so through interfaces

Domain examples may remain in:

- tests
- fixtures
- demo scenario factories
- example apps
- documentation/probe files
