# Forbidden reference policy

## Components

Forbidden references:

- `CanDoItAll.Economy.*`
- `CanDoItAll.Economy.Simulation.*`
- any domain-specific simulation package

## Economy lower simulation layers

Forbidden references from these projects:

- `Simulation.Abstractions`
- `Simulation.SimpleAccounts`
- `Simulation.Visualization`

to:

- `CanDoItAll.Components.*`
- `WebGl`
- renderer-specific APIs
- ledger/backend projects, except where explicitly intended

## Economy bridge

Allowed:

- `Simulation.Abstractions`
- `Simulation.Visualization`
- `CanDoItAll.Components.WebGlRunLib`

Forbidden:

- `Simulation.SimpleAccounts`
- `Simulation.Ledger`
- `CanDoItAll.Economy.Ledger`
- direct Components.WebGlLib unless explicitly justified
