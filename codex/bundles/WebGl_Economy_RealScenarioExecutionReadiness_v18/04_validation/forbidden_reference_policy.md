# Forbidden Reference Policy

## Components repo

Forbidden in all Components source/projects:

```text
CanDoItAll.Economy
Economy
Ledger
SimpleAccounts
shared-well
farmer-land
water
well
farmer
land
```

Exception: human documentation that explicitly explains forbidden leakage, bundle files, or tests designed to assert forbidden leakage.

## Economy repo

Forbidden dependency directions:

```text
Simulation.Abstractions -> Components
Simulation.Abstractions -> WebGl
Simulation.Abstractions -> SimpleAccounts
Simulation.Abstractions -> Ledger
Simulation.Visualization -> Components
Simulation.Visualization -> WebGl
Simulation.WebGlBridge -> Simulation.SimpleAccounts
Simulation.WebGlBridge -> Simulation.Ledger
Simulation.WebGlBridge -> CanDoItAll.Economy.Ledger
```

Allowed:

```text
SimulationSandbox -> Simulation.SimpleAccounts
SimulationSandbox -> Simulation.Visualization
SimulationSandbox -> Simulation.WebGlBridge
Simulation.WebGlBridge -> Components.WebGlRunLib
```
