# SB12 - Economy visual actions without WebGL dependency

Repository: `CanDoItAll.Economy`

## Goal

Convert simulation events into economy visual intentions without emitting WebGL DTOs.

## Add to Simulation.Visualization

```text
EconomyVisualAction
EconomyVisualActionKind
EconomyVisualActionTimeline
EconomyVisualActionMapper
EconomyVisualActionMappingPolicy
EconomyVisualNodeBinding
```

## Suggested visual action kinds

```text
move-to-target
return-to-home
change-pose
show-status-symbol
hide-status-symbol
show-resource-flow
pulse-relationship
wait
```

## Mapping examples

- `resource-use` -> move actor to resource location + show resource-use symbol + return home.
- `administration` -> set pose key `admin-writing` + show document symbol.
- `trade` -> move seller/buyer signals + resource flow visual.
- `rule-enforcement` -> show rule symbol + pulse relationship.
- `relationship-change` -> pulse link.

## Critical rule

Do not reference `CanDoItAll.Components.WebGlLib` or `WebGlRunLib`.
