# Proof manifest SB07

Status: Completed

## Scope

Economy initial scene projector refactor: split layer, node, link, symbol, visual-state catalog, and projection diagnostics responsibilities while preserving bridge output and strict mapping behavior.

## Changed Files

- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlLayerProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlNodeProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlLinkProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSymbolProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlVisualStateCatalogProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlProjectionDiagnostics.cs`
- `economy://tests/CanDoItAll.Economy.Tests/EconomyWebGlInitialSceneProjectorSplitTests.cs`

SHA-256 hashes:

- `bundle://proof/SB07/hashes/sha256.txt`

## Command Transcripts

- Failing-first source split proof: `bundle://proof/SB07/transcripts/failing-first-initial-scene-split.txt`
- Focused Economy WebGL tests: `bundle://proof/SB07/transcripts/economy-webgl-tests.txt`
- Full Economy tests: `bundle://proof/SB07/transcripts/economy-tests.txt`
- Economy simulation boundary audit: `bundle://proof/SB07/transcripts/economy-boundary-audit.txt`
- Bundle prepared validator after SB07: `bundle://proof/SB07/transcripts/bundle-validator-prepared-after-sb07.txt`

## Source Assertions

- Initial scene split source map and line counts: `bundle://proof/SB07/source-assertions/initial-scene-split-source-map.txt`
- Anti-stub scan: `bundle://proof/SB07/source-assertions/anti-stub-scan.txt`
- `EconomyWebGlInitialSceneProjector.cs` was reduced from 316 lines to 108 lines.
- Split production files are all below the production size gate: layer 36, link 50, node 103, diagnostics 66, symbol 35, catalog 79 lines.
- Focused unit tests cover every requested split responsibility: layer, node, link, symbol, visual-state catalog, and diagnostics.

## Semantic Adequacy Gate

- Shallow-pass trap: moving code into files could satisfy a line-count scan while changing layer ordering, node metadata, link diagnostics, symbol mapping, or fallback behavior.
- Adversarial negative proof: `bundle://proof/SB07/transcripts/failing-first-initial-scene-split.txt` records that the target split types were absent and the old projector was 316 lines.
- Semantic positive proof: `bundle://proof/SB07/transcripts/economy-webgl-tests.txt` proves focused bridge/projector behavior; `bundle://proof/SB07/transcripts/economy-tests.txt` proves the wider test project remains green.
- Boundary proof: `bundle://proof/SB07/transcripts/economy-boundary-audit.txt` records `PASS: Economy simulation boundary audit passed.`
- Anti-stub audit: `bundle://proof/SB07/source-assertions/anti-stub-scan.txt` records no placeholder markers in changed split files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Initial scene orchestration | `EconomyWebGlInitialSceneProjector` | Run projector and bridge callers | Creates document/context, delegates focused projection responsibilities, records counts. | Focused and full bridge tests preserve initial scene output. |
| Layer/node/link/symbol projection | Split focused projectors | Initial scene projector | Projectors own local ordering, mapping, metadata, and diagnostics behavior. | Dedicated split tests assert each responsibility. |
| Visual state catalog projection | `EconomyWebGlVisualStateCatalogProjector` | Strict mapping validator and WebGlRun planner | Builds pose/symbol/action catalog and only adds no-op fallbacks when diagnostic fallback is enabled. | Catalog unit test asserts strict vs opt-in fallback. |
| Projection diagnostics | `EconomyWebGlProjectionDiagnostics` | Split projectors and initial scene orchestration | Centralizes missing-frame, missing-asset, unresolved-link, and diagnostic object behavior. | Diagnostics unit test asserts fallback object opt-in and no-frame diagnostic. |

## Failures / Blockers

- No SB07 blocker.
- First focused compile run failed after the split because `EconomyWebGlInitialSceneProjector.cs` was missing the `Simulation.Abstractions` using for `EconomyVisualMappingDefinition`; the import was added and all validation passed.
- Economy test transcripts still include existing package warnings (`NU1701`, `NU1510`) and one pre-existing nullable warning in `InvestmentRevenueSharePaymentTests.cs`.
