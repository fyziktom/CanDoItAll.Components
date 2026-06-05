# SB07 Mapping Boundary Report

## Result

Status: passed.

Economy-specific words now cross into Components only as allowed source provenance values, such as `source.domain`, `source.visualActionId`, `source.eventId`, `source.parentId`, `source.traceId`, and `source.inputPackHash`. Executable ids consumed by Components are generic stable tokens.

## Boundary Implementation

- `EconomyWebGlMappingBoundary` owns the Economy-specific forbidden-term list and exposes `StrictGenericBoundaryOptions` for `WebGlRunDocumentValidator`.
- `EconomyVisualActionWebGlMapper` maps every source `EconomyVisualAction.ActionId` to `visual-action.<hash>` before Components planning/compilation.
- Original domain action ids and event ids are preserved only as `source.visualActionId` and `source.eventId`.
- `EconomyWebGlRunValidator` now runs `new WebGlRunDocumentValidator(EconomyWebGlMappingBoundary.StrictGenericBoundaryOptions)` and reports failures as `generic-boundary`.
- `EconomyWebGlBridgeDiagnosticsAggregator` writes generic diagnostic tokens like `missing-visual-asset.<hash>` and `unsupported-action-kind.<hash>` into document metadata instead of raw domain diagnostic strings.
- Arbitrary `input.Metadata` is no longer copied into `bridgeInput.*`; `scenarioId` maps to `source.parentId`.

## Projector Metadata Review

| Surface | Allowed output | Boundary decision |
| --- | --- | --- |
| `EconomyWebGlRunProjector` document metadata | `source.kind`, `source.domain`, `source.traceId`, `source.inputPackHash`, `source.parentId`, `bridge`, `nodeObjectMappingCount`, `frameCount`, `diagnostic.*`, `diagnosticCount` | Domain identifiers allowed only under `source.*`; diagnostics are generic tokens. |
| `EconomyWebGlInitialSceneProjector` document/scene metadata | `source.kind`, `source.domain`, `source.traceId`, `source.inputPackHash`, `bridge`, object/link/visual-state counts, no-op warnings | Scene provenance remains structural; run strict validation covers run/timeline metadata. |
| `EconomyWebGlNodeProjector` object metadata | `source.nodeId`, `source.nodeKind`, `source.category`, `source.layout.zone`, `metric.*` | Scene-object source details stay in scene metadata, not executable run action/stage ids. |
| `EconomyWebGlLinkProjector` link metadata | `source.linkId`, `source.sourceNodeId`, `source.targetNodeId`, `source.category` | Link domain identity remains source provenance in scene metadata. |
| `EconomyWebGlActionStageProjector` frame metadata | `source.kind`, `source.domain`, `source.simulationFrameId`, `source.parentId`, `source.inputPackHash`, `source.sequence`, `frameHash`, `orderingMode`, `batchingPolicy`, `stageCount`, `commandCount` | Frame source ids are provenance; execution controls are generic Components vocabulary. |
| `EconomyWebGlActionStageProjector` stage metadata | `source.visualActionId`, `source.eventId`, `source.simulationFrameId`, `source.inputPackHash`, `commandBatchId`, explicit wait markers, generic compiler keys | Stage ids and command ids are generic tokens; original domain ids are source provenance only. |
| `EconomyWebGlProjectionDiagnostics` document metadata | `diagnostic.<n>=<generic-code>.<hash>`, `diagnosticCount` | Raw diagnostic text with domain ids is not stored in the run document. |
| `EconomyWebGlVisualStateCatalogProjector` visual catalog metadata | pose/symbol/action mapping metadata controlled by visual mapping | Visual catalog data is renderer mapping data and is not used as Components action/stage ids. |

## Strict Generic Boundary Proof

- `ProjectorMapsDomainActionIdsToGenericExecutableStageIds` projects an action id containing `market`, `water`, and `ledger`, then proves the emitted stage and motion ids are `visual-action.<hash>` tokens while source provenance still carries the original ids.
- `ValidatorRejectsDomainTermsInExecutableStageIds` mutates a valid document to `stage.market.water-ledger` and proves `EconomyWebGlRunValidator` rejects it with `generic-boundary`.
- `ValidatorAcceptsPositiveStrictMappingWithoutFallbacks` runs `WebGlRunDocumentValidator(EconomyWebGlMappingBoundary.StrictGenericBoundaryOptions)` on a positive bridge document.
- The fixture strict mapping tests pass for `shared-well` and `farmer-land`, proving raw fixture/domain diagnostics no longer leak into non-source run metadata.

## Evidence

- Economy boundary tests: `bundle://proof/SB07/economy-webgl-boundary-tests.txt`
- Components strict validator regression: `bundle://proof/SB07/webglrunlib-tests.txt`
- Source assertions: `bundle://proof/SB07/transcripts/source-assertions.txt`
- Failing-first leak transcript: `bundle://proof/SB07/economy-strict-mapping-tests.txt`
