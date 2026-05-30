# Current repo review summary

## Components repo

Observed state:

- `CanDoItAll.Components.slnx` includes `CanDoItAll.Components.WebGlRunLib`, so the generic run layer exists beside `WebGlLib` and `WebGlSandbox`.
- `WebGlRunLib` contains run document/timeline/frame contracts, playback state/commands, frame source/store/applier interfaces, and frame-to-command-batch conversion.
- `WebGlRunLib` contains generic visual action contracts: `WebGlRunEvent`, `WebGlRunAction`, `WebGlRunActionTarget`, `WebGlRunActionPlan`, `WebGlVisualStateCatalog`, pose definitions, symbol definitions, action bindings, and generic action kinds.
- `WebGlLib` contains scene command batching in C# and JS.
- `WebGlLib` has a large-screen-only policy and scene runtime audit that checks unsafe JS patterns, import graph cycles, domain-neutrality, duplicate helper patterns, branch instructions, and small/medium/mobile optimization drift.

Concerns:

- Ordered action staging is still not clearly visible as a first-class contract in the fetched run action contracts. A shared-well sequence needs stage boundaries so batching cannot merge "go to well" and "return home" into one final motion.
- C# and JS command batch normalization exist separately. They must have parity tests or a single canonical semantics document; otherwise they may drift.
- `WebGlRunAction` contains aliases (`Kind`, `ActionKind`, `ObjectId`, `SubjectObjectId`, `TargetObjectId`, `Target.ObjectId`) which help compatibility but create ambiguity. A normalizer must produce one canonical form and tests must assert alias equivalence.
- The run playback controller has reusable logic, but scenario-specific demos must use it only as a consumer. Any orchestration still in sandbox pages should be moved into reusable services or kept explicitly demo-only.

## Economy repo

Observed state:

- `CanDoItAll.Economy.Simulation.Abstractions` now has scenario definitions, event stream, backend contracts, frame/delta contracts, deterministic hashing, validator/loader/store abstractions.
- `Simulation.SimpleAccounts` now has shared-well and entrepreneur scenario factories, definition materialization, delta building, consistency validation, and a backend.
- `Simulation.Ledger` has a ledger adapter with frame projection and delta diffing.
- `Simulation.Visualization` has economy visual frame/actions and action mapping policy.

Concerns:

- Shared-well is still partly handcrafted as materialized frames. This is useful for proof, but not enough for a generic simulation kernel.
- `SimulationScenarioDefinition` has parallel alias collections: `Entities`/`Actors`, `Places`/`Locations`, `Stores`/`InitialStores`. This can drift unless one canonical source is selected and aliases are generated, not independently persisted.
- `SimulationEventKind` contains both dotted canonical-looking names and older short names. This can drift unless there is a canonical taxonomy with an alias map.
- `SimulationScenarioEventCompiler` currently compiles scheduled events. It does not yet expand behaviors, rules, travel, trade, taxes, admin burden, or capacity constraints into derived events.
- Event sources/targets are string IDs without a strongly typed reference kind. This is flexible but ambiguous when the same ID could refer to actor, store, location, object, rule, or resource.
- Experiment reproducibility requires versioned JSON input packs, not hidden constants inside scenario factories.
