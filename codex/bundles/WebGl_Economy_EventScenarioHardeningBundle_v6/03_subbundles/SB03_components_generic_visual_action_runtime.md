# SB03 - Components: generic WebGlRunAction model and planner

Add a generic action model to `WebGlRunLib`.

Required action kinds:

- `sequence`
- `parallel`
- `wait`
- `apply-patch`
- `move-to-position`
- `move-to-object`
- `return-to-anchor`
- `change-pose`
- `show-symbol`
- `hide-symbol`
- `update-symbol`
- `set-layer-visibility`

Do not mention economy/well/water/ledger in public contracts.

Add:

- `WebGlRunAction`
- `WebGlRunActionTarget`
- `WebGlRunActionStep`
- `WebGlRunActionPlan`
- `WebGlRunActionPlanner`
- `WebGlRunActionPlanningDiagnostics`

Planner output:

- `WebGlScenePatch[]`
- `WebGlObjectMotionCommand[]`
- warnings for unresolved objects/anchors/assets/poses

Tests:

- move actor to target object and back;
- sequence with pose + symbol + movement;
- unresolved object returns failed diagnostic, not exception.
