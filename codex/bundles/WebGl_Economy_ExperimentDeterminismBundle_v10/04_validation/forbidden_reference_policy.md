# Forbidden reference policy

## Components

Generic WebGL projects must not contain Economy domain terms in runtime/engine code:

- economy
- ledger
- account
- water
- well
- farmer
- land
- tax
- market
- citizen

Exceptions allowed only in:

- docs explicitly describing forbidden examples
- tests that verify domain-neutrality by negative scan
- bundle files under `codex/bundles`

## Economy

`Simulation.Abstractions`, `Simulation.SimpleAccounts`, and `Simulation.Visualization` must not reference:

- `CanDoItAll.Components.*`
- `WebGl*`
- JS runtime
- Blazor UI

`Simulation.Ledger` must not reference `Simulation.SimpleAccounts`.

## Cross repo

Do not implement the bridge yet. Document bridge contracts only.
