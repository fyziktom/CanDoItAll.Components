# Architecture Boundary: WebGlLib vs Future Run Layer

## WebGlLib responsibility

`WebGlLib` is a generic rendering and interaction wrapper. It should answer:

- What should be rendered?
- Which asset should represent this object?
- Where is the object now?
- What is selected/hovered?
- What generic symbol is displayed above an object?
- How should an object transform be interpolated visually?
- How can a scene layout be exported/imported?
- What diagnostics/proof data can be captured?

## Future WebGlRunLib responsibility

A future run layer should answer:

- What is a run?
- When does a run start/pause/resume/finish?
- What is the authoritative simulation clock?
- Which domain event changes the scene?
- How are scenario frames saved/replayed?
- How are runs persisted?
- How is pathfinding/physics/collision handled?
- What domain-specific symbols are used?

## Practical decision table

| Feature | WebGlLib | Future Run/Game/Simulation layer |
| --- | --- | --- |
| Scene DTO | yes | consumes |
| Asset catalog | yes | provides domain-specific catalog |
| Asset quality profile | yes | selects profile per run/user/device |
| Primitive fallback | yes | consumes |
| Hover/selection/drag | yes | handles domain outcome |
| Export/import scene layout | yes | stores/replays scene layouts |
| Object transform patch | yes | produces patches |
| Basic object motion interpolation | yes | schedules motions |
| Motion completion event | yes | uses for run logic |
| Timeline/replay | no | yes |
| Save slots/storage providers | no | yes |
| Scenario persistence | no | yes |
| Pathfinding | no | yes |
| Physics/collision | no | yes |
| Economy/process/game semantics | no | yes |

## Current next step

Do not build the run layer now. Add only the generic seams needed by the run layer:

- scene document;
- command result;
- motion diagnostics/events;
- patch result parity;
- model diagnostics;
- idle-safe render scheduler.

