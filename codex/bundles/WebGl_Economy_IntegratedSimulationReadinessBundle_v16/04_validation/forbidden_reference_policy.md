# Forbidden reference policy

## Components

Forbidden:

- any reference to `CanDoItAll.Economy.*`
- example vocabulary in generic WebGL runtime

Allowed:

- generic WebGL runtime, run/playback contracts, tests and sandbox proofs.

## Economy

Forbidden outside `Simulation.WebGlBridge` and `SimulationSandbox`:

- `CanDoItAll.Components.*`
- `WebGl`, `WebGL`, `WebGlRunLib`, `WebGlLib`

Forbidden inside `Simulation.WebGlBridge`:

- `Simulation.SimpleAccounts`
- `Simulation.Ledger`
- `Economy.Ledger`
- backend-specific implementations

Forbidden in generic production code except fixtures/scenario factories/docs:

- water
- well
- farmer
- land
- parcel
- oligarchy
- near-household
- far-household
- shared-well
