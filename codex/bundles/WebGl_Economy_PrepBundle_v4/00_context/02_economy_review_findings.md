# 00. Economy repo review findings

## Current shape

The Economy repo already has many layered projects. The most relevant for this wave are:

- `CanDoItAll.Economy.Core`
- `CanDoItAll.Economy.Accounts`
- `CanDoItAll.Economy.Ledger`
- `CanDoItAll.Economy.BusinessObjects`
- `CanDoItAll.Economy.Sdk`
- `CanDoItAll.Economy.Simulator`
- `CanDoItAll.Economy.Simulator.Components`

The existing simulator is intentionally an SDK-level layer. It references `BusinessObjects`, `Core`, and `Sdk`, and should not push simulator concepts downward into Core, Ledger, BusinessObjects, SDK, Node, or CLI.

## Existing design signals

The existing simulator documentation says:
- simulator concepts must stay above the SDK;
- project/runs are managed through `ISimulationRunLifecycleService`;
- runs have automatic/manual modes, selected steps, materialized snapshots, lifecycle states, interventions, output hashes, and evidence tabs;
- UI playback speed is intentionally excluded from deterministic hashes.

The scenario documentation says:
- ledger what-if simulation should not mutate the real ledger;
- scenario identity should use `scenario_id`;
- deterministic outputs are required;
- time control should flow through `ITimeProvider`.

## Main architectural issue for next phase

We need to support at least two future economic simulation backends:

1. Simple-account simulation backend
   - light accounts, resources, flows, obligations, rules;
   - fast toy scenarios such as shared well, village cooperation, entrepreneurs;
   - no ledger/UTXO dependency.

2. Ledger-backed simulation backend
   - scenario fork from ledger snapshot;
   - UTXO/projection based;
   - uses existing ledger/business object/SDK mechanisms.

The shared parts must not reference ledger or simple-account implementation details.

## Recommended new Economy projects

Create these as preparation only. Do not connect them to Components/WebGL yet.

### `CanDoItAll.Economy.Simulation.Abstractions`

References:
- `CanDoItAll.Economy.Core` only, if required.

Contains:
- run identifiers;
- scenario manifest;
- simulation clock abstractions;
- frame/snapshot contracts;
- actor/resource/flow/relationship contracts;
- generic result and issue models;
- backend interface contracts;
- deterministic hash helpers.

Must not reference:
- `Ledger`
- `BusinessObjects`
- `Sdk`
- `Simulator.Components`
- `Components.WebGlLib`

### `CanDoItAll.Economy.Simulation.SimpleAccounts`

References:
- `Simulation.Abstractions`
- optionally `Core`

Contains:
- simple balance stores;
- resources;
- rule execution that is not account-rule BO execution;
- simple transaction/event model;
- shared-well and entrepreneur scenarios.

Must not reference:
- `Ledger`
- `BusinessObjects`
- `Sdk`

### `CanDoItAll.Economy.Simulation.Ledger`

References:
- `Simulation.Abstractions`
- `Ledger`
- `BusinessObjects`
- `Sdk`

Contains:
- ledger snapshot adapter;
- scenario fork adapter;
- ledger projection adapter;
- mapping from ledger/account-rule evidence to generic simulation frames.

Must not reference:
- `SimpleAccounts`
- `Simulator.Components`
- `Components.WebGlLib`

### `CanDoItAll.Economy.Simulation.Visualization`

References:
- `Simulation.Abstractions`

Contains:
- generic economy visual frame contracts:
  - visual nodes;
  - visual links;
  - symbols;
  - frame diffs;
  - layout hints;
  - semantic categories.
- No WebGL references yet.

Later, another adapter can map `EconomyVisualFrame` to `WebGlSceneModel` or `WebGlRunDocument`.

## Existing simulator compatibility

Do not break `CanDoItAll.Economy.Simulator`. It can later either:
- keep existing project/run management and adapt to `Simulation.Abstractions`; or
- provide an adapter that emits generic `SimulationRunFrame` from current run observation data.

For this wave, preparation is enough.
