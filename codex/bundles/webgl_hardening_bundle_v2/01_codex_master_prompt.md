# Codex Master Prompt

You are working in the `CanDoItAll.Components` repository.

## Non-negotiable branch rule

Do not create a new branch.

At the beginning run:

```powershell
git branch --show-current
git status --short
```

Work only in the currently checked-out branch. Do not run `git checkout -b`, `git switch -c`, create a new branch, create a remote branch, or move the work to another branch. If the branch/status is unexpected, stop and report instead of creating a branch.

## Task

Perform a second hardening pass on the generic WebGL scene runtime and standalone WebGL sandbox.

The goal is to prepare `CanDoItAll.Components.WebGlLib` for future consumers such as generic runs, simulations, and game-like visualizations while keeping it domain-neutral. Do not add economy, process, agent, or game-specific semantics.

## Current architectural boundary

Keep in `WebGlLib`:

- declarative scene DTOs;
- asset catalogs, asset variants, quality profiles, fallback logic;
- generic GLB loading and primitive fallback;
- generic model import diagnostics;
- generic camera/selection/hover/drag;
- export/import of a scene DTO/document;
- patching and basic object transform motion;
- diagnostics and proof snapshots;
- render loop policy.

Keep out of `WebGlLib` and reserve for future run/game/simulation layers:

- simulation clock semantics;
- run lifecycle;
- scenario persistence providers;
- replay/event logs;
- domain event orchestration;
- pathfinding;
- physics/collision;
- game rules;
- economy/process-specific symbol policies.

## Required implementation areas

### A. JS module maintainability

Review all files under:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene
```

Refactor where needed so that each module has a clear single purpose. Avoid long catch-all modules.

Required targets:

- keep `01-webgl-scene.js` as a thin public API facade;
- keep lifecycle orchestration in `10-webgl-scene-lifecycle.js`, but extract shell/state/notifications/disposal helpers if this file remains too broad;
- keep asset resolution separate from model loading and primitive rendering;
- keep drag separate from selection/hover;
- keep patching separate from motion;
- keep render scheduling separate from rendering policy.

Do not convert the runtime to TypeScript.

### B. Safe JavaScript implementation

Add a JS audit tool under `tools/webgllib/` that checks:

- runtime scene JS file line counts;
- no accidental large monolithic file growth;
- no unsafe `innerHTML` usage in runtime modules except explicitly documented whitelisted static cases;
- no forbidden branch creation commands in committed workflow instructions;
- syntax validation of scene runtime modules.

### C. Model import diagnostics and invisible model hardening

Add model diagnostics that make bad GLB conversions diagnosable:

- empty scene detection;
- zero/near-zero bounds detection;
- NaN/Infinity transform detection;
- all meshes invisible detection;
- all materials transparent detection;
- extreme bounds/unit-scale warning;
- optional debug bounding box rendering;
- optional material normalization/double-sided mode;
- optional import offsets/scale/rotation per asset/variant.

This must not force high-detail models by default. Primitive profile remains the safe default.

### D. Resource ownership and disposal

Review GLB template cloning and disposal. Fix ownership issues:

- shared template geometry is not disposed per instance;
- cloned materials are disposed;
- primitive fallback geometries/materials are disposed;
- symbols and links are disposed;
- decorations/ground/grid are disposed;
- repeated create/dispose cycles do not leak canvases, event handlers, or owned resources.

### E. Asset catalog and sandbox split

Refactor the sandbox asset catalog so it does not become a giant file. Keep it readable and maintainable.

Suggested split:

```text
WebGlSandboxAssetCatalogFactory.cs
WebGlSandboxCoreAssets.cs
WebGlSandboxBuildingAssets.cs
WebGlSandboxPersonAssets.cs
WebGlSandboxPropAssets.cs
WebGlSandboxExternalModels.cs or generated manifest
```

Add an asset/model lab page if useful:

```text
/model-lab
```

It should let the developer inspect one asset at a time, switch quality profile, show diagnostics, and confirm fallback/model rendering.

### F. Command result and patch/motion hardening

Do not replace existing public APIs unless necessary. Additive APIs are preferred.

Add or refine generic result objects:

- `WebGlSceneCommandResult`;
- `WebGlScenePatchResult` parity between C# and JS;
- motion accepted/completed/failed diagnostics;
- active motion count in diagnostics/proof snapshots;
- failed command details.

Do not implement a full run engine.

### G. Scene document serialization

Add a generic scene document contract suitable for saving/loading a scene layout without run semantics:

```text
WebGlSceneDocument
  SchemaVersion
  Scene
  RuntimeOptions
  SavedAtUtc
  Source
  Checksum/ContentHash optional
  Metadata
```

This belongs in `WebGlLib` because it is a generic scene layout document. Storage providers, save slots, run persistence, and replay logs belong to a future run layer.

### H. Validation

At minimum run:

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
```

Also perform a browser proof on a large desktop viewport. Validate that:

- `window.CanDoItAll.webglScene` exists;
- `window.CanDoItAll.webglWorkbench` still exists;
- `/tycoon-village` still loads;
- model profile switch does not crash;
- primitive fallback profile works;
- loaded model profile either shows models or reports clear diagnostics;
- idle render loop does not keep increasing render count after the scene becomes idle;
- create/dispose does not leave duplicate canvases.

## Deliverables

- implementation changes;
- tests;
- validation/evidence files under `artifacts/webgl-runtime-hardening-v2/`;
- short implementation report;
- updated README/runtime docs.

Do not copy this prompt bundle into the repo unless explicitly requested by the user. Prefer an implementation report and evidence folder only.

