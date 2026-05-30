# Economy to WebGL Visual Bridge Design

## Status

Design only. This bundle does not add a project reference from Economy to Components, nor from Components to Economy.

## Contract Shape

Future bridge consumers can translate Economy visualization output into WebGlRunLib input with a separate adapter package or application layer:

| Economy visual action | WebGlRun action | Notes |
| --- | --- | --- |
| `move-to-target` | `move-to-object` | Resolve `TargetObjectId` from the visual binding index before creating the WebGL action. |
| `return-to-home` | `return-to-anchor(home)` | Use actor home metadata or the canonical `home` anchor. |
| `change-pose` | `set-pose` | Pose ids must be catalog validated before playback. |
| `show-symbol` | `show-symbol` | Symbol ids must be catalog validated before playback. |
| `transfer-resource` | `resource-transfer-visual` | Adapter may emit a grouped/sequence action with source, target, and resource metadata. |
| `pulse-relationship` | `pulse-link` | Link ids should use deterministic source/target relationship ids. |

## Object Id Conventions

Bridge consumers should treat Economy visual ids as stable semantic keys and map them to WebGL object ids with explicit prefixes:

| Economy entity | WebGL object id |
| --- | --- |
| Actor `actor-1` | `node.actor.actor-1` |
| Location `shared-well` | `node.location.shared-well` |
| Resource store `water-store` | `node.resource.water-store` |
| Relationship `a-b` | `link.relationship.a-b` |

The adapter should prefer explicit visual bindings emitted by Economy. Prefix conventions are fallback conventions for consumers that need to construct object ids from normalized simulation definitions.

## Ordering

The adapter must preserve `StepIndex`, `Offset`, `Duration`, `Order`, and `EventId` ordering from `EconomyVisualActionMapper`. Sequence children marked as internal steps should remain nested under the parent WebGL grouped action and should not be promoted into sibling timeline stages.

## Boundary Rules

- Economy remains independent of Components and WebGlRunLib.
- Components remains generic and contains no Economy-specific scenario code.
- Shared resource, store, relationship, and actor names are data carried by action metadata, not hard-coded adapter behavior.
- Large-screen WebGL readiness is the only UI proof target for this bundle.
