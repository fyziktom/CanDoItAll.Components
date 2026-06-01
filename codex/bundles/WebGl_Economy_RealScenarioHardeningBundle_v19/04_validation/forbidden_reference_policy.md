# Forbidden reference policy

## Components

Forbidden references:
- `CanDoItAll.Economy`
- `CanDoItAll.Economy.*`

Forbidden terms in generic runtime files:
- economy
- ledger
- account
- water
- well
- farmer
- land
- parcel
- entrepreneur

## Economy low-level projects

`Simulation.Abstractions` must not reference:
- Components
- WebGL runtime
- SimpleAccounts
- Ledger backend
- SDK/BusinessObjects

`Simulation.Visualization` must not reference:
- Components
- WebGL runtime
- SimpleAccounts
- Ledger backend

`Simulation.WebGlBridge` may reference:
- Simulation.Abstractions
- Simulation.Visualization
- Components.WebGlRunLib

It must not reference:
- Simulation.SimpleAccounts
- Simulation.Ledger

`SimulationSandbox` may compose backends and bridge through interfaces.
