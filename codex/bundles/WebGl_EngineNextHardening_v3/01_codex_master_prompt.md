# Codex Master Prompt

You are working in `fyziktom/CanDoItAll.Components`.

## Mandatory branch rule

Before making any change, run:

```powershell
git status --short --branch
git branch --show-current
```

You must work in the currently checked-out branch. For this bundle the expected branch is `webgl-engine`.

Do **not** create a new branch.
Do **not** run `git switch -c`, `git checkout -b`, or `git branch <new-name>`.
Do **not** push to any new branch.
If the branch is not `webgl-engine`, stop and report the actual branch instead of switching branches.

## Goal

Harden the generic WebGL engine after the current `webgl-engine` implementation and prepare it for future generic run/playback use without adding economy-specific semantics.

The engine must remain domain-neutral. Do not add ledger, account, economy, market, well, community, entrepreneur, governance, process, or game-specific types into `CanDoItAll.Components.WebGlLib`.

## Current architecture assumptions

The current branch already has a generic WebGL scene runtime split into modules:

- `01-webgl-scene.js`: public façade
- `02-webgl-scene-core.js`: core helpers
- `03-webgl-scene-assets.js`: asset resolution
- `04-webgl-scene-symbols.js`: status symbols
- `05-webgl-scene-interaction.js`: selection/hover interaction
- `06-webgl-scene-camera.js`: camera
- `07-webgl-scene-overlays.js`: labels/overlays
- `08-webgl-scene-proof.js`: proof snapshot
- `09-webgl-scene-primitives.js`: primitive models
- `10-webgl-scene-lifecycle.js`: lifecycle/state/handlers
- `11-webgl-scene-graph.js`: runtime graph
- `12-webgl-scene-drag.js`: drag
- `13-webgl-scene-patching.js`: patch commands
- `14-webgl-scene-motion.js`: motion interpolation
- `15-webgl-scene-render-loop.js`: render scheduling
- `16-webgl-scene-models.js`: model loading
- `17-webgl-scene-resources.js`: resource ownership/disposal
- `18-webgl-scene-model-diagnostics.js`: model diagnostics
- `19-webgl-scene-shell.js`: DOM shell

Keep that modularity. Do not collapse modules.

## Execution strategy

Execute the subbundles in order. After each group of 3 subbundles, stop for a refactoring gate:
- Run the audit script.
- Check JS runtime file sizes.
- Review dependency direction.
- Record evidence.
- Refactor before adding new features if any file is drifting into a monolith.

## Hard boundaries

### WebGlLib may contain

- Generic scene DTOs
- Generic object/link/camera/environment state
- Asset catalogs, model variants, import diagnostics
- Primitive fallback rendering
- Generic status symbols
- Selection, hover, drag, camera commands
- Generic patch/import/export commands
- Basic transform motion interpolation
- Generic runtime diagnostics
- Generic scene document serialization

### WebGlLib must not contain

- Economy simulation rules
- Ledger transaction logic
- Simple account logic
- Market rules
- Well/community/entrepreneur scenario semantics
- Governance semantics
- Run lifecycle semantics
- Save-slot persistence provider semantics
- Pathfinding/physics as simulation behavior

### Future WebGlRunLib may contain

A future `CanDoItAll.Components.WebGlRunLib` may contain generic playback/run contracts over WebGlLib:
- run timeline
- frame cursor
- scene patch stream
- playback controls
- deterministic replay metadata
- in-memory run document model
- adapter interfaces

But it must also remain domain-neutral.

### Future Economy repo may contain

A future `CanDoItAll.Economy.Simulation.Abstractions` project may contain shared economy simulation primitives used by both ledger-backed and simple-account simulations. It must not reference Ledger or Accounts directly.

## Required validation

At minimum:

```powershell
npm run webgllib:audit-scene-runtime
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx
```

Also run browser proof for:
- `/tycoon-village`
- `/model-lab`

Record screenshots, console logs, proof snapshots, and diagnostics JSON under a new evidence folder.
