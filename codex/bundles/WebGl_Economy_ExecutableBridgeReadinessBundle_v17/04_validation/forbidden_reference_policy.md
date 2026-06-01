# Forbidden reference policy

## Components repo

Forbidden:

- any reference to `CanDoItAll.Economy`
- domain words in generic WebGL runtime: economy, ledger, account, water, well, farmer, land, parcel, citizen, entrepreneur
- small/medium/mobile/tablet WebGL optimization tasks

Allowed:

- generic WebGL/scene/run contracts
- generic examples in WebGlSandbox if not Economy-specific

## Economy repo

Forbidden in `Simulation.Abstractions`, `Simulation.Visualization`, `Simulation.SimpleAccounts`:

- `CanDoItAll.Components`
- `WebGl`, `WebGL`, renderer-specific classes
- direct `Ledger` / `BusinessObjects` / `Sdk` references outside `Simulation.Ledger`

Allowed:

- `Simulation.WebGlBridge` may reference `CanDoItAll.Components.WebGlRunLib`
- `SimulationSandbox` may reference bridge and backends via registry/orchestration boundary
- scenario fixtures may contain example words such as water/well/farmer/land
