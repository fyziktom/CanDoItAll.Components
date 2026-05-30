# Generic simulation kernel boundaries

## Economy.Simulation.Abstractions

Allowed:

- experiment input contracts
- scenario definition contracts
- canonical reference model
- event stream contracts
- deterministic hash utilities
- validation result contracts
- backend capability contracts

Forbidden:

- ledger implementation details
- WebGL/Components types
- UI state
- hardcoded shared-well or farmer-land scenario logic

## Economy.Simulation.SimpleAccounts

Allowed:

- generic simple state transition engine
- simple account/store/resource state
- behavior/rule expansion for simple scenarios
- deterministic scenario materializer
- sample definitions for shared-well and farmer-land only as examples

Forbidden:

- ledger dependencies
- WebGL dependencies
- scenario-id switch as the long-term materialization strategy

## Economy.Simulation.Ledger

Allowed:

- ledger snapshot/fork adapters
- ledger event projection to neutral `SimulationEvent`
- ledger frame projection to neutral `SimulationFrame`
- diffing optimized for ledger snapshots

Forbidden:

- simple-account engine dependencies
- WebGL dependencies
- hardcoded shared-well logic

## Economy.Simulation.Visualization

Allowed:

- `SimulationFrame/Event` -> `EconomyVisualFrame/Action` mapping
- domain-neutral visual intentions within Economy vocabulary
- no Components/WebGL reference

Forbidden:

- direct `WebGlRunAction` references
- direct GLB/asset IDs from Components
- JS/WebGL runtime concerns

## Components.WebGlRunLib

Allowed:

- generic run documents
- generic action stages
- generic target/anchor resolution
- generic action planning to `WebGlSceneCommandBatch`
- playback state and frame source abstractions

Forbidden:

- Economy-specific concepts: water, well, farmer, tax, market, ledger, account
- any mobile/small-screen optimization; do not add it
