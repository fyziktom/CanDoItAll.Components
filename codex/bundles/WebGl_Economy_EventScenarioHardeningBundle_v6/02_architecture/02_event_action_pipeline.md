# Generic scenario/event/action pipeline

Target conceptual pipeline:

```text
SimulationScenarioDefinition
  -> backend materialization
  -> SimulationFrame + SimulationEvent[]
  -> EconomyVisualFrame + EconomyVisualAction[]
  -> future bridge
  -> WebGlRunFrame + WebGlRunAction[]
  -> WebGlScenePatch + WebGlObjectMotionCommand
  -> WebGlLib runtime
```

## Example: shared well without hardcoding the engine

Domain event emitted by simulator:

```json
{
  "eventId": "event.water.draw.north.1",
  "kind": "resource-use",
  "actorId": "citizen.north",
  "targetObjectId": "object.well.main",
  "resourceId": "water",
  "quantity": 3,
  "durationSeconds": 8,
  "metadata": {
    "visual.intent": "move-use-return"
  }
}
```

Economy visual intention:

```json
{
  "actionId": "visual.water.draw.north.1",
  "kind": "move-use-return",
  "actorNodeId": "node.actor.citizen.north",
  "targetNodeId": "node.place.well.main",
  "statusSymbol": "symbol.water",
  "poseDuringAction": "carrying",
  "durationSeconds": 8
}
```

Future WebGL action:

```json
{
  "actionId": "run.water.draw.north.1",
  "kind": "sequence",
  "steps": [
    { "kind": "move-to-object", "objectId": "object.citizen.north", "targetObjectId": "object.well.main", "targetAnchor": "use" },
    { "kind": "change-pose", "objectId": "object.citizen.north", "poseKey": "carrying" },
    { "kind": "show-symbol", "objectId": "object.citizen.north", "symbolKey": "water" },
    { "kind": "move-to-object", "objectId": "object.citizen.north", "targetObjectId": "object.house.north", "targetAnchor": "home" }
  ]
}
```

The renderer only knows generic object ids, target ids, anchors, poses, and symbols. It does not know about wells, water, citizens, ledgers, or markets.
