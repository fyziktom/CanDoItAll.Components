# SB03 - Components executable run document controller

Codex must verify or implement a reusable generic controller that can execute:

- seek frame,
- apply frame stages,
- export runtime snapshot,
- pause/resume,
- step forward/back in document timeline,
- report current stage/action ids.

It must not know Economy.

It must output enough data for Economy to attach runtime state into `SimulationRunSnapshot.VisualState`.
