# Architecture Direction

## Layering target

```text
CanDoItAll.Components.WebGlLib
  Generic WebGL renderer, scene model, assets, interactions, commands, diagnostics.

CanDoItAll.Components.WebGlRunLib    [future or optional in later subbundle]
  Generic run/playback layer over WebGlLib.
  No economy, ledger, simple-account, process, or game-specific semantics.

CanDoItAll.Economy.Simulation.Abstractions    [future Economy repo]
  Shared economy simulation abstractions.
  No dependency on Ledger, Accounts, SDK, UI, persistence, or WebGL.

CanDoItAll.Economy.Simulation.SimpleAccounts  [future Economy repo]
  Simple account simulation adapter.

CanDoItAll.Economy.Simulation.Ledger          [future Economy repo]
  Ledger-backed simulation adapter.

CanDoItAll.Economy.Simulation.Visualization   [future Economy repo]
  Maps generic economy run frames to WebGlRunLib/WebGlLib scenes.
```

## Why this matters

The shared-well scenario, small community scenario, ledger simulator, and simple-account simulator will share concepts such as actors, resources, flows, observations, timeline steps, and scenario metadata. They should not share implementation details such as ledger UTXO transactions or simple account balance update rules.

## Dependency rules

### Components repo

```text
WebGlRunLib -> WebGlLib
WebGlLib -> no WebGlRunLib
WebGlLib -> no Economy
WebGlSandbox -> WebGlLib and optionally WebGlRunLib
```

### Economy repo

```text
Simulation.Abstractions -> Core only, or no project references if possible
Simulation.SimpleAccounts -> Simulation.Abstractions + Economy.Accounts
Simulation.Ledger -> Simulation.Abstractions + Economy.Ledger
Simulation.Visualization -> Simulation.Abstractions + WebGlRunLib/WebGlLib package
Economy.Simulator -> may consume adapters, but must not force one backend
```

## Generic scene vs generic run vs economic simulation

### Scene
A scene is a snapshot of visual state.

Examples:
- object position
- object asset
- symbol above object
- camera
- labels
- current selection

### Run
A run is generic time-indexed playback over a scene.

Examples:
- frame 10 applies patch A
- frame 11 enqueues motion B
- pause/resume
- export replay
- jump to step

### Economic simulation
An economic simulation decides what the world means and how it evolves.

Examples:
- community member takes water
- well resource decreases
- entrepreneur sells goods
- ledger transaction commits
- simple account balance changes
- trust/conflict status changes
