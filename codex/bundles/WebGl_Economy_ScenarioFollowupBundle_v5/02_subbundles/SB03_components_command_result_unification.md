# SB03 - Components command result unification

Repository: `CanDoItAll.Components`

## Goal

Make every command path return the same result shape and history semantics.

## Tasks

1. Ensure `20-webgl-scene-command-results.js` is the only command-result builder.
2. Remove local/duplicate result builders from patching and motion modules.
3. Add detailed result methods for:
   - import scene;
   - export scene validation;
   - layer visibility changes;
   - action-plan execution later.
4. Add `OnCommandCompleted` and `OnCommandFailed` callbacks or document why `MotionCompleted` is enough for now.
5. Add C# tests for `WebGlSceneCommandResult` serialization roundtrip.

## Done

- No duplicate JS command result factory remains outside `20-webgl-scene-command-results.js`.
- Command history is bounded and deterministic.
- Failed command diagnostics include command kind, command id, scene id, affected ids, errors, warnings.
