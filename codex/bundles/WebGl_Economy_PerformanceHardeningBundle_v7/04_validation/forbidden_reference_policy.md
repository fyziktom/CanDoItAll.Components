# Forbidden reference policy

## Components

Forbidden in `CanDoItAll.Components.WebGlLib` and `CanDoItAll.Components.WebGlRunLib`:

- Economy
- Ledger
- Account
- Water
- Well
- Entrepreneur
- Citizen
- BusinessObject
- SDK-specific domain semantics

These words may appear only in docs explaining that they are forbidden or future examples outside source runtime files.

## Economy

Forbidden in `CanDoItAll.Economy.Simulation.Abstractions`, `.SimpleAccounts`, `.Visualization`:

- CanDoItAll.Components
- WebGlLib
- WebGlRunLib
- Three.js
- GLB
- Renderer

`Simulation.Ledger` may reference Ledger/BO/SDK but must not reference SimpleAccounts or Components.
