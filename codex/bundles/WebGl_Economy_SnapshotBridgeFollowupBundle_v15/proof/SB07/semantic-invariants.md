# Semantic invariants SB07

Status: Completed

- Invariant ID: `SB07-ECONOMY-INITIAL-SCENE-SPLIT`
- Source raw note: RN-005/RN-007
- Expected behavior: Initial scene projection remains deterministic and behavior-compatible while layer, node, link, symbol, visual-state catalog, and diagnostics responsibilities are independently testable.
- Disallowed shallow implementation: splitting files without tests, changing initial scene object/link metadata, or re-enabling strict-mode diagnostic fallbacks.
- Failing-first test: `bundle://proof/SB07/transcripts/failing-first-initial-scene-split.txt`
- Passing tests: `bundle://proof/SB07/transcripts/economy-webgl-tests.txt`; `bundle://proof/SB07/transcripts/economy-tests.txt`; `bundle://proof/SB07/transcripts/economy-boundary-audit.txt`
- Changed source files: `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlLayerProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlNodeProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlLinkProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSymbolProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlVisualStateCatalogProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlProjectionDiagnostics.cs`
- Production assertions: `bundle://proof/SB07/source-assertions/initial-scene-split-source-map.txt`; `bundle://proof/SB07/source-assertions/anti-stub-scan.txt`
- Red-team negative case: no-op visual-state fallbacks and diagnostic fallback object remain opt-in after the split.
- Downstream dependency check: SB11/SB12 can work against smaller bridge projection types without relying on a large mixed-responsibility projector.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Split projector boundary | Focused projector classes | Initial scene projector | Each class owns one projection responsibility and is covered by focused unit tests. | Failing-first split proof and line-count source map. |
| Deterministic node-object binding | Initial scene projector and node projector | Action mapping context and snapshot visual state | Nodes remain ordered by id and object ids are stored in the context map. | Existing bridge tests plus full Economy tests. |
| Strict fallback catalog behavior | Visual-state catalog projector | SB06 strict mapping validator | No-op pose/symbol fallbacks are added only when `AllowDiagnosticFallback` is true. | Split catalog unit test and SB06 strict mapping tests. |
