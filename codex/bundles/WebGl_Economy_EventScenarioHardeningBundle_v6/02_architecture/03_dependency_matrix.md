# Dependency rules

## Components repo

Allowed:

- `WebGlRunLib -> WebGlLib`
- `WebGlSandbox -> WebGlRunLib + WebGlLib + BaseLib`

Forbidden:

- any `CanDoItAll.Economy.*` reference in Components;
- scenario/economy words in `WebGlLib` or `WebGlRunLib` public contracts, except docs/examples explaining forbidden boundaries;
- ledger/simple-account semantics in Components.

## Economy repo

Allowed:

- `Simulation.SimpleAccounts -> Simulation.Abstractions`
- `Simulation.Visualization -> Simulation.Abstractions`
- `Simulation.Ledger -> Simulation.Abstractions + Economy.Ledger + BusinessObjects + Sdk`

Forbidden:

- `Simulation.Abstractions -> anything`;
- `Simulation.SimpleAccounts -> Ledger/BusinessObjects/Sdk/Components/WebGL`;
- `Simulation.Visualization -> Components/WebGL`;
- `Simulation.Ledger -> Simulation.SimpleAccounts`;
- existing `Simulator` should not be retrofitted to depend on the new simple/ledger prep until a later explicit integration phase.
