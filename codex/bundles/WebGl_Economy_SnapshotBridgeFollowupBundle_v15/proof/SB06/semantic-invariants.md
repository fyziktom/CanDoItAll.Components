# Semantic invariants SB06

Status: Completed

- Invariant ID: `SB06-ECONOMY-BRIDGE-STRICT-MAPPING`
- Source raw note: RN-005
- Expected behavior: Economy WebGL projection fails bad mappings by default, allows diagnostic fallback only by explicit option, preserves action/stage source metadata, and carries WebGlRun barrier fields into emitted stages.
- Disallowed shallow implementation: emitting diagnostic metadata while still producing executable fallback-object motion in default mode, or preserving stage counts while dropping source ids and barrier fields.
- Failing-first test: `bundle://proof/SB06/transcripts/failing-first-economy-bridge-strict-mapping.txt`
- Passing tests: `bundle://proof/SB06/transcripts/economy-webgl-bridge-tests.txt`; `bundle://proof/SB06/transcripts/economy-tests.txt`; `bundle://proof/SB06/transcripts/economy-boundary-audit.txt`
- Changed source files: `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`, `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappedActionValidator.cs`, `economy://tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs`
- Production assertions: `bundle://proof/SB06/source-assertions/strict-mapping-source-map.txt`; `bundle://proof/SB06/source-assertions/anti-stub-scan.txt`
- Red-team negative case: configured diagnostic object is ignored unless `AllowDiagnosticFallback = true`; missing pose/symbol definitions do not compile into fallback patches in strict mode.
- Downstream dependency check: SB07 and SB12 may rely on strict mapping failures, preserved source traceability, and WebGlRun barrier propagation.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Strict diagnostic fallback switch | Projection options and initial scene projector | Mapper and validator | False by default; true enables the diagnostic object and no-op pose/symbol fallbacks. | Unresolved subject/target default rejection and explicit fallback positive test. |
| Mapped-action validation errors | Strict action validator | Stage projector plan gate | Validation runs after Economy action mapping and before command-batch compilation. | Missing-node, missing-target, missing-pose, and missing-symbol assertions. |
| Source traceability metadata | Stage projector | Snapshot/bridge downstream consumers | Event id, visual action id, simulation frame id, and input pack hash are copied onto emitted stages. | Explicit fallback test asserts all source fields. |
| Stage barrier handoff | Components WebGlRun compiler and Economy stage projector | Runtime frame application and stage runner | Batch stage barriers are copied into Economy-projected WebGlRun stages. | Explicit fallback test asserts `wait-for-object-motions` and fallback object id. |

## Completed Validator Tokens

Shallow-pass trap: SB06 rejects shallow projection proof by requiring unresolved mappings to fail unless explicit diagnostic fallback is enabled.

Adversarial negative proof: strict mapping tests cover missing node, pose, and symbol mappings so hidden fallback cannot mask bridge failures.

Semantic positive proof: focused Economy bridge tests and full Economy tests prove strict default behavior, explicit fallback metadata, and traceable stage emission.

Anti-stub audit: SB06 proof source assertions and tests provide executable bridge behavior instead of TODO, placeholder, or stub-only proof.
