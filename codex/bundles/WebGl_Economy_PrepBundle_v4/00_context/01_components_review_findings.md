# 00. Components repo review findings

## Current branch reviewed

`webgl-engine`

## What is already good

- `01-webgl-scene.js` is now a thin façade over lifecycle, patching, motion, proof, diagnostics, and command APIs.
- Runtime concerns are split across focused JS modules:
  - `10-webgl-scene-lifecycle.js`
  - `11-webgl-scene-graph.js`
  - `12-webgl-scene-drag.js`
  - `13-webgl-scene-patching.js`
  - `14-webgl-scene-motion.js`
  - `15-webgl-scene-render-loop.js`
  - `16-webgl-scene-models.js`
  - `17-webgl-scene-resources.js`
  - `18-webgl-scene-model-diagnostics.js`
  - `19-webgl-scene-shell.js`
- `WebGlSceneView.razor` exposes generic scene operations: import, export, patch, motion, diagnostics, proof snapshot, and command result APIs.
- The runtime has render modes (`auto`, `continuous`, `on-demand`) and asset quality profiles.
- Model diagnostics now detect zero bounds, hidden meshes, transparent materials, extreme bounds, far-from-origin models, and clipping risks.
- Resource ownership helpers were added to avoid disposing shared GLB templates incorrectly.
- There is now an audit script for JS runtime size, unsafe HTML patterns, syntax, asset includes, and branch-creation instruction leakage.

## Remaining risks

### R1. Command result duplication

Patch and motion JS modules still build command result payloads locally. This can drift over time. Introduce one `webgl-scene-command-results.js` helper.

### R2. Lifecycle is still broad

`10-webgl-scene-lifecycle.js` remains a high-responsibility module. It owns state creation, renderer creation, lights, event handlers, disposal, import/export, and error notifications. It is below the hard line, but it is still a future growth risk.

### R3. Model diagnostics are good but not yet operationalized

There is a `ModelLab`, but the workflow needs a deterministic batch report for all external GLBs and a model import recipe file. This is important because user-supplied GLBs can load but be invisible.

### R4. Scene document hash is too shallow for future run playback

`WebGlSceneDocument` exists, but the split between content state, UI state, runtime options, and transient observation data needs to be explicit before run playback is introduced.

### R5. WebGlLib must not become a game engine

Basic transform interpolation belongs in WebGlLib. Simulation clocks, pathfinding, collision, scenario playback, and domain rules do not.

## Hard rule

Components remains generic. No economy terms, no ledger terms, no process terms, no community-well terms in `WebGlLib`.
