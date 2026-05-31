# Forbidden reference policy

## Components

Forbidden everywhere:

- `CanDoItAll.Economy`
- `CanDoItAll.Economy.*`
- economy-specific event names
- scenario-specific words in generic runtime code

## Economy lower layers

Forbidden in:

- `Simulation.Abstractions`
- `Simulation.SimpleAccounts`
- `Simulation.Visualization`

References to:

- `CanDoItAll.Components`
- `WebGl`
- `WebGL`

## Economy bridge

Allowed:

- `CanDoItAll.Economy.Simulation.Abstractions`
- `CanDoItAll.Economy.Simulation.Visualization`
- `CanDoItAll.Components.WebGlRunLib`
- `CanDoItAll.Components.WebGlLib` if explicitly required for scene document types

Forbidden:

- `Simulation.SimpleAccounts`
- `Simulation.Ledger`
- low-level `Economy.Ledger`
- persistence projects

## Economy SimulationSandbox

May reference:

- Simulation backends
- Visualization
- WebGlBridge
- snapshot services

It should use backend interfaces where possible.
