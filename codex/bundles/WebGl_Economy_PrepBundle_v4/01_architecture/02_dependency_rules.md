# 01. Dependency boundary rules

## Components repo

```text
WebGlRunLib -> WebGlLib
WebGlSandbox -> WebGlRunLib, WebGlLib, BaseLib/OverlayLib as needed
WebGlLib -> OverlayLib only if already required by assets/components
```

Forbidden:
- `Components.WebGlLib -> Economy.*`
- `Components.WebGlRunLib -> Economy.*`
- domain words in generic contracts: ledger, account, tax, business unit, well, entrepreneur, community, process run, artifact.

## Economy repo

```text
Simulation.Abstractions -> Core only
Simulation.SimpleAccounts -> Simulation.Abstractions
Simulation.Ledger -> Simulation.Abstractions, Ledger, BusinessObjects, Sdk
Simulation.Visualization -> Simulation.Abstractions
Simulator -> existing dependencies unchanged unless explicitly adapting later
Simulator.Components -> existing dependencies unchanged in this wave
```

Forbidden:
- `Simulation.Abstractions -> Ledger`
- `Simulation.Abstractions -> BusinessObjects`
- `Simulation.Abstractions -> Sdk`
- `Simulation.SimpleAccounts -> Ledger`
- `Simulation.SimpleAccounts -> BusinessObjects`
- `Simulation.SimpleAccounts -> Sdk`
- `Simulation.Visualization -> Components.WebGlLib`
- any Components package reference in Economy for this wave.

## Validation command requirements

Codex must add or update dependency scan scripts so violations fail CI/manual validation.
