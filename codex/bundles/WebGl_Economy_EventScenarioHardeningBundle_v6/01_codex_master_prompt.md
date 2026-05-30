# Codex master prompt: WebGL + Economy scenario/action hardening

You are working in two repositories that are already cloned locally:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

## Branch rule

Before making changes in each repository, run an inventory command equivalent to:

```bash
git status --short
git branch --show-current
git log -1 --oneline
```

Do not create a new branch. Do not run `git checkout -b`, `git switch -c`, or any command that creates a branch. Continue in the currently checked-out branch.

## Cross-repo rule

Do not connect the repositories yet.

Allowed:

- Prepare generic `WebGlRunLib` contracts and services in `CanDoItAll.Components`.
- Prepare backend-neutral simulation/scenario/event contracts in `CanDoItAll.Economy`.
- Add tests and documentation in both repos.

Forbidden:

- `CanDoItAll.Components.*` must not reference `CanDoItAll.Economy.*`.
- `CanDoItAll.Economy.Simulation.*` must not reference `CanDoItAll.Components.*`, WebGL, Three.js, or UI packages.
- `Simulation.SimpleAccounts` must not reference Ledger, BusinessObjects, SDK, Components, or WebGL.
- `Simulation.Ledger` must not reference `Simulation.SimpleAccounts`.
- Domain examples like shared-well must not become engine-specific code.

## High-level goal

Harden the current implementation and prepare the next phase where scenarios can be defined generically, loaded into a simulator, executed through different backends, projected to visual intentions, and eventually played by WebGL through a generic action mapping layer.

The immediate implementation should focus on preparation and hardening, not final integration.

## Required implementation tracks

### Components repo

1. Move generic playback orchestration out of `WebGlSandbox/RunPlayback.razor.cs` into `CanDoItAll.Components.WebGlRunLib`.
2. Add a generic `WebGlRunAction` model supporting sequence/parallel actions, object movement, target-object movement, return-to-anchor, pose/asset variant changes, symbols, patch application, and waits.
3. Add target resolution contracts that support object ids, anchors, offsets, and current scene positions without requiring the simulator to know WebGL coordinates.
4. Add an action-to-command planner that converts `WebGlRunAction` to `WebGlScenePatch` and `WebGlObjectMotionCommand`.
5. Add batching helpers so one frame can submit patches/motions as a single application step instead of many JS interop calls.
6. Harden asset cache disposal and diagnostics.
7. Continue JS runtime refactoring gates and line-count/audit validation.

### Economy repo

1. Split `SimulationContracts.cs` into smaller files.
2. Split `SimpleSimulation.cs` into scenario definitions, frame materializers, delta builders, and scenario catalog.
3. Add backend-neutral `SimulationScenarioDefinition` models that can describe actors, resources, places, objects, placement, behaviors, and event templates.
4. Add backend-neutral `SimulationEvent` models: resource use, movement intention, transfer, work, administration, relationship update, issue raised/resolved, store changed.
5. Extend `Simulation.Visualization` with `EconomyVisualAction` / `EconomyVisualIntent` DTOs. These must not reference WebGL types.
6. Add shared-well scenario definition proof and small entrepreneur scenario definition proof using the new models.
7. Keep SimpleAccounts and Ledger backends isolated behind common abstractions.
8. Add reference-boundary audits and tests.

## Performance and bottleneck focus

Pay special attention to:

- full scene rebuilds versus patch application;
- repeated JS interop calls per frame;
- rebuilding visual nodes/links for every step;
- deterministic hashing on large frames;
- model asset cache lifecycle;
- unbounded command history or failed command detail lists;
- hardcoded scenario frames that cannot scale;
- high-frequency `StateHasChanged` during playback.

## Evidence required

Produce evidence files in `artifacts/` or `codex/bundles/.../proof/`:

- branch inventory;
- dependency/reference scans;
- line-count/runtime audit;
- dotnet build/test transcripts;
- at least one deterministic scenario definition test;
- at least one generic visual-action mapping test;
- no cross-repo coupling proof.
