# WebGL JS Runtime Findings

## Positive

The runtime has been split into focused modules:
- public facade,
- lifecycle,
- core helpers,
- graph,
- patching,
- motion,
- motion queues,
- batch normalization,
- command batch execution,
- stage runner,
- scheduler,
- diagnostics.

This is the right direction. Avoid TypeScript for now.

## Current risk areas

### `14-webgl-scene-motion.js`
This file is still relatively large and owns:
- command normalization,
- active motion lifecycle,
- queue interaction,
- easing,
- vector lerp helpers,
- notification logic.

It is below the hard threshold but above the warning threshold. It should be split only if adding new functionality:
- `motion-normalization.js`
- `motion-easing.js`
- `motion-notifications.js`
- keep `14-webgl-scene-motion.js` as the public motion module.

### `28-webgl-scene-command-batch-normalizer.js`
This module is also near the warning threshold and duplicates concepts with the C# normalizer.
Before more features are added, add parity fixtures and snapshot tests rather than expanding it.

### `30-webgl-scene-stage-runner.js`
Good size and clear responsibility. Missing hardening:
- explicit state transition model,
- event/log entries for stage started/completed/skipped/failed,
- cancellation reason handling in diagnostics,
- stage timeout/maximum queue depth safety,
- test proving that waits and queued stages advance only when render loop ticks.

## Required guardrails

- Keep JS files under 320 lines hard threshold.
- Warning above 220 lines should require either refactoring or explicit allowlist with reason.
- Keep public facade thin.
- No domain words in generic WebGL runtime.
- No TypeScript migration.
- Prefer ES module helpers over large classes.
