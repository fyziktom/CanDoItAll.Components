# Forbidden reference policy

## Components

Forbidden:
- Any reference to `CanDoItAll.Economy`.
- Any Economy terms in generic WebGL JS runtime.
- Any mobile/small/medium screen optimization task.

Allowed:
- Generic WebGL scene/run/playback contracts.
- Generic browser runtime adapter.

## Economy

Allowed:
- `Simulation.WebGlBridge` may reference `Components.WebGlRunLib`.
- `SimulationSandbox` may reference `Simulation.WebGlBridge`.
- Lower-level simulation abstractions must not reference Components/WebGL.

Forbidden:
- Components referencing Economy.
- SimpleAccounts referencing Ledger.
- Ledger referencing SimpleAccounts.
- Abstractions referencing Components/WebGL.
