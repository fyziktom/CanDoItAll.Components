# SB19 final red-team report

Result: passed.

| Question | Result | Evidence |
| --- | --- | --- |
| Can research-ready be claimed without real artifact bytes/hash? | No. Evidence refs now resolve files and validate bytes, sha256, and schemaVersion. | `proof/SB02/artifact-hash-proof.json`, `proof/SB19/economy-targeted-validation.txt` |
| Can source.* carry raw domain identifiers into generic run documents? | No. Source provenance is restricted to opaque tokens and strict hashes; raw ids move to the trace map. | `proof/SB03/trace-map-artifact.json`, `proof/SB03/provenance-opacity-tests.txt` |
| Do generic Components sources contain Economy semantics? | No production leakage found by the v12 Components audit. | `proof/SB04/domain-audit-source-gate.txt`, `proof/SB17/generic-boundary-report.txt` |
| Does multi-goods-elite exercise new semantics? | Yes. Contribution, claim, obligation, return, trade, fee, and design-matrix semantics are tested. | `proof/SB09/exchange-investment-driver-tests.txt`, `proof/SB10/multi-goods-headless-report.json` |
| Does browser observer compare actual exported browser state? | Yes. Real browser proof compares loaded document hash, scene content hash, driver hash, runtime idle, completed stages, and final positions. | `proof/SB14/browser-observer-proof.json`, `proof/SB14/playwright-transcript.txt` |
| Can economic result be reproduced headlessly without browser runtime? | Yes. CLI canary generated a headless-valid manifest/readiness report. | `proof/SB10/multi-goods-cli-run.txt`, `proof/SB10/multi-goods-readiness-report.json` |
| Is every warning/error classified? | Yes in focused validation; unclassified diagnostics are negative-tested. | `proof/SB15/diagnostic-classification-tests.txt`, `proof/SB15/unclassified-diagnostic-negative-test.txt` |
| Are there zero-byte proof transcripts? | No after cleanup and completed validation. | `proof/SB01/proof-integrity-inventory.txt`, `proof/SB19/completed-validator.txt` |

Validation commands:

- `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore --filter "WebGlRunValidatorTests" -m:1 -p:UseSharedCompilation=false -p:BuildInParallel=false`
- `node tools\webgllib\audit-webglrunlib-boundary.cjs --config tools\webgllib\domain-boundary-audit.config.json`
- `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore --filter "EconomyWebGlBridgeTests|EconomyWebGlBridgeReferenceTests|EconomyWebGlInitialSceneProjectorSplitTests|SimulationEconomicTrustHardeningTests|EconomyWebGlBridgeStrictMappingTests|EconomyWebGlBridgeStrictMappingDynamicTests|SimulationMetamorphicPropertyTests|EconomyPerformanceProbeTests" -m:1 -p:UseSharedCompilation=false -p:BuildInParallel=false`
- `.\scripts\audit-simulation-boundaries.ps1`
- `dotnet run --no-build --project src\CanDoItAll.Economy.Cli\CanDoItAll.Economy.Cli.csproj -- scenario run --catalog src\CanDoItAll.Economy.Node\SimulationScenarios\EconomySimulationSandbox --scenario multi-goods-elite --output <bundle>\proof\SB10\multi-goods-run --clean --no-oracle`
- WebGlSandbox browser proof at `http://127.0.0.1:5327/run-playback` using `proof\SB14\browser-observer-real-state.mjs`
- `python scripts\validate_bundle.py --stage completed`

Residual warnings:

- Economy targeted tests still emit existing NuGet warnings for `ncalc` and `Microsoft.Extensions.DependencyInjection.Abstractions`; no v12 assertion failed.
- The final multi-goods CLI canary is intentionally headless/no-oracle and therefore does not claim full research-ready status.
