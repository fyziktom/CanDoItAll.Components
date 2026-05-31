# Forbidden reference policy

## Components

Forbidden domain terms in generic runtime and WebGlRunLib production code:

- economy
- ledger
- account
- water
- well
- farmer
- land
- tax
- citizen
- entrepreneur

Allowed only in test fixture names or comments explaining forbidden examples.

## Economy

Forbidden references from these projects:

- `CanDoItAll.Economy.Simulation.Abstractions` must not reference Ledger, BusinessObjects, SDK, Components, WebGL, UI, EFCore.
- `CanDoItAll.Economy.Simulation.SimpleAccounts` must reference only Simulation.Abstractions.
- `CanDoItAll.Economy.Simulation.Visualization` must reference only Simulation.Abstractions.
- `CanDoItAll.Economy.Simulation.Ledger` must not reference SimpleAccounts or Visualization.

## Experiment input rule

The simulation runtime must not generate hidden random inputs. Random generation may only create versioned JSON input files before a run.
