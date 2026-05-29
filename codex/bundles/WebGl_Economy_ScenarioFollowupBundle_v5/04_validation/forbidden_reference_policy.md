# Forbidden reference policy

## Components repo

Forbidden in `CanDoItAll.Components.WebGlLib` and `CanDoItAll.Components.WebGlRunLib`:

```text
CanDoItAll.Economy
Ledger
SimpleAccounts
BusinessObjects
Simulation.Abstractions
water
well
citizen
market
entrepreneur
```

Exceptions:
- test/sample content may use generic demo objects only;
- docs may mention future integration, but not reusable code.

## Economy repo

Forbidden in `src/CanDoItAll.Economy.Simulation.*` except existing UI projects outside this namespace:

```text
CanDoItAll.Components
WebGl
WebGL
Three
Babylon
Blazor component references
```

Allowed:
- `Simulation.Visualization` may define visual DTOs, action kinds, layout hints, symbols, and nodes.
- It must not import renderer DTOs.
