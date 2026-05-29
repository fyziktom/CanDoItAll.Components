# SB03 — Command Result and Patch Hardening

## Goal

Make runtime commands deterministic, safe, and debuggable.

## Current issue

`13-webgl-scene-patching.js` has good detailed result support, but command result creation is duplicated between patching and motion. There is also a fragile path where `addSceneObject` can call `failPatch` with the wrong signature for invalid added objects.

## Tasks

1. Add a shared JS module:
   ```text
   20-webgl-scene-command-results.js
   ```
   Responsibilities:
   - `createCommandResult(state, commandKind, commandId)`
   - `completeCommandResult(state, result)`
   - `failCommand(state, result, message, runtimeErrorTitle)`
   - bounded command-result history
   - bounded failed-command history
   - stable timestamp-free fallback IDs in deterministic mode where possible.

2. Refactor:
   - `13-webgl-scene-patching.js`
   - `14-webgl-scene-motion.js`

3. Fix patch edge cases:
   - add object without id must fail cleanly and return detailed result.
   - remove missing object should warn, not silently succeed unless configured.
   - add link with missing endpoint should fail or warn according to explicit option.
   - baseRevision mismatch should be configurable: warn by default, fail in strict mode.
   - duplicate affected IDs should be deduplicated.

4. Add C# and JS proof coverage:
   - malformed patch does not throw.
   - detailed result contains errors.
   - state diagnostics records failed command.
   - successful patch increments revision exactly once.

5. Add `MaxCommandResultHistory` to runtime options or a constant with audit coverage.

## Done criteria

- No duplicated command result factories.
- Malformed patch commands never crash the runtime.
- Failed command history is bounded.
- Existing `ApplyPatchAsync` and `ApplyPatchDetailedAsync` continue to work.
