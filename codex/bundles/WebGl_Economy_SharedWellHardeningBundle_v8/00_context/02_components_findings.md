# Components review findings

## Good progress

`WebGlRunLib` now provides a generic layer above `WebGlLib`. It has document, timeline, frame, playback, action, target, pose, symbol, and binding contracts.

The JS runtime is better structured than before:
- `01-webgl-scene.js` is a public facade.
- command batching is isolated in `26-webgl-scene-command-batch.js`.
- render scheduling is isolated in `22-webgl-scene-scheduler.js`.
- scene indexes are isolated in `23-webgl-scene-indexes.js`.
- command result helpers are isolated in `20-webgl-scene-command-results.js`.

## Key risks

### 1. Ordered action semantics are still fragile

A frame can include patches and motions, and batch normalization can coalesce patches and drop duplicate motions per object. That is useful for performance, but dangerous for sequential storytelling.

Example that must not collapse:
1. actor changes to walking pose;
2. actor moves to well;
3. actor shows water/use symbol;
4. actor changes to admin-writing pose;
5. actor returns home.

This must be represented as ordered stages, not just a flat coalesced patch/motion bag.

### 2. Duplicate alias fields increase ambiguity

`WebGlRunAction` contains compatibility aliases such as:
- `Kind` and `ActionKind`;
- `ObjectId` and `SubjectObjectId`;
- `TargetObjectId` and `Target.ObjectId`.

These help compatibility but must be normalized immediately into canonical fields. Internal code should not keep reading all aliases repeatedly.

### 3. JS/C# batch normalization can drift

There is a C# `WebGlSceneCommandBatchNormalizer` and a JS normalizer inside `26-webgl-scene-command-batch.js`. They must have parity tests, otherwise browser behavior will differ from C# tests.

### 4. Runtime performance risks

Potential bottlenecks:
- rebuilding/replacing object groups when only symbols changed;
- link updates scanning more than necessary;
- batch applying many child commands one-by-one internally;
- animated symbols causing continuous frames when only some symbols need animation;
- asset cache diagnostics not yet pressure-tested with repeated import/dispose cycles.

### 5. Desktop-only policy exists but must be enforced in new bundles

Do not add mobile/tablet/small-screen tasks. WebGL validation should use 1440x900 or larger.
