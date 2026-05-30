# Layer boundary

## Components repository

`CanDoItAll.Components.WebGlLib`
: Generic renderer, scene DTOs, asset catalog, scene document, commands, patches, motion primitives, diagnostics, JS runtime.

`CanDoItAll.Components.WebGlRunLib`
: Generic run/playback/action layer above WebGlLib. It may reference WebGlLib. It must not contain economy, ledger, market, water, well, citizen, or business domain concepts.

`CanDoItAll.Components.WebGlSandbox`
: Generic proof host. It can demonstrate a runner moving to a target and changing pose, but it must not depend on Economy.

## Economy repository

`CanDoItAll.Economy.Simulation.Abstractions`
: Backend-neutral scenario, event, frame, delta, run, and capability contracts. No dependencies.

`CanDoItAll.Economy.Simulation.SimpleAccounts`
: Lightweight backend that consumes scenario definitions and emits backend-neutral frames/events. References only Abstractions.

`CanDoItAll.Economy.Simulation.Ledger`
: Ledger-backed adapter. References Abstractions plus ledger-specific packages. Must not reference SimpleAccounts.

`CanDoItAll.Economy.Simulation.Visualization`
: Maps SimulationFrame/SimulationEvent to EconomyVisualFrame/EconomyVisualAction DTOs. Must not reference WebGL or Components.

## Future bridge, not now

A future integration package may map `EconomyVisualAction` to `WebGlRunAction`. Do not implement that coupling in this phase.
