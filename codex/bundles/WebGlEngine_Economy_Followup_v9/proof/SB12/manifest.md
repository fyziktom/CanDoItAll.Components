# Proof manifest - SB12

Status: completed

## Scope

SB12 renders the multi-goods exchange/investment scenario through generic WebGL objects, links, symbols, and stages. Domain vocabulary remains in Economy input/source provenance; executable Components concepts stay generic.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB12/transcripts/changed-file-hashes.txt`

Components production files:

- `repo://C:/repositories/CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
- `repo://C:/repositories/CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl-proof.js`
- `repo://C:/repositories/CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingRendererBinding.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingDefinition.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingActionDefinitions.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingValidation.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualActionMappingPolicy.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualCategoryPolicy.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRendererBindingResolver.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlNodeProjector.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlVisualStateCatalogProjector.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSymbolProjector.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlProjectionDiagnostics.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappingBoundary.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs`

Economy fixtures and tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/visual.mapping.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/scenario.definition.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/multi-goods-elite/experiment.json`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentLoaderTests.cs`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `rendererBinding` visual assets | Economy visual mapping abstraction | Economy WebGL bridge resolver | Renderer-specific category, pose, symbol, anchor, and diagnostic object bindings are isolated from neutral mapping categories | Failing-first readiness report shows direct `assetId`, `symbolAssetId`, and `anchorKey` fields produced `bridge-bound-visual-field` diagnostics before SB12 |
| Generic executable boundary | Economy WebGL bridge strict validator | WebGlRunLib document validator | Object kind/family and stage ids reject `buyer`, `seller`, `investor`, and `elite`; source provenance remains allowed | Strict mapping test and browser assertion report zero `genericityFailures` |
| SB11 event visualization | Economy visualization policy | Visual frames and WebGL run projector | Contribution, claim, return, and obligation events become generic transfer/relationship visual actions with capital/obligation categories | Focused tests pass the multi-goods fixture with no bridge-bound diagnostics and all generic event mappings present |
| Generated multi-goods run document | Economy real-scenario runner | Browser proof loader | Real artifacts are exported, hash-validated, and loaded into `/run-playback` instead of relying on a sandbox sample | Browser proof confirms generated run id, matching document hashes, 23 objects, 12 links, and 1 generic stage |
| Browser proof document loading | WebGlSandbox proof bridge | Playwright proof script | Large run documents are loaded by chunked JS interop, reset into the sandbox, and played to idle | Browser proof records `observerProofValid=true`, `runtimeIdle=true`, and no disallowed console messages |
| Nullable patch handling | WebGlLib scene patching | Browser runtime object application | Optional nullable patch fields are ignored when absent/null instead of resetting positions to defaults | Browser proof confirms policy-board scene/object/group positions remain `(4, 3, 0)` after playback |

## Proof Artifacts

- Failing-first readiness gap: `bundle://proof/SB12/failing-first-bridge-bound-readiness.txt`
- Focused generic visualization tests: `bundle://proof/SB12/transcripts/generic-visualization-tests.txt`
- Real run export transcript: `bundle://proof/SB12/transcripts/real-run-export.txt`
- Real run artifacts: `bundle://proof/SB12/economy-real-run/multi-goods-elite/`
- Browser observer proof script: `bundle://proof/SB12/browser/multi-goods-observer-proof.mjs`
- Browser proof transcript: `bundle://proof/SB12/transcripts/multi-goods-browser-playwright.txt`
- Browser assertions: `bundle://proof/SB12/browser/multi-goods-browser-assertions.json`
- Browser observer diagnostics: `bundle://proof/SB12/browser/multi-goods-observer-proof.json`
- Browser screenshot: `bundle://proof/SB12/browser/multi-goods-browser-after.png`
- Browser console log: `bundle://proof/SB12/browser/multi-goods-browser-console.log`
- WebGlSandbox build: `bundle://proof/SB12/transcripts/webgl-sandbox-build.txt`
- Source assertions: `bundle://proof/SB12/transcripts/source-assertions.txt`
- Changed-file hashes: `bundle://proof/SB12/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB12/transcripts/anti-stub-audit.txt`
- Bundle validator transcript: `bundle://proof/SB12/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB12/semantic-invariants.md`

## Closure

SB12 passes. The previous `bridge-bound-visual-field` readiness diagnostics are eliminated by moving renderer-specific IDs into `rendererBinding`, the multi-goods scenario projects through strict generic executable validation, and `/run-playback` loads the generated run document with matching hashes, completed stage observation, valid browser observer proof, runtime idle, and no disallowed console output.
