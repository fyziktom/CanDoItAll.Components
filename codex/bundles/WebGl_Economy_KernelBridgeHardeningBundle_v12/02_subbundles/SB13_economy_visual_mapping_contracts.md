# SB13 - Economy Visual Mapping Contracts

## Problem

The bridge needs `visual.mapping.json`, but current Economy visual mapping is mostly code policy.

## Goal

Introduce a serializable visual mapping document independent from WebGL.

## Proposed concepts

```text
EconomyVisualMappingDefinition
  NodeMappings
  ActionMappings
  SymbolMappings
  PoseHints
  RelationshipMappings
  Fallbacks
```

## It should define

- actor kind -> visual category
- resource kind -> visual category
- event kind -> visual action kind
- action kind -> pose key / symbol category
- rule category -> symbol category
- fallback behavior when missing

## Important

Do not reference WebGL asset IDs here. Use Economy-neutral visual keys. The future bridge maps those keys to WebGL asset IDs.

Example:

```json
{
  "eventKind": "actor.resource.use",
  "actionKind": "sequence",
  "poseKey": "carry-resource",
  "symbolCategory": "resource"
}
```

## Tests

- Serialization round-trip for visual mapping definitions.
- Validation rejects direct WebGL asset IDs in Economy-neutral mapping fields.
- Shared-well and farmer-land fixtures can express event/action visual intent through the same mapping contract.
