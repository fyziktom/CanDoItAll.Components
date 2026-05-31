# Proof manifest SB06

Status: Completed

## Scope

Economy WebGL bridge mapping validation: strict default behavior for unresolved node/object mappings, missing pose/symbol mappings, opt-in diagnostic fallback, source traceability, and WebGlRun stage barrier handoff.

## Changed Files

- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`
- `economy://src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlMappedActionValidator.cs`
- `economy://tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs`

SHA-256 hashes:

- `bundle://proof/SB06/hashes/sha256.txt`

## Command Transcripts

- Failing-first strict mapping compile proof: `bundle://proof/SB06/transcripts/failing-first-economy-bridge-strict-mapping.txt`
- Focused bridge tests: `bundle://proof/SB06/transcripts/economy-webgl-bridge-tests.txt`
- Full Economy tests: `bundle://proof/SB06/transcripts/economy-tests.txt`
- Economy simulation boundary audit: `bundle://proof/SB06/transcripts/economy-boundary-audit.txt`
- Bundle prepared validator after SB06: `bundle://proof/SB06/transcripts/bundle-validator-prepared-after-sb06.txt`

## Source Assertions

- Strict mapping source map: `bundle://proof/SB06/source-assertions/strict-mapping-source-map.txt`
- Anti-stub scan: `bundle://proof/SB06/source-assertions/anti-stub-scan.txt`
- `AllowDiagnosticFallback` is false by default and is carried from projection options into the mapping context.
- Diagnostic object and no-op pose/symbol fallbacks are only materialized when `AllowDiagnosticFallback` is true.
- `EconomyWebGlMappedActionValidator` adds plan errors for unresolved subject/target objects and missing pose/symbol definitions in strict mode.
- `EconomyWebGlActionStageProjector` now copies `BarrierPolicy` and `BarrierObjectIds` from command-batch stages into emitted WebGlRun stages.
- Strict mapping tests cover unresolved subject, unresolved target, missing pose, missing symbol, and explicit diagnostic fallback traceability.

## Semantic Adequacy Gate

- Shallow-pass trap: a bridge can still emit a stage with diagnostics while moving a diagnostic fallback object, which looks executable but misrepresents unresolved simulation output.
- Adversarial negative proof: `bundle://proof/SB06/transcripts/failing-first-economy-bridge-strict-mapping.txt` shows the strict opt-in property was absent before implementation.
- Semantic positive proof: `bundle://proof/SB06/transcripts/economy-webgl-bridge-tests.txt` proves strict negative cases and fallback traceability; `bundle://proof/SB06/transcripts/economy-tests.txt` proves the wider test project remains green.
- Boundary proof: `bundle://proof/SB06/transcripts/economy-boundary-audit.txt` records `PASS: Economy simulation boundary audit passed.`
- Anti-stub audit: `bundle://proof/SB06/source-assertions/anti-stub-scan.txt` records no placeholder markers in changed bridge/test files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `AllowDiagnosticFallback` | `EconomyWebGlProjectionOptions` | Initial scene projector, mapping context, action mapper, strict validator | Defaults false; opt-in enables diagnostic object and no-op visual-state fallbacks. | Failing-first compile proof and strict unresolved-object test. |
| Strict mapped-action errors | `EconomyWebGlMappedActionValidator` | `EconomyWebGlActionStageProjector` plan validity gate | Missing subject/target/pose/symbol mappings become plan errors before command-batch compilation. | Missing-node, missing-target, missing-pose, and missing-symbol tests. |
| Stage trace metadata and barriers | Action stage projector and WebGlRun batch compiler | WebGlRun runtime frame application / JS stage runner | Source ids remain on emitted stages; barrier policy/object IDs survive projection. | Explicit fallback positive test asserts event/action/frame/input hash plus barrier fields. |

## Failures / Blockers

- No SB06 blocker.
- The first boundary-audit rerun failed because the new tests temporarily pushed `EconomyWebGlBridgeTests.cs` above the 500-line test gate. The strict mapping tests were split into `EconomyWebGlBridgeStrictMappingTests.cs`; the audit then passed.
- Economy test transcripts still include existing package warnings (`NU1701`, `NU1510`) and one pre-existing nullable warning in `InvestmentRevenueSharePaymentTests.cs`.
