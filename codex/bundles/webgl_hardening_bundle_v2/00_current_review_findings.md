# Current Review Findings

## What is already good

The current `main` contains the `CanDoItAll.Components.WebGlSandbox` project in the solution and a `tests/CanDoItAll.Components.WebGlLib.Tests` test project. This is a healthy direction because the WebGL sandbox is now separated from the general component sandbox and from process/economy modules.

The WebGL scene runtime was refactored: `01-webgl-scene.js` is now a thin facade that delegates lifecycle, patching, motion, render loop, assets, interaction, proof snapshots, and camera operations to smaller modules.

The current boundary document correctly says `WebGlLib` should own declarative scene contracts, asset catalogs, variants, generic interaction, patching, export/import, proof snapshots, basic motion interpolation, diagnostics, and render-loop policy, while future `WebGlRunLib` owns simulation clock semantics, run lifecycle, scenario playback, path planning, physics/collision, persistence semantics, and domain rules.

The previous hardening report says external/user GLB models were detected and registered as optional/high-detail alternatives, while primitives remain the default quality profile. This is the right default for tycoon-like usage.

## Remaining risks

### 1. JS lifecycle is better, but still mixes responsibilities

`10-webgl-scene-lifecycle.js` is still doing shell DOM construction, renderer/camera/lights setup, state construction, diagnostics construction, event attachment, notification helpers, import/export, update, and disposal. It is not terrible yet, but it is likely to become the next long fragile file.

Expected follow-up: extract shell, state factory, notifications, and disposal into dedicated modules while keeping `10-webgl-scene-lifecycle.js` as a small orchestrator.

### 2. Resource ownership and disposal need a hard pass

Loaded GLB templates and cloned instances need clear ownership rules. Shared template geometry should not be disposed per instance, but cloned materials and instance-only resources must be disposed. The current pattern uses `skipDispose`, which can accidentally skip owned cloned materials and cause leaks. Decorations are also removed during dispose but must be explicitly disposed.

Expected follow-up: introduce resource ownership flags or helper functions that can dispose owned materials while preserving shared template geometry.

### 3. Invisible model debugging is still too weak

The current model support loads external GLB variants, but model invisibility can be caused by empty scenes, zero-size bounds, huge/small unit scale, off-origin meshes, axis mismatch, transparent materials, disabled visibility, camera clipping, bad material side, or conversion artifacts. The runtime needs diagnostics and a model lab/proof page so bad converted models can be triaged quickly.

Expected follow-up: add import options, bounds visualization, material normalization toggles, asset diagnostics, and per-model proof output.

### 4. Asset catalog factory is becoming too large

`WebGlSandboxAssetCatalogFactory.cs` now mixes core assets, variants, external models, performance hints, and helper constructors. It will keep growing as more models are added.

Expected follow-up: split by responsibility and/or generate external model metadata from a manifest file.

### 5. Render loop still appears to keep an RAF loop alive

The render loop only renders on a reason, but still schedules `requestAnimationFrame` continuously. For tycoon-like scenes and future simulations this should be explicit: continuous mode should run continuously, motion/effects should run while active, and static scenes should sleep.

Expected follow-up: implement a scheduler that sleeps when idle and wakes on invalidation, motion, animated symbols, camera damping, or resize.

### 6. JS patching and C# patch reducer can drift

There is now a C# `WebGlScenePatchReducer` and a JS `13-webgl-scene-patching.js`. Both encode patch semantics. If they diverge, future run/state synchronization will become brittle.

Expected follow-up: align result shapes, add parity tests/fixtures, and add a command result object rather than only boolean returns.

### 7. Run preparation needs contracts, not a full engine

`WebGlLib` should not become a game engine. But it should expose enough generic primitives for a future run layer: scene document serialization, patch result, motion command result, deterministic proof snapshot, and command diagnostics. Simulation clock, pathfinding, persistence providers, replay logs, and domain-specific scenario semantics should remain outside.

