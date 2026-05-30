# Forbidden reference policy

## Components

`WebGlLib` and `WebGlRunLib` must not reference:
- Economy
- Ledger
- Accounts
- BusinessObjects
- water/well demo code
- simulator-specific domain rules

## Economy

`Simulation.Abstractions`, `Simulation.SimpleAccounts`, `Simulation.Ledger`, and `Simulation.Visualization` must not reference:
- CanDoItAll.Components.*
- WebGlLib
- WebGlRunLib
- Razor/UI packages

`Simulation.Visualization` may define visual DTOs and visual actions, but no renderer-specific types.
