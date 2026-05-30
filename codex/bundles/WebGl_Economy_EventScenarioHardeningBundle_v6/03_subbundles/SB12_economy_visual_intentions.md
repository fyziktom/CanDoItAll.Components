# SB12 - Economy: visual intentions without WebGL

Extend `Simulation.Visualization` to map events to visual intentions.

Add:

- `EconomyVisualAction`
- `EconomyVisualActionTarget`
- `EconomyVisualActionSequence`
- `EconomyVisualPoseHint`
- `EconomyVisualSymbolHint`
- `IEconomyVisualActionMapper`

Action kinds must be generic:

- `move-to-target`
- `return-to-origin`
- `change-state`
- `show-symbol`
- `hide-symbol`
- `perform-work`
- `transfer-resource`
- `wait`
- `sequence`
- `parallel`

No WebGL types, no `WebGlVector3`, no Components references.

Shared-well mapping example:

- `resource-use` -> sequence: move-to-target, change-state(carrying), show-symbol(resource), return-to-origin.
- `actor-admin` -> change-state(writing), show-symbol(admin).
- `issue-raised(conflict)` -> show-symbol(conflict).
