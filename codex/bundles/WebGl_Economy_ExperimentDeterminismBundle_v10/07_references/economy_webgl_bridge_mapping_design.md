# Economy to WebGL bridge mapping design

## Boundary rule

This bundle does not add any direct project reference between `CanDoItAll.Economy` and `CanDoItAll.Components`. The future bridge should live in an adapter package or app composition layer that already references both sides.

## Mapping table

| Economy source | Bridge responsibility | Components target |
| --- | --- | --- |
| `EconomyVisualAction.ActionKind` | Translate visual intention kind to generic run action kind. | `WebGlRunAction.ActionKind` |
| `EconomyVisualAction.SubjectNodeId` | Resolve visual node binding to a scene object id. | `WebGlRunAction.SubjectObjectId` |
| `EconomyVisualAction.Target.NodeId` | Resolve target node and anchor hint. | `WebGlRunAction.Target.ObjectId` / `Target.AnchorKey` |
| `EconomyVisualAction.PoseKey` | Pass through generic pose key. | `WebGlRunAction.PoseKey` |
| `EconomyVisualAction.SymbolCategory` | Pass through generic symbol key/category. | `WebGlRunAction.SymbolKey` / parameters |
| `EconomyVisualAction.Timeline` | Preserve step/start/duration ordering. | `WebGlRunAction.StartsAtSeconds`, `DurationSeconds`, stage metadata |
| `EconomyVisualFrame.Nodes` | Produce scene objects or patch targets. | `WebGlSceneDocument.Scene.Objects` / `WebGlScenePatch.ObjectPatches` |
| `EconomyVisualFrame.Links` | Produce scene links or link patches. | `WebGlSceneDocument.Scene.Links` / `WebGlScenePatch` |
| `SimulationExperimentInputPack` hashes | Copy provenance without Economy types. | `WebGlRunDocument.Metadata` and `RunSourceRef` |

## Future adapter flow

1. Load Economy visual frames and actions from a simulation run artifact.
2. Build a domain-neutral node binding index that maps Economy node ids to WebGL scene object ids.
3. Normalize Economy actions with `EconomyVisualActionNormalizer`.
4. Convert actions to `WebGlRunAction` using only generic action kinds, target refs, pose keys, symbol keys, timeline data, and stage metadata.
5. Build `WebGlRunDocument.Metadata` with input-pack hashes through generic provenance keys.
6. Validate WebGL provenance with `WebGlRunDocumentProvenanceValidator`.

## Non-goals for this bundle

- No Economy project references to Components.
- No Components project references to Economy.
- No shared-well demo UI.
- No WebGL asset ids embedded in Economy output.
