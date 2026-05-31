# Target architecture

```text
CanDoItAll.Components
  WebGlLib
    - scene rendering
    - scene patches
    - motion primitives
    - command batches
    - diagnostics
    - large-screen WebGL runtime only

  WebGlRunLib
    - run document contracts
    - ordered stages
    - action normalization
    - per-object action/motion scheduling
    - playback controller
    - source/provenance refs
    - no economy vocabulary

CanDoItAll.Economy
  Simulation.Abstractions
    - experiment input packs
    - scenario definitions
    - placement and parameters
    - event taxonomy
    - invariant definitions
    - deterministic hashing

  Simulation.SimpleAccounts
    - simple deterministic transition engine
    - resource/actor/store state
    - event handlers registered by event kind
    - no WebGL references

  Simulation.Ledger
    - ledger-backed projection adapter
    - no SimpleAccounts reference

  Simulation.Visualization
    - economy visual actions/intents
    - no WebGL references
    - bridge remains design-only for now
```

## Data flow target

```text
Experiment input pack JSONs
  -> validate hashes/schemas/references
  -> load scenario + placement + parameters + rules + run plan + invariants
  -> compile deterministic event stream
  -> apply generic transition engine
  -> produce frames/deltas + metrics + invariant results
  -> map to economy visual actions
  -> later bridge maps visual actions to WebGlRun actions
```

No simulation should derive random positions, random actor traits, random buyer demand, or random rule settings at runtime. Random generation is permitted only as a pre-run tool that writes explicit JSON files and provenance.
